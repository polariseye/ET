using System;

namespace ETModel
{
    /// <summary>
    /// 组件初始化事件
    /// 模块一加入到EventSystem后，就会随后被调用
    /// 这个是在IAwakeSystem之后调用
    /// </summary>
    public interface IStartSystem
    {
        /// <summary>
        /// 事件针对的类型
        /// </summary>
        /// <returns></returns>
        Type Type();

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="o"></param>
        void Run(object o);
    }

    /// <summary>
    /// 组件初始化基类
    /// </summary>
    /// <typeparam name="T">针对的类型</typeparam>
    public abstract class StartSystem<T> : IStartSystem
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="o"></param>
        public void Run(object o)
        {
            this.Start((T)o);
        }

        /// <summary>
        /// 事件针对的类型
        /// </summary>
        /// <returns></returns>
        public Type Type()
        {
            return typeof(T);
        }

        /// <summary>
        /// 开始处理
        /// </summary>
        /// <param name="self"></param>
        public abstract void Start(T self);
    }
}
