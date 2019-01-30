using System;

namespace ETModel
{
    /// <summary>
    /// Actor消息处理自定义特性
    /// </summary>
	public class ActorMessageHandlerAttribute : BaseAttribute
    {
        /// <summary>
        /// 针对的App类型
        /// </summary>
        public AppType Type { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appType"></param>
        public ActorMessageHandlerAttribute(AppType appType)
        {
            this.Type = appType;
        }
    }
}