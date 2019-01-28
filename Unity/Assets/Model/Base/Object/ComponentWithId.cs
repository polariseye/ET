using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    /// <summary>
    /// 组件基类
    /// </summary>
	[BsonIgnoreExtraElements]
    public abstract class ComponentWithId : Component
    {
        /// <summary>
        /// 组件Id
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0L)]
        [BsonElement]
        [BsonId]
        public long Id { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        protected ComponentWithId()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        protected ComponentWithId(long id)
        {
            this.Id = id;
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
        }
    }
}