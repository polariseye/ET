using System;

namespace ETModel
{
    /// <summary>
    /// 消息处理自定义特性
    /// </summary>
	public class MessageHandlerAttribute : BaseAttribute
    {
        /// <summary>
        /// 消息针对的APP类型
        /// </summary>
        public AppType Type { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MessageHandlerAttribute()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appType">App类型</param>
        public MessageHandlerAttribute(AppType appType)
        {
            this.Type = appType;
        }
    }
}