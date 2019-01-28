using System;

namespace ETModel
{
    /// <summary>
    /// 消息处理接口
    /// </summary>
	public interface IMHandler
    {
        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="session"></param>
        /// <param name="message"></param>
        void Handle(Session session, object message);

        /// <summary>
        /// 消息类型
        /// </summary>
        /// <returns></returns>
        Type GetMessageType();
    }
}