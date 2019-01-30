namespace ETModel
{
    /// <summary>
    /// 消息接口
    /// </summary>
    public interface IMessage
    {
    }

    /// <summary>
    /// 需要应答的请求接口
    /// </summary>
    public interface IRequest : IMessage
    {
        int RpcId { get; set; }
    }

    /// <summary>
    /// 请求应答接口
    /// </summary>
    public interface IResponse : IMessage
    {
        /// <summary>
        /// 错误码
        /// </summary>
        int Error { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// 请求Id
        /// </summary>
        int RpcId { get; set; }
    }

    /// <summary>
    /// 应答消息对象
    /// </summary>
    public class ResponseMessage : IResponse
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int Error { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 请求Id
        /// </summary>
        public int RpcId { get; set; }
    }
}