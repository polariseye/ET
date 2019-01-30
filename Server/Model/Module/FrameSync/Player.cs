namespace ETModel
{
    /// <summary>
    /// 玩家加载
    /// </summary>
    [ObjectSystem]
    public class PlayerSystem : AwakeSystem<Player, string>
    {
        public override void Awake(Player self, string a)
        {
            self.Awake(a);
        }
    }

    /// <summary>
    /// 玩家类
    /// </summary>
    public sealed class Player : Entity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string Account { get; private set; }

        /// <summary>
        /// 所在节点Id
        /// </summary>
        public long UnitId { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="account"></param>
        public void Awake(string account)
        {
            this.Account = account;
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
        }
    }
}