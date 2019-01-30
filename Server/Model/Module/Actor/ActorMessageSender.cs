using System.Net;

namespace ETModel
{
    /// <summary>
    /// 知道对方的instanceId，使用这个类发actor消息
    /// </summary>
    public struct ActorMessageSender
    {
        /// <summary>
        /// actor的地址
        /// </summary>
        public IPEndPoint Address { get; }

        /// <summary>
        /// ActorId
        /// </summary>
		public long ActorId { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="address"></param>
		public ActorMessageSender(long actorId, IPEndPoint address)
        {
            this.ActorId = actorId;
            this.Address = address;
        }
    }
}