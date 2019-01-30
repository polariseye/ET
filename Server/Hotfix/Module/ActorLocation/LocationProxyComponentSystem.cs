using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class LocationProxyComponentSystem : AwakeSystem<LocationProxyComponent>
    {
        public override void Awake(LocationProxyComponent self)
        {
            self.Awake();
        }
    }

    /// <summary>
    /// 位置代理组件
    /// </summary>
    public static class LocationProxyComponentEx
    {
        /// <summary>
        /// 初始化处理
        /// </summary>
        /// <param name="self"></param>
        public static void Awake(this LocationProxyComponent self)
        {
            StartConfigComponent startConfigComponent = StartConfigComponent.Instance;

            StartConfig startConfig = startConfigComponent.LocationConfig;
            self.LocationAddress = startConfig.GetComponent<InnerConfig>().IPEndPoint;
        }

        /// <summary>
        /// 添加一个位置映射
        /// </summary>
        /// <param name="self">本身对象</param>
        /// <param name="key">对象的唯一Id</param>
        /// <param name="instanceId">对象在内存实体中的实例Id</param>
        /// <returns></returns>
        public static async Task Add(this LocationProxyComponent self, long key, long instanceId)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.LocationAddress);
            await session.Call(new ObjectAddRequest() { Key = key, InstanceId = instanceId });
        }

        /// <summary>
        /// 锁住一个Id
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <param name="instanceId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static async Task Lock(this LocationProxyComponent self, long key, long instanceId, int time = 1000)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.LocationAddress);
            await session.Call(new ObjectLockRequest() { Key = key, InstanceId = instanceId, Time = time });
        }

        /// <summary>
        /// 解锁（用于分布式锁的场景）
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <param name="oldInstanceId"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public static async Task UnLock(this LocationProxyComponent self, long key, long oldInstanceId, long instanceId)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.LocationAddress);
            await session.Call(new ObjectUnLockRequest() { Key = key, OldInstanceId = oldInstanceId, InstanceId = instanceId });
        }

        /// <summary>
        /// 移除一项
        /// 如果是移除的锁，则会触发对应的锁事件
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task Remove(this LocationProxyComponent self, long key)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.LocationAddress);
            await session.Call(new ObjectRemoveRequest() { Key = key });
        }

        /// <summary>
        /// 根据玩家Id获取对应在内存中的实例Id
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <returns></returns>
		public static async Task<long> Get(this LocationProxyComponent self, long key)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.LocationAddress);
            ObjectGetResponse response = (ObjectGetResponse)await session.Call(new ObjectGetRequest() { Key = key });
            return response.InstanceId;
        }
    }
}