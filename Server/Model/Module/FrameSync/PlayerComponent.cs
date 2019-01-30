using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    /// <summary>
    /// 初始化
    /// </summary>
    [ObjectSystem]
    public class PlayerComponentSystem : AwakeSystem<PlayerComponent>
    {
        public override void Awake(PlayerComponent self)
        {
            self.Awake();
        }
    }

    /// <summary>
    /// 玩家管理组件
    /// </summary>
    public class PlayerComponent : Component
    {
        /// <summary>
        /// 组件实例
        /// </summary>
        public static PlayerComponent Instance { get; private set; }

        public Player MyPlayer;

        /// <summary>
        /// 玩家Id到玩家对象的映射
        /// </summary>
        private readonly Dictionary<long, Player> idPlayers = new Dictionary<long, Player>();

        /// <summary>
        /// 初始化
        /// </summary>
        public void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 添加一个玩家
        /// </summary>
        /// <param name="player"></param>
        public void Add(Player player)
        {
            this.idPlayers.Add(player.Id, player);
        }

        /// <summary>
        /// 通过玩家Id获取玩家对象
        /// </summary>
        /// <param name="id">玩家Id</param>
        /// <returns></returns>
        public Player Get(long id)
        {
            this.idPlayers.TryGetValue(id, out Player gamer);
            return gamer;
        }

        /// <summary>
        /// 删除一个玩家
        /// </summary>
        /// <param name="id">玩家Id</param>
        public void Remove(long id)
        {
            this.idPlayers.Remove(id);
        }

        /// <summary>
        /// 当前玩家数量
        /// </summary>
        public int Count
        {
            get
            {
                return this.idPlayers.Count;
            }
        }

        /// <summary>
        /// 获取所有玩家
        /// </summary>
        /// <returns></returns>
        public Player[] GetAll()
        {
            return this.idPlayers.Values.ToArray();
        }

        /// <summary>
        /// 资源销毁
        /// </summary>
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            foreach (Player player in this.idPlayers.Values)
            {
                player.Dispose();
            }

            Instance = null;
        }
    }
}