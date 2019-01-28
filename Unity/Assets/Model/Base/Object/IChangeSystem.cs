using System;

namespace ETModel
{
    /// <summary>
    /// 变更事件处理接口
    /// </summary>
	public interface IChangeSystem
    {
        Type Type();
        void Run(object o);
    }

    /// <summary>
    /// 变更事件处理基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public abstract class ChangeSystem<T> : IChangeSystem
    {
        public void Run(object o)
        {
            this.Change((T)o);
        }

        public Type Type()
        {
            return typeof(T);
        }

        public abstract void Change(T self);
    }
}
