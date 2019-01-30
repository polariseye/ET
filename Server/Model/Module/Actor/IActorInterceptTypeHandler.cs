using System.Threading.Tasks;

namespace ETModel
{
    /// <summary>
    /// Actor���ش���ʵ�ֽӿ�
    /// </summary>
    public interface IActorInterceptTypeHandler
    {
        /// <summary>
        /// Actor��Ϣ����
        /// </summary>
        /// <param name="session">�Ự����</param>
        /// <param name="entity">���ʵ��</param>
        /// <param name="actorMessage">��Ϣʵ��</param>
        /// <returns></returns>
        Task Handle(Session session, Entity entity, object actorMessage);
    }
}