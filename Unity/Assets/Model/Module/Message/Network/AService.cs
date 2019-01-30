using System;
using System.Net;

namespace ETModel
{
    /// <summary>
    /// 网络通信抽象
    /// </summary>
	public enum NetworkProtocol
    {
        /// <summary>
        /// KCP
        /// </summary>
        KCP,
        /// <summary>
        /// TCP
        /// </summary>
        TCP,
        /// <summary>
        /// WebSocket
        /// </summary>
        WebSocket,
    }

    /// <summary>
    /// 网络通信抽象组件基类
    /// </summary>
    public abstract class AService : Component
    {
        /// <summary>
        /// 通过连接Id获取连接对象
        /// </summary>
        /// <param name="id">连接Id</param>
        /// <returns></returns>
        public abstract AChannel GetChannel(long id);

        /// <summary>
        /// 连接处理回调
        /// </summary>
        private Action<AChannel> acceptCallback;

        /// <summary>
        /// 连接处理回调
        /// </summary>
        public event Action<AChannel> AcceptCallback
        {
            add
            {
                this.acceptCallback += value;
            }
            remove
            {
                this.acceptCallback -= value;
            }
        }

        /// <summary>
        /// 收到连接时的处理
        /// </summary>
        /// <param name="channel"></param>
        protected void OnAccept(AChannel channel)
        {
            this.acceptCallback.Invoke(channel);
        }

        /// <summary>
        /// 连接到
        /// </summary>
        /// <param name="ipEndPoint">连接地址</param>
        /// <returns></returns>
        public abstract AChannel ConnectChannel(IPEndPoint ipEndPoint);

        /// <summary>
        /// 连接到
        /// </summary>
        /// <param name="address">连接地址</param>
        /// <returns></returns>
        public abstract AChannel ConnectChannel(string address);

        /// <summary>
        /// 删除一个连接
        /// </summary>
        /// <param name="channelId">连接Id</param>
        public abstract void Remove(long channelId);

        /// <summary>
        /// 轮训处理
        /// </summary>
        public abstract void Update();
    }
}