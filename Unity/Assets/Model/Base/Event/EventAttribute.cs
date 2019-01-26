using System;

namespace ETModel
{
    /// <summary>
    /// 针对事件的自定义特性，打了此特性的接口必须实现接口<see cref="IEvent"/>
    /// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class EventAttribute: BaseAttribute
	{
		public string Type { get; }

		public EventAttribute(string type)
		{
			this.Type = type;
		}
	}
}