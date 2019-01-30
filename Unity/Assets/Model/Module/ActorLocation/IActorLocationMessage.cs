namespace ETModel
{
    /// <summary>
    /// actor位置请求消息接口(用于异步请求)
    /// </summary>
	public interface IActorLocationMessage : IActorRequest
    {
    }

    /// <summary>
    /// Actor位置请求接口 用于RPC请求
    /// </summary>
	public interface IActorLocationRequest : IActorRequest
    {
    }

    /// <summary>
    /// Actor位置请求应答接口 用于RPC请求的应答
    /// </summary>
    public interface IActorLocationResponse : IActorResponse
    {
    }
}