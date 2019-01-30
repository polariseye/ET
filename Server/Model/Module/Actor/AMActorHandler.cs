using System;
using System.Threading.Tasks;

namespace ETModel
{
    /// <summary>
    /// Actor消息处理基类（异步消息处理）
    /// </summary>
    /// <typeparam name="E">Actor针对的实体类型</typeparam>
    /// <typeparam name="Message">不需要应答的消息</typeparam>
    public abstract class AMActorHandler<E, Message> : IMActorHandler where E : Entity where Message : class, IActorMessage
    {
        protected abstract void Run(E entity, Message message);

        /// <summary>
        /// 具体处理
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entity"></param>
        /// <param name="actorMessage"></param>
        /// <returns></returns>
        public async Task Handle(Session session, Entity entity, object actorMessage)
        {
            Message msg = actorMessage as Message;
            if (msg == null)
            {
                Log.Error($"消息类型转换错误: {actorMessage.GetType().FullName} to {typeof(Message).Name}");
                return;
            }
            E e = entity as E;
            if (e == null)
            {
                Log.Error($"Actor类型转换错误: {entity.GetType().Name} to {typeof(E).Name}");
                return;
            }

            this.Run(e, msg);

            await Task.CompletedTask;
        }

        /// <summary>
        /// 获取消息针对的消息类型
        /// </summary>
        /// <returns></returns>
        public Type GetMessageType()
        {
            return typeof(Message);
        }
    }
}