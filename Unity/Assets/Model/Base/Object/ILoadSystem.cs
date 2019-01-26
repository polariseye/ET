using System;

namespace ETModel
{
    /// <summary>
    /// 模块加载时会调用的接口(只有热更新时才会被调用)
    /// </summary>
	public interface ILoadSystem
    {
        /// <summary>
        /// 自身类型
        /// </summary>
        /// <returns></returns>
        Type Type();

        /// <summary>
        /// 加载初始化
        /// </summary>
        /// <param name="o"></param>
        void Run(object o);
    }

    /// <summary>
    /// 加载系统(只有热更新时才会被调用)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class LoadSystem<T> : ILoadSystem
    {
        /// <summary>
        /// 加载时调用的函数
        /// </summary>
        /// <param name="o"></param>
        public void Run(object o)
        {
            this.Load((T)o);
        }

        /// <summary>
        /// 自身类型
        /// </summary>
        /// <returns></returns>
        public Type Type()
        {
            return typeof(T);
        }

        /// <summary>
        /// 加载具体调用函数
        /// </summary>
        /// <param name="self"></param>
        public abstract void Load(T self);
    }
}
