using System;

namespace ETModel
{
    /// <summary>
    /// 实体事件自定义特性
    /// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EntityEventAttribute : Attribute
    {
        public int ClassType;

        public EntityEventAttribute(int classType)
        {
            this.ClassType = classType;
        }
    }
}