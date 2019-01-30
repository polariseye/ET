using System;

namespace ETModel
{
    /// <summary>
    /// 组件工厂类
    /// </summary>
	public static class ComponentFactory
    {
        /// <summary>
        /// 通过类型创建一个组件
        /// </summary>
        /// <param name="type">组件具体类型</param>
        /// <param name="parent">父组件</param>
        /// <returns></returns>
        public static Component CreateWithParent(Type type, Component parent)
        {
            Component component = Game.ObjectPool.Fetch(type);
            component.Parent = parent;
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null)
            {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component);
            return component;
        }

        /// <summary>
        /// 通过类型创建一个组件
        /// </summary>
        /// <param name="type">组件具体类型</param>
        /// <param name="parent">父组件</param>
        /// <returns></returns>
        public static T CreateWithParent<T>(Component parent) where T : Component
        {
            T component = Game.ObjectPool.Fetch<T>();
            component.Parent = parent;
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null)
            {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component);
            return component;
        }

        /// <summary>
        /// 通过类型创建一个组件
        /// </summary>
        /// <param name="type">组件具体类型</param>
        /// <typeparam name="A">组件参数类型</typeparam>
        /// <param name="parent">父组件</param>
        /// <param name="a">组件参数</param>
        /// <returns></returns>
        public static T CreateWithParent<T, A>(Component parent, A a) where T : Component
        {
            T component = Game.ObjectPool.Fetch<T>();
            component.Parent = parent;
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null)
            {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component, a);
            return component;
        }

        /// <summary>
        /// 通过类型创建一个组件
        /// </summary>
        /// <param name="type">组件具体类型</param>
        /// <typeparam name="A">组件参数类型</typeparam>
        /// <typeparam name="B">组件参数类型</typeparam>
        /// <param name="parent">父组件</param>
        /// <param name="a">组件参数</param>
        /// <param name="b">组件参数</param>
        public static T CreateWithParent<T, A, B>(Component parent, A a, B b) where T : Component
        {
            T component = Game.ObjectPool.Fetch<T>();
            component.Parent = parent;
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null)
            {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component, a, b);
            return component;
        }

        /// <summary>
        /// 通过类型创建一个组件
        /// </summary>
        /// <param name="type">组件具体类型</param>
        /// <typeparam name="A">组件参数类型</typeparam>
        /// <typeparam name="B">组件参数类型</typeparam>
        /// <typeparam name="C">组件参数类型</typeparam>
        /// <param name="parent">父组件</param>
        /// <param name="a">组件参数</param>
        /// <param name="b">组件参数</param>
        /// <param name="c">组件参数</param>
        public static T CreateWithParent<T, A, B, C>(Component parent, A a, B b, C c) where T : Component
        {
            T component = Game.ObjectPool.Fetch<T>();
            component.Parent = parent;
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null)
            {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component, a, b, c);
            return component;
        }

        /// <summary>
        /// 通过类型创建一个组件
        /// </summary>
        /// <param name="type">组件具体类型</param>
        public static T Create<T>() where T : Component
        {
            T component = Game.ObjectPool.Fetch<T>();
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null)
            {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component);
            return component;
        }

        /// <summary>
        /// 通过类型创建一个组件
        /// </summary>
        /// <param name="type">组件具体类型</param>
        /// <typeparam name="A">组件参数类型</typeparam>
        /// <param name="a">组件参数</param>
        public static T Create<T, A>(A a) where T : Component
        {
            T component = Game.ObjectPool.Fetch<T>();
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null)
            {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component, a);
            return component;
        }

        /// <summary>
        /// 通过类型创建一个组件
        /// </summary>
        /// <param name="type">组件具体类型</param>
        /// <typeparam name="A">组件参数类型</typeparam>
        /// <typeparam name="B">组件参数类型</typeparam>
        /// <param name="a">组件参数</param>
        /// <param name="b">组件参数</param>
        public static T Create<T, A, B>(A a, B b) where T : Component
        {
            T component = Game.ObjectPool.Fetch<T>();
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null)
            {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component, a, b);
            return component;
        }

        /// <summary>
        /// 通过类型创建一个组件
        /// </summary>
        /// <param name="type">组件具体类型</param>
        /// <typeparam name="A">组件参数类型</typeparam>
        /// <typeparam name="B">组件参数类型</typeparam>
        /// <typeparam name="C">组件参数类型</typeparam>
        /// <param name="a">组件参数</param>
        /// <param name="b">组件参数</param>
        /// <param name="c">组件参数</param>
        public static T Create<T, A, B, C>(A a, B b, C c) where T : Component
        {
            T component = Game.ObjectPool.Fetch<T>();
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null)
            {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component, a, b, c);
            return component;
        }

        /// <summary>
        /// 通过类型创建一个组件
        /// </summary>
        /// <param name="type">组件具体类型</param>
        /// <param name="id">组件Id</param>
        public static T CreateWithId<T>(long id) where T : ComponentWithId
        {
            T component = Game.ObjectPool.Fetch<T>();
            component.Id = id;
            Game.EventSystem.Awake(component);
            return component;
        }

        /// <summary>
        /// 通过类型创建一个组件
        /// </summary>
        /// <param name="type">组件具体类型</param>
        /// <typeparam name="A">组件参数类型</typeparam>
        /// <param name="id">组件Id</param>
        /// <param name="a">组件参数</param>
        public static T CreateWithId<T, A>(long id, A a) where T : ComponentWithId
        {
            T component = Game.ObjectPool.Fetch<T>();
            component.Id = id;
            Game.EventSystem.Awake(component, a);
            return component;
        }

        /// <summary>
        /// 通过类型创建一个组件
        /// </summary>
        /// <param name="type">组件具体类型</param>
        /// <typeparam name="A">组件参数类型</typeparam>
        /// <typeparam name="B">组件参数类型</typeparam>
        /// <param name="id">组件Id</param>
        /// <param name="a">组件参数</param>
        /// <param name="b">组件参数</param>
        public static T CreateWithId<T, A, B>(long id, A a, B b) where T : ComponentWithId
        {
            T component = Game.ObjectPool.Fetch<T>();
            component.Id = id;
            Game.EventSystem.Awake(component, a, b);
            return component;
        }

        /// <summary>
        /// 通过类型创建一个组件
        /// </summary>
        /// <param name="type">组件具体类型</param>
        /// <typeparam name="A">组件参数类型</typeparam>
        /// <typeparam name="B">组件参数类型</typeparam>
        /// <typeparam name="C">组件参数类型</typeparam>
        /// <param name="id">组件Id</param>
        /// <param name="a">组件参数</param>
        /// <param name="b">组件参数</param>
        /// <param name="c">组件参数</param>
        public static T CreateWithId<T, A, B, C>(long id, A a, B b, C c) where T : ComponentWithId
        {
            T component = Game.ObjectPool.Fetch<T>();
            component.Id = id;
            Game.EventSystem.Awake(component, a, b, c);
            return component;
        }
    }
}
