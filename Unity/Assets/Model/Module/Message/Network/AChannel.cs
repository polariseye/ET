using System;
using System.IO;
using System.Net;

namespace ETModel
{
    /// <summary>
    /// 连接类型
    /// </summary>
	public enum ChannelType
    {
        /// <summary>
        /// 主动创建的连接
        /// </summary>
        Connect,

        /// <summary>
        /// 收到的远端连接
        /// </summary>
        Accept,
    }

    /// <summary>
    /// 连接处理类
    /// </summary>
    public abstract class AChannel : ComponentWithId
    {
        /// <summary>
        /// 连接类型
        /// </summary>
        public ChannelType ChannelType { get; }

        /// <summary>
        /// 连接对应的服务对象
        /// </summary>
        private AService service;

        /// <summary>
        /// 服务对象
        /// </summary>
        public AService Service
        {
            get
            {
                return this.service;
            }
        }

        /// <summary>
        /// 流对象
        /// </summary>
        public abstract MemoryStream Stream { get; }

        /// <summary>
        /// 错误码
        /// </summary>
        public int Error { get; set; }

        /// <summary>
        /// 远端地址
        /// </summary>
        public IPEndPoint RemoteAddress { get; protected set; }

        /// <summary>
        /// 错误处理回调
        /// </summary>
        private Action<AChannel, int> errorCallback;

        /// <summary>
        /// 设置错误处理回调
        /// </summary>
        public event Action<AChannel, int> ErrorCallback
        {
            add
            {
                this.errorCallback += value;
            }
            remove
            {
                this.errorCallback -= value;
            }
        }

        /// <summary>
        /// 数据读取回调
        /// </summary>
        private Action<MemoryStream> readCallback;

        /// <summary>
        /// 注册数据读取回调
        /// </summary>
        public event Action<MemoryStream> ReadCallback
        {
            add
            {
                this.readCallback += value;
            }
            remove
            {
                this.readCallback -= value;
            }
        }

        /// <summary>
        /// 收到数据时的处理
        /// </summary>
        /// <param name="memoryStream"></param>
        protected void OnRead(MemoryStream memoryStream)
        {
            this.readCallback.Invoke(memoryStream);
        }

        /// <summary>
        /// 出错时的处理
        /// </summary>
        /// <param name="e"></param>
        protected void OnError(int e)
        {
            this.Error = e;
            this.errorCallback?.Invoke(this, e);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service"></param>
        /// <param name="channelType"></param>
        protected AChannel(AService service, ChannelType channelType)
        {
            this.Id = IdGenerater.GenerateId();
            this.ChannelType = channelType;
            this.service = service;
        }

        /// <summary>
        /// 开始处理
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="stream"></param>
        public abstract void Send(MemoryStream stream);

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            this.service.Remove(this.Id);
        }
    }
}