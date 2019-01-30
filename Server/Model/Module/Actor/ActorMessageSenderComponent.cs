using System;
using System.Net;

namespace ETModel
{
    /// <summary>
    /// Actor消息远端发送组件
    /// </summary>
	public class ActorMessageSenderComponent : Component
    {
        /// <summary>
        /// 获取远端发送组件
        /// </summary>
        /// <param name="actorId">ActorId</param>
        /// <returns></returns>
        public ActorMessageSender Get(long actorId)
        {
            if (actorId == 0)
            {
                throw new Exception($"actor id is 0");
            }

            // 获取一个发送对象
            IPEndPoint ipEndPoint = StartConfigComponent.Instance.GetInnerAddress(IdGenerater.GetAppIdFromId(actorId));
            ActorMessageSender actorMessageSender = new ActorMessageSender(actorId, ipEndPoint);
            return actorMessageSender;
        }
    }
}
