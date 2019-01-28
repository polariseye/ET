using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    /// <summary>
    /// 游戏内的个体类型
    /// </summary>
    public enum UnitType
    {
        /// <summary>
        /// 玩家
        /// </summary>
        Hero,

        /// <summary>
        /// NPC
        /// </summary>
        Npc
    }

    /// <summary>
    /// 加载事件
    /// </summary>
    [ObjectSystem]
    public class UnitSystem : AwakeSystem<Unit, UnitType>
    {
        public override void Awake(Unit self, UnitType a)
        {
            self.Awake(a);
        }
    }

    /// <summary>
    /// 游戏内的独立个体
    /// </summary>
	public sealed class Unit : Entity
    {
        /// <summary>
        /// 单位类型
        /// </summary>
        public UnitType UnitType { get; private set; }

        /// <summary>
        /// 所在位置
        /// </summary>
        [BsonIgnore]
        public Vector3 Position { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="unitType"></param>
        public void Awake(UnitType unitType)
        {
            this.UnitType = unitType;
        }

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