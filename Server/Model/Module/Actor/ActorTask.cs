using System.Threading.Tasks;

namespace ETModel
{
    /// <summary>
    /// Actor发送任务
    /// </summary>
    public struct ActorTask
    {
        /// <summary>
        /// 请求对象
        /// </summary>
        public IActorRequest ActorRequest;

        /// <summary>
        /// 完成处理函数
        /// </summary>
        public TaskCompletionSource<IActorLocationResponse> Tcs;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="actorRequest"></param>
        public ActorTask(IActorLocationMessage actorRequest)
        {
            this.ActorRequest = actorRequest;
            this.Tcs = null;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="actorRequest">请求对象</param>
        /// <param name="tcs">完成回调函数</param>
        public ActorTask(IActorLocationRequest actorRequest, TaskCompletionSource<IActorLocationResponse> tcs)
        {
            this.ActorRequest = actorRequest;
            this.Tcs = tcs;
        }
    }
}