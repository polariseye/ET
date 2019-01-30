using System.Threading.Tasks;

namespace ETModel
{
    /// <summary>
    /// Actor拦截处理实现接口
    /// </summary>
    public interface IActorInterceptTypeHandler
    {
        /// <summary>
        /// Actor消息处理
        /// </summary>
        /// <param name="session">会话对象</param>
        /// <param name="entity">针对实体</param>
        /// <param name="actorMessage">消息实体</param>
        /// <returns></returns>
        Task Handle(Session session, Entity entity, object actorMessage);
    }
}