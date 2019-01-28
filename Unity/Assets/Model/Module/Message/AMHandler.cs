using System;

namespace ETModel
{
    /// <summary>
    /// 普通消息处理基类
    /// 类似Socket这种，只管收不管发
    /// </summary>
    /// <typeparam name="Message">消息类型</typeparam>
	public abstract class AMHandler<Message> : IMHandler where Message : class
    {
        /// <summary>
        /// 消息处理的子类实现
        /// </summary>
        /// <param name="session"></param>
        /// <param name="message"></param>
		protected abstract void Run(Session session, Message message);

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="session"></param>
        /// <param name="msg"></param>
		public void Handle(Session session, object msg)
        {
            Message message = msg as Message;
            if (message == null)
            {
                Log.Error($"消息类型转换错误: {msg.GetType().Name} to {typeof(Message).Name}");
                return;
            }
            if (session.IsDisposed)
            {
                Log.Error($"session disconnect {msg}");
                return;
            }
            this.Run(session, message);
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        /// <returns></returns>
        public Type GetMessageType()
        {
            return typeof(Message);
        }
    }
}