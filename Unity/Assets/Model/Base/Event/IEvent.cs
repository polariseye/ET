using System;

namespace ETModel
{
    /// <summary>
    /// 用于事件实现的接口,实现了此接口的类需要加上自定义特性<see cref="EventAttribute"/>
    /// </summary>
	public interface IEvent
    {
        /// <summary>
        /// 不带参数的处理方式
        /// </summary>
        void Handle();

        /// <summary>
        /// 只有一个参数的处理方式
        /// </summary>
        /// <param name="a"></param>
        void Handle(object a);

        /// <summary>
        /// 带有两个参数的处理方式
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        void Handle(object a, object b);

        /// <summary>
        /// 带有三个参数的处理方式
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        void Handle(object a, object b, object c);
    }

    /// <summary>
    /// 事件基类
    /// 继承此类的类需要加上自定义特性<see cref="EventAttribute"/>
    /// </summary>
    public abstract class AEvent : IEvent
    {
        public void Handle()
        {
            this.Run();
        }

        public void Handle(object a)
        {
            throw new NotImplementedException();
        }

        public void Handle(object a, object b)
        {
            throw new NotImplementedException();
        }

        public void Handle(object a, object b, object c)
        {
            throw new NotImplementedException();
        }

        public abstract void Run();
    }

    /// <summary>
    /// 事件基类
    /// 继承此类的类需要加上自定义特性<see cref="EventAttribute"/>
    public abstract class AEvent<A> : IEvent
    {
        public void Handle()
        {
            throw new NotImplementedException();
        }

        public void Handle(object a)
        {
            this.Run((A)a);
        }

        public void Handle(object a, object b)
        {
            throw new NotImplementedException();
        }

        public void Handle(object a, object b, object c)
        {
            throw new NotImplementedException();
        }

        public abstract void Run(A a);
    }

    /// <summary>
    /// 事件基类
    /// 继承此类的类需要加上自定义特性<see cref="EventAttribute"/>
    public abstract class AEvent<A, B> : IEvent
    {
        public void Handle()
        {
            throw new NotImplementedException();
        }

        public void Handle(object a)
        {
            throw new NotImplementedException();
        }

        public void Handle(object a, object b)
        {
            this.Run((A)a, (B)b);
        }

        public void Handle(object a, object b, object c)
        {
            throw new NotImplementedException();
        }

        public abstract void Run(A a, B b);
    }

    /// <summary>
    /// 事件基类
    /// 继承此类的类需要加上自定义特性<see cref="EventAttribute"/>
    public abstract class AEvent<A, B, C> : IEvent
    {
        public void Handle()
        {
            throw new NotImplementedException();
        }

        public void Handle(object a)
        {
            throw new NotImplementedException();
        }

        public void Handle(object a, object b)
        {
            throw new NotImplementedException();
        }

        public void Handle(object a, object b, object c)
        {
            this.Run((A)a, (B)b, (C)c);
        }

        public abstract void Run(A a, B b, C c);
    }
}