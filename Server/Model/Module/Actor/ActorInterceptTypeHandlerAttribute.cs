using System;

namespace ETModel
{
    /// <summary>
    /// Actor消息类型拦截处理自定义特性
    /// </summary>
    public class ActorInterceptTypeHandlerAttribute : BaseAttribute
    {
        /// <summary>
        /// 针对的App类型
        /// </summary>
        public AppType Type { get; }

        /// <summary>
        /// Actor类型的唯一标记
        /// </summary>
        public string ActorType { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appType">针对的App类型</param>
        /// <param name="actorType">Actor类型的唯一标记</param>
        public ActorInterceptTypeHandlerAttribute(AppType appType, string actorType)
        {
            this.Type = appType;
            this.ActorType = actorType;
        }
    }
}