using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ETModel
{
    /// <summary>
    /// 知道对方的Id，使用这个类发actor消息
    /// </summary>
    public class ActorLocationSender : ComponentWithId
    {
        /// <summary>
        /// actor的地址
        /// </summary>
        public IPEndPoint Address;

        /// <summary>
        /// ActorId
        /// </summary>
        public long ActorId;

        /// <summary>
        /// 还没发送的消息
        /// </summary>
        public Queue<ActorTask> WaitingTasks = new Queue<ActorTask>();

        /// <summary>
        /// 最近发送消息的时间
        /// </summary>
        public long LastSendTime;

        /// <summary>
        /// 已失败次数
        /// </summary>
        public int FailTimes;

        /// <summary>
        /// 最大失败次数
        /// </summary>
        public const int MaxFailTimes = 5;

        /// <summary>
        /// 已完成的任务
        /// </summary>
        public TaskCompletionSource<ActorTask> Tcs;

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
        }
    }
}