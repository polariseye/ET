namespace ETModel
{
    /// <summary>
    /// actor RPC消息响应
    /// </summary>
    [Message(Opcode.ActorResponse)]
    public class ActorResponse : IActorLocationResponse
    {
        /// <summary>
        /// 此次RPC的请求Id
        /// </summary>
		public int RpcId { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
		public int Error { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
		public string Message { get; set; }
    }
}
