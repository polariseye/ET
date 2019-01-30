namespace ETModel
{
    /// <summary>
    /// 不需要应答的Actor消息
    /// </summary>
    public interface IActorMessage : IMessage
    {
        /// <summary>
        /// Actor对象Id
        /// </summary>
        long ActorId { get; set; }
    }

    /// <summary>
    /// 需要应答的Actor消息
    /// </summary>
    public interface IActorRequest : IRequest
    {
        /// <summary>
        /// Actor对象Id
        /// </summary>
        long ActorId { get; set; }
    }

    /// <summary>
    /// 应答的Actor对象
    /// </summary>
    public interface IActorResponse : IResponse
    {
    }

    /// <summary>
    /// 帧消息接口
    /// </summary>
    public interface IFrameMessage : IMessage
    {
        long Id { get; set; }
    }
}