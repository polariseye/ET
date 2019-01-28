using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    /// <summary>
    /// 实体基类
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Entity : ComponentWithId
    {
        /// <summary>
        /// 组件列表
        /// 只记录可序列化的组件列表， <see cref="ISerializeToEntity"/>
        /// </summary>
        [BsonElement("C")]
        [BsonIgnoreIfNull]
        private HashSet<Component> components;

        /// <summary>
        /// 组件列表
        /// </summary>
        [BsonIgnore]
        private Dictionary<Type, Component> componentDict;

        /// <summary>
        /// 实体构造函数
        /// </summary>
        public Entity()
        {
            this.components = new HashSet<Component>();
            this.componentDict = new Dictionary<Type, Component>();
        }

        /// <summary>
        /// 实体构造函数
        /// </summary>
        /// <param name="id">实体Id</param>
        protected Entity(long id) : base(id)
        {
            this.components = new HashSet<Component>();
            this.componentDict = new Dictionary<Type, Component>();
        }

        /// <summary>
        /// 资源释放
        /// 会循环调用内部组件的Dispose函数
        /// </summary>
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            // 释放内部函数
            foreach (Component component in this.componentDict.Values)
            {
                try
                {
                    component.Dispose();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            this.components.Clear();
            this.componentDict.Clear();
        }

        /// <summary>
        /// 给实体添加一个组件
        /// </summary>
        /// <param name="component">组件对象</param>
        /// <returns>返回组件对象</returns>
        public virtual Component AddComponent(Component component)
        {
            Type type = component.GetType();
            if (this.componentDict.ContainsKey(type))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {type.Name}");
            }

            component.Parent = this;

            if (component is ISerializeToEntity)
            {
                this.components.Add(component);
            }
            this.componentDict.Add(type, component);
            return component;
        }

        /// <summary>
        /// 添加一个组件对象
        /// </summary>
        /// <param name="type">组件类型</param>
        /// <returns></returns>
        public virtual Component AddComponent(Type type)
        {
            if (this.componentDict.ContainsKey(type))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {type.Name}");
            }

            Component component = ComponentFactory.CreateWithParent(type, this);

            if (component is ISerializeToEntity)
            {
                this.components.Add(component);
            }
            this.componentDict.Add(type, component);
            return component;
        }

        /// <summary>
        /// 添加一个组件对象
        /// </summary>
        /// <typeparam name="K">组件类型</typeparam>
        /// <returns></returns>
        public virtual K AddComponent<K>() where K : Component, new()
        {
            Type type = typeof(K);
            if (this.componentDict.ContainsKey(type))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
            }

            K component = ComponentFactory.CreateWithParent<K>(this);

            if (component is ISerializeToEntity)
            {
                this.components.Add(component);
            }
            this.componentDict.Add(type, component);
            return component;
        }

        /// <summary>
        /// 创建一个组件对象
        /// </summary>
        /// <typeparam name="K">组件类型</typeparam>
        /// <typeparam name="P1">组件构造所需参数类型</typeparam>
        /// <param name="p1">组件构造所需参数值</param>
        /// <returns></returns>
		public virtual K AddComponent<K, P1>(P1 p1) where K : Component, new()
        {
            Type type = typeof(K);
            if (this.componentDict.ContainsKey(type))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
            }

            K component = ComponentFactory.CreateWithParent<K, P1>(this, p1);

            if (component is ISerializeToEntity)
            {
                this.components.Add(component);
            }
            this.componentDict.Add(type, component);
            return component;
        }

        /// <summary>
        /// 创建一个组件
        /// </summary>
        /// <typeparam name="K">组件类型</typeparam>
        /// <typeparam name="P1">组件构造所需参数类型</typeparam>
        /// <typeparam name="P2">组件构造所需参数类型2</typeparam>
        /// <param name="p1">组件构造所需参数值1</param>
        /// <param name="p2">组件构造所需参数值2</param>
        /// <returns></returns>
        public virtual K AddComponent<K, P1, P2>(P1 p1, P2 p2) where K : Component, new()
        {
            Type type = typeof(K);
            if (this.componentDict.ContainsKey(type))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
            }

            K component = ComponentFactory.CreateWithParent<K, P1, P2>(this, p1, p2);

            if (component is ISerializeToEntity)
            {
                this.components.Add(component);
            }
            this.componentDict.Add(type, component);
            return component;
        }

        /// <summary>
        /// 添加一个组件
        /// </summary>
        /// <typeparam name="K">组件类型</typeparam>
        /// <typeparam name="P1">组件参数类型1</typeparam>
        /// <typeparam name="P2">组件参数类型2</typeparam>
        /// <typeparam name="P3">组件参数类型3</typeparam>
        /// <param name="p1">组件参数1</param>
        /// <param name="p2">组件参数2</param>
        /// <param name="p3">组件参数3</param>
        /// <returns></returns>
        public virtual K AddComponent<K, P1, P2, P3>(P1 p1, P2 p2, P3 p3) where K : Component, new()
        {
            Type type = typeof(K);
            if (this.componentDict.ContainsKey(type))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
            }

            K component = ComponentFactory.CreateWithParent<K, P1, P2, P3>(this, p1, p2, p3);

            if (component is ISerializeToEntity)
            {
                this.components.Add(component);
            }
            this.componentDict.Add(type, component);
            return component;
        }

        /// <summary>
        /// 移除一个组件
        /// </summary>
        /// <typeparam name="K"></typeparam>
        public virtual void RemoveComponent<K>() where K : Component
        {
            if (this.IsDisposed)
            {
                return;
            }
            Type type = typeof(K);
            Component component;
            if (!this.componentDict.TryGetValue(type, out component))
            {
                return;
            }

            this.components.Remove(component);
            this.componentDict.Remove(type);

            component.Dispose();
        }

        /// <summary>
        /// 移除一个组件
        /// </summary>
        /// <param name="type"></param>
        public virtual void RemoveComponent(Type type)
        {
            if (this.IsDisposed)
            {
                return;
            }
            Component component;
            if (!this.componentDict.TryGetValue(type, out component))
            {
                return;
            }

            this.components?.Remove(component);
            this.componentDict.Remove(type);

            component.Dispose();
        }

        /// <summary>
        /// 获取一个组件
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public K GetComponent<K>() where K : Component
        {
            Component component;
            if (!this.componentDict.TryGetValue(typeof(K), out component))
            {
                return default(K);
            }
            return (K)component;
        }

        /// <summary>
        /// 获取一个组件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Component GetComponent(Type type)
        {
            Component component;
            if (!this.componentDict.TryGetValue(type, out component))
            {
                return null;
            }
            return component;
        }

        /// <summary>
        /// 获取组件列表
        /// </summary>
        /// <returns></returns>
        public Component[] GetComponents()
        {
            return this.componentDict.Values.ToArray();
        }

        /// <summary>
        /// 结束初始化
        /// </summary>
        public override void EndInit()
        {
            try
            {
                base.EndInit();

                this.componentDict.Clear();

                if (this.components != null)
                {
                    foreach (Component component in this.components)
                    {
                        component.Parent = this;
                        this.componentDict.Add(component.GetType(), component);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        /// <summary>
        /// 开始序列化
        /// </summary>
        public override void BeginSerialize()
        {
            base.BeginSerialize();

            foreach (Component component in this.components)
            {
                component.BeginSerialize();
            }
        }

        /// <summary>
        /// 结束序列化
        /// </summary>
        public override void EndDeSerialize()
        {
            base.EndDeSerialize();

            foreach (Component component in this.components)
            {
                component.EndDeSerialize();
            }
        }
    }
}