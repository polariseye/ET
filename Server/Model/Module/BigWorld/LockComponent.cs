using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ETModel
{
    /// <summary>
    /// 锁状态
    /// </summary>
    public enum LockStatus
    {
        /// <summary>
        /// 未锁
        /// </summary>
        LockedNot,

        /// <summary>
        /// 请求中
        /// </summary>
        LockRequesting,

        /// <summary>
        /// 已锁住
        /// </summary>
        Locked,
    }

    /// <summary>
    /// 分布式锁组件,Unit对象可能在不同进程上有镜像,访问该对象的时候需要对他加锁
    /// </summary>
    public class LockComponent : Component
    {
        public LockStatus status = LockStatus.LockedNot;
        public IPEndPoint address;
        public int lockCount;
        public readonly Queue<TaskCompletionSource<bool>> queue = new Queue<TaskCompletionSource<bool>>();
    }
}