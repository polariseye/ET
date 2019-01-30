namespace ETModel
{
    /// <summary>
    /// 消息分发处理接口
    /// </summary>
	public interface IMessageDispatcher
    {
        /// <summary>
        /// 消息分发处理接口
        /// </summary>
        /// <param name="session">会话对象</param>
        /// <param name="opcode">消息类型编码</param>
        /// <param name="message">消息对象</param>
        void Dispatch(Session session, ushort opcode, object message);
    }
}
