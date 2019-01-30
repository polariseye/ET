using System.Collections.Generic;
using System.Net;

namespace ETModel
{
    /// <summary>
    /// 内网连接组件
    /// 用于内部系统进程之间通信
    /// </summary>
    public class NetInnerComponent : NetworkComponent
    {
        /// <summary>
        /// 记录地址到会话的映射
        /// </summary>
		public readonly Dictionary<IPEndPoint, Session> adressSessions = new Dictionary<IPEndPoint, Session>();

        /// <summary>
        /// 移除会话
        /// </summary>
        /// <param name="id">会话Id</param>
        public override void Remove(long id)
        {
            Session session = this.Get(id);
            if (session == null)
            {
                return;
            }
            this.adressSessions.Remove(session.RemoteAddress);

            base.Remove(id);
        }

        /// <summary>
        /// 从地址缓存中取Session,如果没有则创建一个新的Session,并且保存到地址缓存中
        /// </summary>
        public Session Get(IPEndPoint ipEndPoint)
        {
            if (this.adressSessions.TryGetValue(ipEndPoint, out Session session))
            {
                return session;
            }

            session = this.Create(ipEndPoint);

            this.adressSessions.Add(ipEndPoint, session);
            return session;
        }
    }
}