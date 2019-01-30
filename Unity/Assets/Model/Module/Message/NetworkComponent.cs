using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ETModel
{
    /// <summary>
    /// 网络连接处理组件基类
    /// </summary>
	public abstract class NetworkComponent : Component
    {
        /// <summary>
        /// APP类型
        /// </summary>
        public AppType AppType;

        /// <summary>
        /// 服务对象
        /// </summary>
        protected AService Service;

        /// <summary>
        /// 会话列表
        /// </summary>
        private readonly Dictionary<long, Session> sessions = new Dictionary<long, Session>();

        public IMessagePacker MessagePacker { get; set; }

        public IMessageDispatcher MessageDispatcher { get; set; }

        /// <summary>
        /// 服务加载处理
        /// </summary>
        /// <param name="protocol">协议类型/param>
        /// <param name="packetSize">包长度的字节数</param>
        public void Awake(NetworkProtocol protocol, int packetSize = Packet.PacketSizeLength2)
        {
            switch (protocol)
            {
                case NetworkProtocol.KCP:
                    this.Service = new KService();
                    break;
                case NetworkProtocol.TCP:
                    this.Service = new TService(packetSize);
                    break;
                case NetworkProtocol.WebSocket:
                    this.Service = new WService();
                    break;
            }
        }

        /// <summary>
        /// 服务加载处理
        /// </summary>
        /// <param name="protocol">协议类型/param>
        /// <param name="address">本地监听地址</param>
        /// <param name="packetSize">包长度的字节数</param>
        public void Awake(NetworkProtocol protocol, string address, int packetSize = Packet.PacketSizeLength2)
        {
            try
            {
                IPEndPoint ipEndPoint;
                switch (protocol)
                {
                    case NetworkProtocol.KCP:
                        ipEndPoint = NetworkHelper.ToIPEndPoint(address);
                        this.Service = new KService(ipEndPoint, this.OnAccept);
                        break;
                    case NetworkProtocol.TCP:
                        ipEndPoint = NetworkHelper.ToIPEndPoint(address);
                        this.Service = new TService(packetSize, ipEndPoint, this.OnAccept);
                        break;
                    case NetworkProtocol.WebSocket:
                        string[] prefixs = address.Split(';');
                        this.Service = new WService(prefixs, this.OnAccept);
                        break;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"NetworkComponent Awake Error {address}", e);
            }
        }

        /// <summary>
        /// 当前连接数量
        /// </summary>
        public int Count
        {
            get { return this.sessions.Count; }
        }

        /// <summary>
        /// 收到连接时的处理
        /// </summary>
        /// <param name="channel">连接对象</param>
        public void OnAccept(AChannel channel)
        {
            Session session = ComponentFactory.CreateWithParent<Session, AChannel>(this, channel);
            this.sessions.Add(session.Id, session);
            session.Start();
        }

        /// <summary>
        /// 移除会话
        /// </summary>
        /// <param name="id">会话Id</param>
        public virtual void Remove(long id)
        {
            Session session;
            if (!this.sessions.TryGetValue(id, out session))
            {
                return;
            }
            this.sessions.Remove(id);
            session.Dispose();
        }

        /// <summary>
        /// 根据会话Id获取会话对象
        /// </summary>
        /// <param name="id">会话Id</param>
        /// <returns></returns>
        public Session Get(long id)
        {
            Session session;
            this.sessions.TryGetValue(id, out session);
            return session;
        }

        /// <summary>
        /// 创建一个新Session
        /// </summary>
        public Session Create(IPEndPoint ipEndPoint)
        {
            AChannel channel = this.Service.ConnectChannel(ipEndPoint);
            Session session = ComponentFactory.CreateWithParent<Session, AChannel>(this, channel);
            this.sessions.Add(session.Id, session);
            session.Start();
            return session;
        }

        /// <summary>
        /// 创建一个新Session
        /// </summary>
        public Session Create(string address)
        {
            AChannel channel = this.Service.ConnectChannel(address);
            Session session = ComponentFactory.CreateWithParent<Session, AChannel>(this, channel);
            this.sessions.Add(session.Id, session);
            session.Start();
            return session;
        }

        /// <summary>
        /// 轮训更新处理
        /// </summary>
        public void Update()
        {
            if (this.Service == null)
            {
                return;
            }

            this.Service.Update();
        }

        /// <summary>
        /// 资源释放s
        /// </summary>
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            foreach (Session session in this.sessions.Values.ToArray())
            {
                session.Dispose();
            }

            this.Service.Dispose();
        }
    }
}