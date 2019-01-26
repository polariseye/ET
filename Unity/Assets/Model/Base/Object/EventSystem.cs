using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ETModel
{
    public enum DLLType
    {
        /// <summary>
        /// 实体模块
        /// </summary>
        Model,
        /// <summary>
        /// 热更模块
        /// </summary>
        Hotfix,
        /// <summary>
        /// 编辑器相关组件
        /// </summary>
        Editor,
    }

    /// <summary>
    /// 事件系统
    /// </summary>
	public sealed class EventSystem
    {
        private readonly Dictionary<long, Component> allComponents = new Dictionary<long, Component>();

        /// <summary>
        /// DLL类型到程序集的映射
        /// </summary>
        private readonly Dictionary<DLLType, Assembly> assemblies = new Dictionary<DLLType, Assembly>();

        /// <summary>
        /// 类型映射缓存
        /// Key:自定义标签的具体类型 <see cref="BaseAttribute"/>
        /// Value：对应Class的类型
        /// </summary>
        private readonly UnOrderMultiMap<Type, Type> types = new UnOrderMultiMap<Type, Type>();

        private readonly Dictionary<string, List<IEvent>> allEvents = new Dictionary<string, List<IEvent>>();

        private readonly UnOrderMultiMap<Type, IAwakeSystem> awakeSystems = new UnOrderMultiMap<Type, IAwakeSystem>();

        private readonly UnOrderMultiMap<Type, IStartSystem> startSystems = new UnOrderMultiMap<Type, IStartSystem>();

        private readonly UnOrderMultiMap<Type, IDestroySystem> destroySystems = new UnOrderMultiMap<Type, IDestroySystem>();

        private readonly UnOrderMultiMap<Type, ILoadSystem> loadSystems = new UnOrderMultiMap<Type, ILoadSystem>();

        private readonly UnOrderMultiMap<Type, IUpdateSystem> updateSystems = new UnOrderMultiMap<Type, IUpdateSystem>();

        private readonly UnOrderMultiMap<Type, ILateUpdateSystem> lateUpdateSystems = new UnOrderMultiMap<Type, ILateUpdateSystem>();

        private readonly UnOrderMultiMap<Type, IChangeSystem> changeSystems = new UnOrderMultiMap<Type, IChangeSystem>();

        private Queue<long> updates = new Queue<long>();
        private Queue<long> updates2 = new Queue<long>();

        private readonly Queue<long> starts = new Queue<long>();

        private Queue<long> loaders = new Queue<long>();
        private Queue<long> loaders2 = new Queue<long>();

        private Queue<long> lateUpdates = new Queue<long>();
        private Queue<long> lateUpdates2 = new Queue<long>();

        /// <summary>
        /// 对一个程序集进行反射，取出所有关注的事件
        /// 每次调用时，会把之前调用的信息给清除掉
        /// （每次调用时，都会调用已有组件的Load实现）
        /// </summary>
        /// <param name="dllType">DLL类型</param>
        /// <param name="assembly">程序集</param>
        public void Add(DLLType dllType, Assembly assembly)
        {
            this.assemblies[dllType] = assembly;

            // 获取所有Class中自定义的标签到具体类的映射
            this.types.Clear();
            foreach (Assembly value in this.assemblies.Values)
            {
                foreach (Type type in value.GetTypes())
                {
                    object[] objects = type.GetCustomAttributes(typeof(BaseAttribute), false);
                    if (objects.Length == 0)
                    {
                        continue;
                    }

                    BaseAttribute baseAttribute = (BaseAttribute)objects[0];
                    this.types.Add(baseAttribute.AttributeType, type);
                }
            }

            // 清理子系统缓存的所有事件Class类型
            this.awakeSystems.Clear();
            this.lateUpdateSystems.Clear();
            this.updateSystems.Clear();
            this.startSystems.Clear();
            this.loadSystems.Clear();
            this.changeSystems.Clear();
            this.destroySystems.Clear();

            // 把所有带ObjectSystemAttribute的类实例化，并取出继承的的相关接口,最后按接口分类 
            foreach (Type type in types[typeof(ObjectSystemAttribute)])
            {
                object[] attrs = type.GetCustomAttributes(typeof(ObjectSystemAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                object obj = Activator.CreateInstance(type);

                // 如果是需要唤醒系统的，则添加唤醒列表里
                IAwakeSystem objectSystem = obj as IAwakeSystem;
                if (objectSystem != null)
                {
                    this.awakeSystems.Add(objectSystem.Type(), objectSystem);
                }

                IUpdateSystem updateSystem = obj as IUpdateSystem;
                if (updateSystem != null)
                {
                    this.updateSystems.Add(updateSystem.Type(), updateSystem);
                }

                ILateUpdateSystem lateUpdateSystem = obj as ILateUpdateSystem;
                if (lateUpdateSystem != null)
                {
                    this.lateUpdateSystems.Add(lateUpdateSystem.Type(), lateUpdateSystem);
                }

                IStartSystem startSystem = obj as IStartSystem;
                if (startSystem != null)
                {
                    this.startSystems.Add(startSystem.Type(), startSystem);
                }

                IDestroySystem destroySystem = obj as IDestroySystem;
                if (destroySystem != null)
                {
                    this.destroySystems.Add(destroySystem.Type(), destroySystem);
                }

                ILoadSystem loadSystem = obj as ILoadSystem;
                if (loadSystem != null)
                {
                    this.loadSystems.Add(loadSystem.Type(), loadSystem);
                }

                IChangeSystem changeSystem = obj as IChangeSystem;
                if (changeSystem != null)
                {
                    this.changeSystems.Add(changeSystem.Type(), changeSystem);
                }
            }

            // 把所有带EventAttribute的类实例化，并取出继承的的相关接口,最后注册到对象 allEvents 中
            this.allEvents.Clear();
            foreach (Type type in types[typeof(EventAttribute)])
            {
                object[] attrs = type.GetCustomAttributes(typeof(EventAttribute), false);

                foreach (object attr in attrs)
                {
                    EventAttribute aEventAttribute = (EventAttribute)attr;
                    object obj = Activator.CreateInstance(type);
                    IEvent iEvent = obj as IEvent;
                    if (iEvent == null)
                    {
                        Log.Error($"{obj.GetType().Name} 没有继承IEvent");
                    }

                    // 注册事件
                    this.RegisterEvent(aEventAttribute.Type, iEvent);
                }
            }

            // 调用ILoadSystem接口的Load函数
            this.Load();
        }

        /// <summary>
        /// 注册一个事件(记录下来，以便后面触发)
        /// </summary>
        /// <param name="eventId">事件Id</param>
        /// <param name="e"></param>
        public void RegisterEvent(string eventId, IEvent e)
        {
            if (!this.allEvents.ContainsKey(eventId))
            {
                this.allEvents.Add(eventId, new List<IEvent>());
            }

            this.allEvents[eventId].Add(e);
        }

        public Assembly Get(DLLType dllType)
        {
            return this.assemblies[dllType];
        }

        public List<Type> GetTypes(Type systemAttributeType)
        {
            if (!this.types.ContainsKey(systemAttributeType))
            {
                return new List<Type>();
            }
            return this.types[systemAttributeType];
        }

        /// <summary>
        /// 注册一个组件
        /// 热更和需要用到组件池时才会使用
        /// </summary>
        /// <param name="component"></param>
        public void Add(Component component)
        {
            this.allComponents.Add(component.InstanceId, component);

            Type type = component.GetType();

            if (this.loadSystems.ContainsKey(type))
            {
                this.loaders.Enqueue(component.InstanceId);
            }

            if (this.updateSystems.ContainsKey(type))
            {
                this.updates.Enqueue(component.InstanceId);
            }

            if (this.startSystems.ContainsKey(type))
            {
                this.starts.Enqueue(component.InstanceId);
            }

            if (this.lateUpdateSystems.ContainsKey(type))
            {
                this.lateUpdates.Enqueue(component.InstanceId);
            }
        }

        /// <summary>
        /// 删除一个组件
        /// </summary>
        /// <param name="instanceId">组件实例Id</param>
        public void Remove(long instanceId)
        {
            this.allComponents.Remove(instanceId);
        }

        public Component Get(long id)
        {
            Component component = null;
            this.allComponents.TryGetValue(id, out component);
            return component;
        }

        /// <summary>
        /// 创建一个组件实例时，触发对应的IAwake接口
        /// </summary>
        /// <param name="component">组件对象</param>
        public void Awake(Component component)
        {
            List<IAwakeSystem> iAwakeSystems = this.awakeSystems[component.GetType()];
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                IAwake iAwake = aAwakeSystem as IAwake;
                if (iAwake == null)
                {
                    continue;
                }

                try
                {
                    iAwake.Run(component);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        /// <summary>
        /// 创建一个组件实例时，触发对应的IAwake接口
        /// </summary>
        /// <param name="component">组件对象</param>
        public void Awake<P1>(Component component, P1 p1)
        {
            List<IAwakeSystem> iAwakeSystems = this.awakeSystems[component.GetType()];
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                IAwake<P1> iAwake = aAwakeSystem as IAwake<P1>;
                if (iAwake == null)
                {
                    continue;
                }

                try
                {
                    iAwake.Run(component, p1);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        /// <summary>
        /// 创建一个组件实例时，触发对应的IAwake接口
        /// </summary>
        /// <param name="component">组件对象</param>
        /// <param name="p1">参数1</param>
        /// <param name="p2">参数2</param>
        public void Awake<P1, P2>(Component component, P1 p1, P2 p2)
        {
            List<IAwakeSystem> iAwakeSystems = this.awakeSystems[component.GetType()];
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                IAwake<P1, P2> iAwake = aAwakeSystem as IAwake<P1, P2>;
                if (iAwake == null)
                {
                    continue;
                }

                try
                {
                    iAwake.Run(component, p1, p2);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        /// <summary>
        /// 创建一个组件实例时，触发对应的IAwake接口
        /// </summary>
        /// <param name="component">组件对象</param>
        /// <param name="p1">参数1</param>
        /// <param name="p2">参数2</param>
        /// <param name="p3">参数3</param>
        public void Awake<P1, P2, P3>(Component component, P1 p1, P2 p2, P3 p3)
        {
            List<IAwakeSystem> iAwakeSystems = this.awakeSystems[component.GetType()];
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                IAwake<P1, P2, P3> iAwake = aAwakeSystem as IAwake<P1, P2, P3>;
                if (iAwake == null)
                {
                    continue;
                }

                try
                {
                    iAwake.Run(component, p1, p2, p3);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Change(Component component)
        {
            List<IChangeSystem> iChangeSystems = this.changeSystems[component.GetType()];
            if (iChangeSystems == null)
            {
                return;
            }

            foreach (IChangeSystem iChangeSystem in iChangeSystems)
            {
                if (iChangeSystem == null)
                {
                    continue;
                }

                try
                {
                    iChangeSystem.Run(component);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        /// <summary>
        /// 进行具体加载 调用所有已加入的组件，并调用对应的ILoadSystem接口
        /// (只有热更新时，才会被调用)
        /// </summary>
        public void Load()
        {
            while (this.loaders.Count > 0)
            {
                long instanceId = this.loaders.Dequeue();
                Component component;
                if (!this.allComponents.TryGetValue(instanceId, out component))
                {
                    continue;
                }
                if (component.IsDisposed)
                {
                    continue;
                }

                List<ILoadSystem> iLoadSystems = this.loadSystems[component.GetType()];
                if (iLoadSystems == null)
                {
                    continue;
                }

                this.loaders2.Enqueue(instanceId);

                foreach (ILoadSystem iLoadSystem in iLoadSystems)
                {
                    try
                    {
                        iLoadSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }

            ObjectHelper.Swap(ref this.loaders, ref this.loaders2);
        }

        /// <summary>
        /// 调用所有Component的Start实现
        /// </summary>
        private void Start()
        {
            while (this.starts.Count > 0)
            {
                long instanceId = this.starts.Dequeue();
                Component component;
                if (!this.allComponents.TryGetValue(instanceId, out component))
                {
                    continue;
                }

                List<IStartSystem> iStartSystems = this.startSystems[component.GetType()];
                if (iStartSystems == null)
                {
                    continue;
                }

                foreach (IStartSystem iStartSystem in iStartSystems)
                {
                    try
                    {
                        iStartSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }

        /// <summary>
        /// 模块销毁时调用
        /// </summary>
        /// <param name="component">组件对象</param>
        public void Destroy(Component component)
        {
            List<IDestroySystem> iDestroySystems = this.destroySystems[component.GetType()];
            if (iDestroySystems == null)
            {
                return;
            }

            foreach (IDestroySystem iDestroySystem in iDestroySystems)
            {
                if (iDestroySystem == null)
                {
                    continue;
                }

                try
                {
                    iDestroySystem.Run(component);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        /// <summary>
        /// 调用一次注册进来的所有组件的Update实现
        /// (在每个循环周期都调用一次)
        /// </summary>
        public void Update()
        {
            // 调用组件Start接口
            this.Start();

            while (this.updates.Count > 0)
            {
                long instanceId = this.updates.Dequeue();
                Component component;
                if (!this.allComponents.TryGetValue(instanceId, out component))
                {
                    continue;
                }
                if (component.IsDisposed)
                {
                    continue;
                }

                List<IUpdateSystem> iUpdateSystems = this.updateSystems[component.GetType()];
                if (iUpdateSystems == null)
                {
                    continue;
                }

                this.updates2.Enqueue(instanceId);

                foreach (IUpdateSystem iUpdateSystem in iUpdateSystems)
                {
                    try
                    {
                        iUpdateSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }

            ObjectHelper.Swap(ref this.updates, ref this.updates2);
        }

        public void LateUpdate()
        {
            while (this.lateUpdates.Count > 0)
            {
                long instanceId = this.lateUpdates.Dequeue();
                Component component;
                if (!this.allComponents.TryGetValue(instanceId, out component))
                {
                    continue;
                }
                if (component.IsDisposed)
                {
                    continue;
                }

                List<ILateUpdateSystem> iLateUpdateSystems = this.lateUpdateSystems[component.GetType()];
                if (iLateUpdateSystems == null)
                {
                    continue;
                }

                this.lateUpdates2.Enqueue(instanceId);

                foreach (ILateUpdateSystem iLateUpdateSystem in iLateUpdateSystems)
                {
                    try
                    {
                        iLateUpdateSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }

            ObjectHelper.Swap(ref this.lateUpdates, ref this.lateUpdates2);
        }

        public void Run(string type)
        {
            List<IEvent> iEvents;
            if (!this.allEvents.TryGetValue(type, out iEvents))
            {
                return;
            }
            foreach (IEvent iEvent in iEvents)
            {
                try
                {
                    iEvent?.Handle();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Run<A>(string type, A a)
        {
            List<IEvent> iEvents;
            if (!this.allEvents.TryGetValue(type, out iEvents))
            {
                return;
            }
            foreach (IEvent iEvent in iEvents)
            {
                try
                {
                    iEvent?.Handle(a);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Run<A, B>(string type, A a, B b)
        {
            List<IEvent> iEvents;
            if (!this.allEvents.TryGetValue(type, out iEvents))
            {
                return;
            }
            foreach (IEvent iEvent in iEvents)
            {
                try
                {
                    iEvent?.Handle(a, b);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Run<A, B, C>(string type, A a, B b, C c)
        {
            List<IEvent> iEvents;
            if (!this.allEvents.TryGetValue(type, out iEvents))
            {
                return;
            }
            foreach (IEvent iEvent in iEvents)
            {
                try
                {
                    iEvent?.Handle(a, b, c);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}