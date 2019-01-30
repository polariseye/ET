using System;
using System.Collections.Generic;

namespace ETModel
{
    /// <summary>
    /// Actor通过玩家Id发送数据的组件
    /// </summary>
    public class ActorLocationSenderComponent : Component
    {
        /// <summary>
        /// 玩家Id到数据发送的映射
        /// </summary>
        public readonly Dictionary<long, ActorLocationSender> ActorLocationSenders = new Dictionary<long, ActorLocationSender>();

        /// <summary>
        /// 资源释放
        /// </summary>
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            // 释放所有actor对象
            foreach (ActorLocationSender actorLocationSender in this.ActorLocationSenders.Values)
            {
                actorLocationSender.Dispose();
            }

            this.ActorLocationSenders.Clear();
        }

        /// <summary>
        /// 根据玩家Id获取一个actor访问对象
        /// 不存在则会创建一个
        /// </summary>
        /// <param name="id">玩家Id</param>
        /// <returns></returns>
        public ActorLocationSender Get(long id)
        {
            if (id == 0)
            {
                throw new Exception($"actor id is 0");
            }

            if (this.ActorLocationSenders.TryGetValue(id, out ActorLocationSender actorLocationSender))
            {
                return actorLocationSender;
            }

            actorLocationSender = ComponentFactory.CreateWithId<ActorLocationSender>(id);
            actorLocationSender.Parent = this;
            this.ActorLocationSenders[id] = actorLocationSender;
            return actorLocationSender;
        }

        /// <summary>
        /// 移除一个发送对象
        /// </summary>
        /// <param name="id"></param>
        public void Remove(long id)
        {
            if (!this.ActorLocationSenders.TryGetValue(id, out ActorLocationSender actorMessageSender))
            {
                return;
            }
            this.ActorLocationSenders.Remove(id);
            actorMessageSender.Dispose();
        }
    }
}
