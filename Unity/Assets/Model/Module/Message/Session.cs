using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ETModel
{
    /// <summary>
    /// Session初始化时调用
    /// </summary>
	[ObjectSystem]
    public class SessionAwakeSystem : AwakeSystem<Session, AChannel>
    {
        public override void Awake(Session self, AChannel b)
        {
            self.Awake(b);
        }
    }

    /// <summary>
    /// 远端会话类抽象类
    /// </summary>
    public sealed class Session : Entity
    {
        /// <summary>
        /// 全局唯一的RPC包Id
        /// 用于为每个消息添加唯一Id，以便把请求和应答关联
        /// </summary>
        private static int RpcId { get; set; }

        /// <summary>
        /// 网络连接对象
        /// </summary>
        private AChannel channel;

        /// <summary>
        /// 等待应答的请求回调处理
        /// </summary>
        private readonly Dictionary<int, Action<IResponse>> requestCallback = new Dictionary<int, Action<IResponse>>();

        /// <summary>
        /// 发消息用的一个buffer缓存变量，用于减少GC
        /// </summary>
        private readonly List<byte[]> byteses = new List<byte[]>() { new byte[1], new byte[2] };

        /// <summary>
        /// 具体网络连接组件
        /// </summary>
        public NetworkComponent Network
        {
            get
            {
                return this.GetParent<NetworkComponent>();
            }
        }

        /// <summary>
        /// 当前连接的错误码
        /// </summary>
        public int Error
        {
            get
            {
                return this.channel.Error;
            }
            set
            {
                this.channel.Error = value;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="aChannel">连接对象</param>
        public void Awake(AChannel aChannel)
        {
            this.channel = aChannel;
            this.requestCallback.Clear();
            long id = this.Id;
            channel.ErrorCallback += (c, e) =>
            {
                this.Network.Remove(id);
            };
            channel.ReadCallback += this.OnRead;
        }

        /// <summary>
        /// 资源销毁处理
        /// </summary>
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            long id = this.Id;

            base.Dispose();

            foreach (Action<IResponse> action in this.requestCallback.Values.ToArray())
            {
                action.Invoke(new ResponseMessage { Error = this.Error });
            }

            //int error = this.channel.Error;
            //if (this.channel.Error != 0)
            //{
            //	Log.Trace($"session dispose: {this.Id} ErrorCode: {error}, please see ErrorCode.cs!");
            //}

            this.channel.Dispose();
            this.Network.Remove(id);
            this.requestCallback.Clear();
        }

        /// <summary>
        /// 会话开启处理
        /// </summary>
        public void Start()
        {
            this.channel.Start();
        }

        /// <summary>
        /// 远端地址
        /// </summary>
        public IPEndPoint RemoteAddress
        {
            get
            {
                return this.channel.RemoteAddress;
            }
        }

        /// <summary>
        /// 网络连接类型
        /// </summary>
        public ChannelType ChannelType
        {
            get
            {
                return this.channel.ChannelType;
            }
        }

        /// <summary>
        /// 当前连接的数据流
        /// </summary>
        public MemoryStream Stream
        {
            get
            {
                return this.channel.Stream;
            }
        }

        /// <summary>
        /// 收到数据的处理
        /// </summary>
        /// <param name="memoryStream"></param>
        public void OnRead(MemoryStream memoryStream)
        {
            try
            {
                this.Run(memoryStream);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        /// <summary>
        /// 处理收到数据的事件
        /// </summary>
        /// <param name="memoryStream"></param>
        private void Run(MemoryStream memoryStream)
        {
            memoryStream.Seek(Packet.MessageIndex, SeekOrigin.Begin);
            byte flag = memoryStream.GetBuffer()[Packet.FlagIndex];
            ushort opcode = BitConverter.ToUInt16(memoryStream.GetBuffer(), Packet.OpcodeIndex);

#if !SERVER
			if (OpcodeHelper.IsClientHotfixMessage(opcode))
			{
				this.GetComponent<SessionCallbackComponent>().MessageCallback.Invoke(this, flag, opcode, memoryStream);
				return;
			}
#endif

            object message;
            try
            {
                OpcodeTypeComponent opcodeTypeComponent = this.Network.Entity.GetComponent<OpcodeTypeComponent>();
                object instance = opcodeTypeComponent.GetInstance(opcode);
                message = this.Network.MessagePacker.DeserializeFrom(instance, memoryStream);

                if (OpcodeHelper.IsNeedDebugLogMessage(opcode))
                {
                    Log.Msg(message);
                }
            }
            catch (Exception e)
            {
                // 出现任何消息解析异常都要断开Session，防止客户端伪造消息
                Log.Error($"opcode: {opcode} {this.Network.Count} {e} ");
                this.Error = ErrorCode.ERR_PacketParserError;
                this.Network.Remove(this.Id);
                return;
            }

            // flag第一位为1表示这是rpc返回消息,否则交由MessageDispatcher分发
            if ((flag & 0x01) == 0)
            {
                this.Network.MessageDispatcher.Dispatch(this, opcode, message);
                return;
            }

            IResponse response = message as IResponse;
            if (response == null)
            {
                throw new Exception($"flag is response, but message is not! {opcode}");
            }
            Action<IResponse> action;
            if (!this.requestCallback.TryGetValue(response.RpcId, out action))
            {
                return;
            }
            this.requestCallback.Remove(response.RpcId);

            action(response);
        }

        /// <summary>
        /// 阻塞式地调用Actor
        /// </summary>
        /// <param name="request">请求的具体对象</param>
        /// <returns></returns>
        public Task<IResponse> Call(IRequest request)
        {
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) =>
            {
                try
                {
                    if (ErrorCode.IsRpcNeedThrowException(response.Error))
                    {
                        throw new RpcException(response.Error, response.Message);
                    }

                    tcs.SetResult(response);
                }
                catch (Exception e)
                {
                    tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName}", e));
                }
            };

            request.RpcId = rpcId;
            this.Send(0x00, request);
            return tcs.Task;
        }

        /// <summary>
        /// 以可取消的方式阻塞式地调用Actor
        /// </summary>
        /// <param name="request">请求参数对象</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IResponse> Call(IRequest request, CancellationToken cancellationToken)
        {
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) =>
            {
                try
                {
                    if (ErrorCode.IsRpcNeedThrowException(response.Error))
                    {
                        throw new RpcException(response.Error, response.Message);
                    }

                    tcs.SetResult(response);
                }
                catch (Exception e)
                {
                    tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName}", e));
                }
            };

            cancellationToken.Register(() => this.requestCallback.Remove(rpcId));

            request.RpcId = rpcId;
            this.Send(0x00, request);
            return tcs.Task;
        }

        /// <summary>
        /// 直接发送一个消息(发送后直接返回)
        /// </summary>
        /// <param name="message">消息对象</param>
        public void Send(IMessage message)
        {
            this.Send(0x00, message);
        }

        /// <summary>
        /// 应答一个消息
        /// </summary>
        /// <param name="message">应答的消息对象</param>
        public void Reply(IResponse message)
        {
            if (this.IsDisposed)
            {
                throw new Exception("session已经被Dispose了");
            }

            this.Send(0x01, message);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="flag">消息第一个字节的标记， flag第一位为1表示这是rpc返回消息,否则交由MessageDispatcher分发</param>
        /// <param name="message">消息对象</param>
        public void Send(byte flag, IMessage message)
        {
            OpcodeTypeComponent opcodeTypeComponent = this.Network.Entity.GetComponent<OpcodeTypeComponent>();
            ushort opcode = opcodeTypeComponent.GetOpcode(message.GetType());

            Send(flag, opcode, message);
        }

        /// <summary>
        /// 发送一个消息
        /// </summary>
        /// <param name="flag">消息第一个字节的标记， flag第一位为1表示这是rpc返回消息,否则交由MessageDispatcher分发</param>
        /// <param name="opcode">消息类型码</param>
        /// <param name="message">消息对象</param>
        public void Send(byte flag, ushort opcode, object message)
        {
            if (this.IsDisposed)
            {
                throw new Exception("session已经被Dispose了");
            }

            if (OpcodeHelper.IsNeedDebugLogMessage(opcode))
            {
#if !SERVER
				if (OpcodeHelper.IsClientHotfixMessage(opcode))
				{
				}
				else
#endif
                {
                    Log.Msg(message);
                }
            }

            MemoryStream stream = this.Stream;

            stream.Seek(Packet.MessageIndex, SeekOrigin.Begin);
            stream.SetLength(Packet.MessageIndex);
            this.Network.MessagePacker.SerializeTo(message, stream);
            stream.Seek(0, SeekOrigin.Begin);

            this.byteses[0][0] = flag;
            this.byteses[1].WriteTo(0, opcode);
            int index = 0;
            foreach (var bytes in this.byteses)
            {
                Array.Copy(bytes, 0, stream.GetBuffer(), index, bytes.Length);
                index += bytes.Length;
            }

#if SERVER
            // 如果是allserver，内部消息不走网络，直接转给session,方便调试时看到整体堆栈
            if (this.Network.AppType == AppType.AllServer)
            {
                Session session = this.Network.Entity.GetComponent<NetInnerComponent>().Get(this.RemoteAddress);
                session.Run(stream);
                return;
            }
#endif

            this.Send(stream);
        }

        /// <summary>
        /// 直接发送一个数据流
        /// </summary>
        /// <param name="stream"></param>
        public void Send(MemoryStream stream)
        {
            channel.Send(stream);
        }
    }
}