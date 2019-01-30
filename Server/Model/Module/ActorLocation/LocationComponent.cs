using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    /// <summary>
    /// 位置相关处理基类
    /// </summary>
    public abstract class LocationTask : Component
    {
        public abstract void Run();
    }

    [ObjectSystem]
    public class LocationQueryTaskAwakeSystem : AwakeSystem<LocationQueryTask, long>
    {
        public override void Awake(LocationQueryTask self, long key)
        {
            self.Key = key;
            self.Tcs = new TaskCompletionSource<long>();
        }
    }

    /// <summary>
    /// 查询锁的处理类
    /// </summary>
    public sealed class LocationQueryTask : LocationTask
    {
        public long Key;

        public TaskCompletionSource<long> Tcs;

        public Task<long> Task
        {
            get
            {
                return this.Tcs.Task;
            }
        }

        public override void Run()
        {
            try
            {
                LocationComponent locationComponent = this.GetParent<LocationComponent>();
                long location = locationComponent.Get(this.Key);
                this.Tcs.SetResult(location);
            }
            catch (Exception e)
            {
                this.Tcs.SetException(e);
            }
        }
    }

    /// <summary>
    /// 位置代记录组件
    /// </summary>
	public class LocationComponent : Component
    {
        /// <summary>
        /// 位置映射数据
        /// Key:唯一Id（一般是玩家Id）
        /// Value:实体Id
        /// </summary>
        private readonly Dictionary<long, long> locations = new Dictionary<long, long>();

        /// <summary>
        /// 锁映射集合
        /// Key:唯一Id（一般是玩家Id）
        /// Value:实体Id
        /// </summary>
        private readonly Dictionary<long, long> lockDict = new Dictionary<long, long>();

        /// <summary>
        /// 解除锁时，需要触发的事件
        /// Key:唯一Id（一般是玩家Id）
        /// Value:等待的队列
        /// </summary>
        private readonly Dictionary<long, Queue<LocationTask>> taskQueues = new Dictionary<long, Queue<LocationTask>>();

        /// <summary>
        /// 添加一个位置映射
        /// </summary>
        /// <param name="key">唯一Id（一般是玩家Id）</param>
        /// <param name="instanceId">实例Id</param>
        public void Add(long key, long instanceId)
        {
            this.locations[key] = instanceId;

            Log.Info($"location add key: {key} instanceId: {instanceId}");

            // 更新db
            //await Game.Scene.GetComponent<DBProxyComponent>().Save(new Location(key, address));
        }

        /// <summary>
        /// 删除一个位置映射
        /// </summary>
        /// <param name="key">唯一Id（一般是玩家Id）</param>
        public void Remove(long key)
        {
            Log.Info($"location remove key: {key}");
            this.locations.Remove(key);
        }

        /// <summary>
        /// 获取一个位置映射
        /// </summary>
        /// <param name="key">唯一Id（一般是玩家Id）</param>
        /// <returns></returns>
        public long Get(long key)
        {
            this.locations.TryGetValue(key, out long instanceId);
            return instanceId;
        }

        /// <summary>
        /// 锁对象
        /// </summary>
        /// <param name="key">唯一Id（一般是玩家Id）</param>
        /// <param name="instanceId">实体Id</param>
        /// <param name="time">解除锁的时间</param>
        public async void Lock(long key, long instanceId, int time = 0)
        {
            if (this.lockDict.ContainsKey(key))
            {
                Log.Error($"不可能同时存在两次lock, key: {key} InstanceId: {instanceId}");
                return;
            }

            Log.Info($"location lock key: {key} InstanceId: {instanceId}");

            if (!this.locations.TryGetValue(key, out long saveInstanceId))
            {
                Log.Error($"actor没有注册, key: {key} InstanceId: {instanceId}");
                return;
            }

            if (saveInstanceId != instanceId)
            {
                Log.Error($"actor注册的instanceId与lock的不一致, key: {key} InstanceId: {instanceId} saveInstanceId: {saveInstanceId}");
                return;
            }

            this.lockDict.Add(key, instanceId);

            // 超时则解锁
            if (time > 0)
            {
                await Game.Scene.GetComponent<TimerComponent>().WaitAsync(time);

                if (!this.lockDict.ContainsKey(key))
                {
                    return;
                }
                Log.Info($"location timeout unlock key: {key} time: {time}");
                this.UnLock(key);
            }
        }

        /// <summary>
        /// 解除锁的使用
        /// </summary>
        /// <param name="key">唯一Id（一般是玩家Id）</param>
        /// <param name="oldInstanceId">之前的实体Id</param>
        /// <param name="instanceId">实体Id</param>
        public void UnLockAndUpdate(long key, long oldInstanceId, long instanceId)
        {
            this.lockDict.TryGetValue(key, out long lockInstanceId);
            if (lockInstanceId != oldInstanceId)
            {
                Log.Error($"unlock appid is different {lockInstanceId} {oldInstanceId}");
            }
            Log.Info($"location unlock key: {key} oldInstanceId: {oldInstanceId} new: {instanceId}");
            this.locations[key] = instanceId;
            this.UnLock(key);
        }

        /// <summary>
        /// 解除锁的使用,并触发对应解除锁的等待事件
        /// </summary>
        /// <param name="key">唯一Id（一般是玩家Id）</param>
        private void UnLock(long key)
        {
            this.lockDict.Remove(key);

            if (!this.taskQueues.TryGetValue(key, out Queue<LocationTask> tasks))
            {
                return;
            }

            while (true)
            {
                if (tasks.Count <= 0)
                {
                    this.taskQueues.Remove(key);
                    return;
                }
                if (this.lockDict.ContainsKey(key))
                {
                    return;
                }

                LocationTask task = tasks.Dequeue();
                try
                {
                    task.Run();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                task.Dispose();
            }
        }

        /// <summary>
        /// 根据唯一Id获取实体Id
        /// </summary>
        /// <param name="key">唯一Id（一般是玩家Id）</param>
        /// <returns></returns>
        public Task<long> GetAsync(long key)
        {
            if (!this.lockDict.ContainsKey(key))
            {
                this.locations.TryGetValue(key, out long instanceId);
                Log.Info($"location get key: {key} {instanceId}");
                return Task.FromResult(instanceId);
            }

            LocationQueryTask task = ComponentFactory.CreateWithParent<LocationQueryTask, long>(this, key);
            this.AddTask(key, task);
            return task.Task;
        }

        /// <summary>
        /// 添加一个位置查询任务
        /// </summary>
        /// <param name="key">唯一Id（一般是玩家Id）</param>
        /// <param name="task">需要添加的任务对象</param>
        public void AddTask(long key, LocationTask task)
        {
            if (!this.taskQueues.TryGetValue(key, out Queue<LocationTask> tasks))
            {
                tasks = new Queue<LocationTask>();
                this.taskQueues[key] = tasks;
            }
            tasks.Enqueue(task);
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();

            this.locations.Clear();
            this.lockDict.Clear();
            this.taskQueues.Clear();
        }
    }
}