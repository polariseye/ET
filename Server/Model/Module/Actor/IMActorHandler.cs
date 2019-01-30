using System;
using System.Threading.Tasks;

namespace ETModel
{
    /// <summary>
    /// Actor消息处理接口
    /// </summary>
    public interface IMActorHandler
    {
        /// <summary>
        /// 消息处理接口
        /// </summary>
        /// <param name="session">会话对象</param>
        /// <param name="entity">实体对象</param>
        /// <param name="actorMessage">actor消息对象</param>
        /// <returns></returns>
        Task Handle(Session session, Entity entity, object actorMessage);

        /// <summary>
        /// 具体的消息类型
        /// </summary>
        /// <returns></returns>
        Type GetMessageType();
    }
}