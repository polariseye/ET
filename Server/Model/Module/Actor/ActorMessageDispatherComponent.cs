using System;
using System.Collections.Generic;

namespace ETModel
{
    /// <summary>
    /// Actor消息分发组件
    /// </summary>
    public class ActorMessageDispatherComponent : Component
    {
        /// <summary>
        /// actor的字符串Id到类型的映射
        /// Key:Actor类型的唯一标记
        /// Value:处理实例
        /// </summary>
        public readonly Dictionary<string, IActorInterceptTypeHandler> ActorTypeHandlers = new Dictionary<string, IActorInterceptTypeHandler>();

        /// <summary>
        /// actor消息的具体处理类型的映射
        /// Key:消息类型
        /// Value:消息类型对应的处理函数
        /// </summary>
        public readonly Dictionary<Type, IMActorHandler> ActorMessageHandlers = new Dictionary<Type, IMActorHandler>();

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();

            this.ActorMessageHandlers.Clear();
            this.ActorTypeHandlers.Clear();
        }
    }
}