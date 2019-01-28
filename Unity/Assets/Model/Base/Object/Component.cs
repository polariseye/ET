using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    /// <summary>
    /// 组件基类
    /// </summary>
	[BsonIgnoreExtraElements]
    public abstract class Component : Object, IDisposable, IComponentSerialize
    {
        /// <summary>
        /// 实例Id
        /// 只有Game.EventSystem.Add方法中会设置该值，如果new出来的对象不想加入Game.EventSystem中，则需要自己在构造函数中设置
        /// </summary>
        [BsonIgnore]
        public long InstanceId { get; private set; }

        [BsonIgnore]
        private bool isFromPool;

        /// <summary>
        /// 是否是从应用程序池取的
        /// 如果是从应用程序池取的，则会加入到事件处理中
        /// </summary>
        [BsonIgnore]
        public bool IsFromPool
        {
            get
            {
                return this.isFromPool;
            }
            set
            {
                this.isFromPool = value;

                if (!this.isFromPool)
                {
                    return;
                }

                this.InstanceId = IdGenerater.GenerateId();
                Game.EventSystem.Add(this);
            }
        }

        /// <summary>
        /// 对象是否已释放
        /// </summary>
        [BsonIgnore]
        public bool IsDisposed
        {
            get
            {
                return this.InstanceId == 0;
            }
        }

        /// <summary>
        /// 父组件对象
        /// </summary>
        [BsonIgnore]
        public Component Parent { get; set; }

        /// <summary>
        /// 获取父组件对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetParent<T>() where T : Component
        {
            return this.Parent as T;
        }

        /// <summary>
        /// 获取所在实体
        /// </summary>
        [BsonIgnore]
        public Entity Entity
        {
            get
            {
                return this.Parent as Entity;
            }
        }

        protected Component()
        {
            this.InstanceId = IdGenerater.GenerateId();
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public virtual void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            // 触发Destroy事件
            Game.EventSystem.Destroy(this);

            // 从Id到实体映射的记录表中移除此项
            Game.EventSystem.Remove(this.InstanceId);

            this.InstanceId = 0;

            // 如果是从对象池中获取的，则加入到对象池中
            if (this.IsFromPool)
            {
                Game.ObjectPool.Recycle(this);
            }
        }

        /// <summary>
        /// 开始序列化
        /// </summary>
        public virtual void BeginSerialize()
        {
        }

        /// <summary>
        /// 结束序列化
        /// </summary>
        public virtual void EndDeSerialize()
        {
        }
    }
}