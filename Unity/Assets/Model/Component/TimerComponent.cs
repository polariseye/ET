using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ETModel
{
    public struct Timer
    {
        public long Id { get; set; }
        public long Time { get; set; }
        public TaskCompletionSource<bool> tcs;
    }

    [ObjectSystem]
    public class TimerComponentUpdateSystem : UpdateSystem<TimerComponent>
    {
        public override void Update(TimerComponent self)
        {
            self.Update();
        }
    }

    /// <summary>
    /// 定时等待组件
    /// </summary>
	public class TimerComponent : Component
    {
        /// <summary>
        /// 所有需要等待的定时器
        /// </summary>
        private readonly Dictionary<long, Timer> timers = new Dictionary<long, Timer>();

        /// <summary>
        /// key: time, value: timer id
        /// </summary>
        private readonly MultiMap<long, long> timeId = new MultiMap<long, long>();

        /// <summary>
        /// 已达成的定时时间项
        /// </summary>
        private readonly Queue<long> timeOutTime = new Queue<long>();

        /// <summary>
        /// 等待触发完成的定时任务项
        /// </summary>
        private readonly Queue<long> timeOutTimerIds = new Queue<long>();

        /// <summary>
        /// 记录最小时间，不用每次都去MultiMap取第一个值
        /// </summary>
        private long minTime;

        public void Update()
        {
            if (this.timeId.Count == 0)
            {
                return;
            }

            long timeNow = TimeHelper.Now();

            if (timeNow < this.minTime)
            {
                return;
            }

            // 找到所有超时的项，加入到已超时队列中
            foreach (KeyValuePair<long, List<long>> kv in this.timeId.GetDictionary())
            {
                long k = kv.Key;
                if (k > timeNow)
                {
                    minTime = k;
                    break;
                }
                this.timeOutTime.Enqueue(k);
            }

            // 把超时队列的时间转换为对应的定时调度Id
            while (this.timeOutTime.Count > 0)
            {
                long time = this.timeOutTime.Dequeue();
                foreach (long timerId in this.timeId[time])
                {
                    this.timeOutTimerIds.Enqueue(timerId);
                }
                this.timeId.Remove(time);
            }

            // 触发定时任务完成事件
            while (this.timeOutTimerIds.Count > 0)
            {
                long timerId = this.timeOutTimerIds.Dequeue();
                Timer timer;
                if (!this.timers.TryGetValue(timerId, out timer))
                {
                    continue;
                }
                this.timers.Remove(timerId);
                timer.tcs.SetResult(true);
            }
        }

        /// <summary>
        /// 删除一个定时调度项
        /// </summary>
        /// <param name="id">定时调度Id</param>
        private void Remove(long id)
        {
            Timer timer;
            if (!this.timers.TryGetValue(id, out timer))
            {
                return;
            }
            this.timers.Remove(id);
        }

        /// <summary>
        /// 等待一段时间
        /// </summary>
        /// <param name="time">等待时长</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task WaitTillAsync(long tillTime, CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = tillTime, tcs = tcs };
            this.timers[timer.Id] = timer;
            this.timeId.Add(timer.Time, timer.Id);
            if (timer.Time < this.minTime)
            {
                this.minTime = timer.Time;
            }
            cancellationToken.Register(() => { this.Remove(timer.Id); });
            return tcs.Task;
        }

        /// <summary>
        /// 等待一段时间
        /// </summary>
        /// <param name="time">等待时长</param>
        /// <returns></returns>
        public Task WaitTillAsync(long tillTime)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = tillTime, tcs = tcs };
            this.timers[timer.Id] = timer;
            this.timeId.Add(timer.Time, timer.Id);
            if (timer.Time < this.minTime)
            {
                this.minTime = timer.Time;
            }
            return tcs.Task;
        }

        /// <summary>
        /// 等待一段时间
        /// </summary>
        /// <param name="time">等待时长</param>
        /// <returns></returns>
        public Task WaitAsync(long time, CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = TimeHelper.Now() + time, tcs = tcs };
            this.timers[timer.Id] = timer;
            this.timeId.Add(timer.Time, timer.Id);
            if (timer.Time < this.minTime)
            {
                this.minTime = timer.Time;
            }
            cancellationToken.Register(() => { this.Remove(timer.Id); });
            return tcs.Task;
        }

        /// <summary>
        /// 等待一段时间
        /// </summary>
        /// <param name="time">等待时长</param>
        /// <returns></returns>
        public Task WaitAsync(long time)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = TimeHelper.Now() + time, tcs = tcs };
            this.timers[timer.Id] = timer;
            this.timeId.Add(timer.Time, timer.Id);
            if (timer.Time < this.minTime)
            {
                this.minTime = timer.Time;
            }
            return tcs.Task;
        }
    }
}