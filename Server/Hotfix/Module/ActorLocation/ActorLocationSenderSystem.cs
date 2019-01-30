using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 加载数据发送组件
    /// </summary>
    [ObjectSystem]
    public class ActorLocationSenderAwakeSystem : AwakeSystem<ActorLocationSender, long>
    {
        /// <summary>
        /// 加载处理
        /// </summary>
        /// <param name="self"></param>
        /// <param name="id">玩家Id</param>
        public override void Awake(ActorLocationSender self, long id)
        {
            self.LastSendTime = TimeHelper.Now();
            self.Id = id;
            self.Tcs = null;
            self.FailTimes = 0;
            self.ActorId = 0;
        }
    }

    /// <summary>
    /// 初始化数据发送组件
    /// </summary>
    [ObjectSystem]
    public class ActorLocationSenderStartSystem : StartSystem<ActorLocationSender>
    {
        public override async void Start(ActorLocationSender self)
        {
            self.ActorId = await Game.Scene.GetComponent<LocationProxyComponent>().Get(self.Id); //// 此处使用的玩家Id作为 组件Id 所以可以通过这个组件获取地址

            // 实例Id转具体通信地址
            self.Address = StartConfigComponent.Instance
                    .Get(IdGenerater.GetAppIdFromId(self.ActorId))
                    .GetComponent<InnerConfig>().IPEndPoint;

            self.UpdateAsync();
        }
    }

    /// <summary>
    /// 对象销毁处理
    /// </summary>
    [ObjectSystem]
    public class ActorLocationSenderDestroySystem : DestroySystem<ActorLocationSender>
    {
        public override void Destroy(ActorLocationSender self)
        {
            self.RunError(ErrorCode.ERR_ActorRemove);

            self.Id = 0;
            self.LastSendTime = 0;
            self.Address = null;
            self.ActorId = 0;
            self.FailTimes = 0;
            self.Tcs = null;
        }
    }

    /// <summary>
    /// Actor具体的处理逻辑
    /// </summary>
    public static class ActorLocationSenderHelper
    {
        /// <summary>
        /// 添加一个发送请求
        /// </summary>
        /// <param name="self"></param>
        /// <param name="task"></param>
        private static void Add(this ActorLocationSender self, ActorTask task)
        {
            if (self.IsDisposed)
            {
                throw new Exception("ActorLocationSender Disposed! dont hold ActorMessageSender");
            }

            self.WaitingTasks.Enqueue(task);
            // failtimes > 0表示正在重试，这时候不能加到正在发送队列
            if (self.FailTimes == 0)
            {
                self.AllowGet();
            }
        }

        /// <summary>
        /// 通知所有actor任务，执行失败
        /// </summary>
        /// <param name="self"></param>
        /// <param name="errorCode">错误码</param>
        public static void RunError(this ActorLocationSender self, int errorCode)
        {
            while (self.WaitingTasks.Count > 0)
            {
                ActorTask actorTask = self.WaitingTasks.Dequeue();
                actorTask.Tcs?.SetException(new RpcException(errorCode, ""));
            }
            self.WaitingTasks.Clear();
        }

        /// <summary>
        /// 设置任务已完成
        /// </summary>
        /// <param name="self"></param>
        private static void AllowGet(this ActorLocationSender self)
        {
            if (self.Tcs == null || self.WaitingTasks.Count <= 0)
            {
                return;
            }

            ActorTask task = self.WaitingTasks.Peek();

            var t = self.Tcs;
            self.Tcs = null;
            t.SetResult(task); //// 设置任务已完成
        }

        /// <summary>
        /// 获取一个actor的任务项
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        private static Task<ActorTask> GetAsync(this ActorLocationSender self)
        {
            if (self.WaitingTasks.Count > 0)
            {
                ActorTask task = self.WaitingTasks.Peek();
                return Task.FromResult(task);
            }

            self.Tcs = new TaskCompletionSource<ActorTask>(); //// 只是创建一个可等待的不需要执行的 Task
            return self.Tcs.Task;
        }

        /// <summary>
        /// 循环执行所有Task
        /// </summary>
        /// <param name="self"></param>
        public static async void UpdateAsync(this ActorLocationSender self)
        {
            try
            {
                long instanceId = self.InstanceId;
                while (true)
                {
                    if (self.InstanceId != instanceId)
                    {
                        return;
                    }
                    ActorTask actorTask = await self.GetAsync();

                    if (self.InstanceId != instanceId)
                    {
                        return;
                    }
                    if (actorTask.ActorRequest == null)
                    {
                        return;
                    }

                    await self.RunTask(actorTask);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        /// <summary>
        /// 执行一个任务
        /// </summary>
        /// <param name="self">数据发送上层对象</param>
        /// <param name="task"></param>
        /// <returns></returns>
        private static async Task RunTask(this ActorLocationSender self, ActorTask task)
        {
            ActorMessageSender actorMessageSender = Game.Scene.GetComponent<ActorMessageSenderComponent>().Get(self.ActorId);
            IActorResponse response = await actorMessageSender.Call(task.ActorRequest);

            // 发送成功
            switch (response.Error)
            {
                case ErrorCode.ERR_NotFoundActor:
                    // 如果没找到Actor,重试
                    ++self.FailTimes;

                    // 失败MaxFailTimes次则清空actor发送队列，返回失败
                    if (self.FailTimes > ActorLocationSender.MaxFailTimes)
                    {
                        // 失败直接删除actorproxy
                        Log.Info($"actor send message fail, actorid: {self.Id}");
                        self.RunError(response.Error);
                        self.GetParent<ActorLocationSenderComponent>().Remove(self.Id);
                        return;
                    }

                    // 等待0.5s再发送
                    await Game.Scene.GetComponent<TimerComponent>().WaitAsync(500);
                    self.ActorId = await Game.Scene.GetComponent<LocationProxyComponent>().Get(self.Id);
                    self.Address = StartConfigComponent.Instance
                            .Get(IdGenerater.GetAppIdFromId(self.ActorId))
                            .GetComponent<InnerConfig>().IPEndPoint;
                    self.AllowGet(); //// 目的是防止任务队列阻塞，所以此处主动通知还有任务可以处理
                    return;

                case ErrorCode.ERR_ActorNoMailBoxComponent:
                    self.RunError(response.Error);
                    self.GetParent<ActorLocationSenderComponent>().Remove(self.Id);
                    return;

                default:
                    self.LastSendTime = TimeHelper.Now();
                    self.FailTimes = 0;
                    self.WaitingTasks.Dequeue();

                    if (task.Tcs == null)
                    {
                        return;
                    }

                    IActorLocationResponse actorLocationResponse = response as IActorLocationResponse;
                    if (actorLocationResponse == null)
                    {
                        task.Tcs.SetException(new Exception($"actor location respose is not IActorLocationResponse, but is: {response.GetType().Name}"));
                    }
                    task.Tcs.SetResult(actorLocationResponse);
                    return;
            }
        }

        /// <summary>
        /// 异步发送一个请求
        /// </summary>
        /// <param name="self"></param>
        /// <param name="request"></param>
        public static void Send(this ActorLocationSender self, IActorLocationMessage request)
        {
            if (request == null)
            {
                throw new Exception($"actor location send message is null");
            }
            ActorTask task = new ActorTask(request);
            self.Add(task);
        }

        /// <summary>
        /// 同步发送一个请求
        /// </summary>
        /// <param name="self"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Task<IActorLocationResponse> Call(this ActorLocationSender self, IActorLocationRequest request)
        {
            if (request == null)
            {
                throw new Exception($"actor location call message is null");
            }
            TaskCompletionSource<IActorLocationResponse> tcs = new TaskCompletionSource<IActorLocationResponse>();
            ActorTask task = new ActorTask(request, tcs);
            self.Add(task);
            return task.Tcs.Task;
        }
    }
}