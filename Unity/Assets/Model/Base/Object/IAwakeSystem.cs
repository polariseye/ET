using System;

namespace ETModel
{
    /// <summary>
    /// 组件或实体初始化事件接口
    /// （一般组件定义和事件定义是分成两个类实现的）
    /// </summary>
	public interface IAwakeSystem
    {
        /// <summary>
        /// 组件具体类型
        /// </summary>
        /// <returns></returns>
        Type Type();
    }

    /// <summary>
    /// 组件或实体初始化接口
    /// </summary>
    public interface IAwake
    {
        void Run(object o);
    }

    /// <summary>
    /// 组件或实体初始化接口
    /// </summary>
    /// <typeparam name="A">运行参数1</typeparam>
    public interface IAwake<A>
    {
        void Run(object o, A a);
    }

    /// <summary>
    /// 组件或实体初始化接口
    /// </summary>
    /// <typeparam name="A">运行参数1</typeparam>
    /// <typeparam name="B">运行参数2</typeparam>
    public interface IAwake<A, B>
    {
        void Run(object o, A a, B b);
    }

    /// <summary>
    /// 组件或实体初始化接口
    /// </summary>
    /// <typeparam name="A">运行参数1</typeparam>
    /// <typeparam name="B">运行参数2</typeparam>
    /// <typeparam name="C">运行参数3</typeparam>
    public interface IAwake<A, B, C>
    {
        void Run(object o, A a, B b, C c);
    }

    /// <summary>
    /// 组件或实体初始化基类
    /// </summary>
    /// <typeparam name="T">对应实体具体类型</typeparam>
    public abstract class AwakeSystem<T> : IAwakeSystem, IAwake
    {
        public Type Type()
        {
            return typeof(T);
        }

        public void Run(object o)
        {
            this.Awake((T)o);
        }

        public abstract void Awake(T self);
    }

    /// <summary>
    /// 组件或实体初始化基类
    /// </summary>
    /// <typeparam name="T">对应组件具体类型</typeparam>
    /// <typeparam name="A"></typeparam>
    public abstract class AwakeSystem<T, A> : IAwakeSystem, IAwake<A>
    {
        /// <summary>
        /// 唤醒事件对应的具体组件类型
        /// </summary>
        /// <returns></returns>
        public Type Type()
        {
            return typeof(T);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="o">自身组件对象</param>
        /// <param name="a">组件参数</param>
        public void Run(object o, A a)
        {
            this.Awake((T)o, a);
        }

        /// <summary>
        /// 子类处理函数
        /// </summary>
        /// <param name="self">自身组件对象</param>
        /// <param name="a">组件参数</param>
        public abstract void Awake(T self, A a);
    }

    /// <summary>
    /// 组件或实体初始化基类
    /// </summary>
    /// <typeparam name="T">对应组件具体类型</typeparam>
    /// <typeparam name="A">组件参数1</typeparam>
    /// <typeparam name="B">组件参数2</typeparam>
    public abstract class AwakeSystem<T, A, B> : IAwakeSystem, IAwake<A, B>
    {
        public Type Type()
        {
            return typeof(T);
        }

        public void Run(object o, A a, B b)
        {
            this.Awake((T)o, a, b);
        }

        public abstract void Awake(T self, A a, B b);
    }

    /// <summary>
    /// 组件或实体初始化基类
    /// </summary>
    /// <typeparam name="T">对应组件具体类型</typeparam>
    /// <typeparam name="A">组件参数1</typeparam>
    /// <typeparam name="B">组件参数2</typeparam>
    /// <typeparam name="C">组件参数3</typeparam>
    public abstract class AwakeSystem<T, A, B, C> : IAwakeSystem, IAwake<A, B, C>
    {
        public Type Type()
        {
            return typeof(T);
        }

        public void Run(object o, A a, B b, C c)
        {
            this.Awake((T)o, a, b, c);
        }

        public abstract void Awake(T self, A a, B b, C c);
    }
}
