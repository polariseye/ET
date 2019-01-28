using System;
using System.Collections.Generic;

namespace ETModel
{
    /// <summary>
    /// 加载消息类型映射的事件
    /// </summary>
    [ObjectSystem]
    public class OpcodeTypeComponentSystem : AwakeSystem<OpcodeTypeComponent>
    {
        public override void Awake(OpcodeTypeComponent self)
        {
            self.Load();
        }
    }

    /// <summary>
    /// 热更时的加载消息类型映射
    /// </summary>
    [ObjectSystem]
    public class OpcodeTypeComponentLoadSystem : LoadSystem<OpcodeTypeComponent>
    {
        public override void Load(OpcodeTypeComponent self)
        {
            self.Load();
        }
    }

    /// <summary>
    /// 编码到类型的一个映射组件,用于记录消息码到消息类型的映射信息
    /// 加载时会提取程序集中所有的消息编码和类型信息
    /// <see cref="MessageAttribute"/>
    /// </summary>
	public class OpcodeTypeComponent : Component
    {
        /// <summary>
        /// 类型编码数据字典
        /// Key:类型编码
        /// Value:对应的Class类型
        /// </summary>
        private readonly DoubleMap<ushort, Type> opcodeTypes = new DoubleMap<ushort, Type>();

        /// <summary>
        /// 类型编码的具体对象字典
        /// Key:类型编码
        /// Value:类型编码的实例对象
        /// </summary>
        private readonly Dictionary<ushort, object> typeMessages = new Dictionary<ushort, object>();

        /// <summary>
        /// 类型映射加载
        /// </summary>
        public void Load()
        {
            this.opcodeTypes.Clear();
            this.typeMessages.Clear();

            // 提取类型编码到类型的映射
            List<Type> types = Game.EventSystem.GetTypes(typeof(MessageAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(MessageAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                MessageAttribute messageAttribute = attrs[0] as MessageAttribute;
                if (messageAttribute == null)
                {
                    continue;
                }

                this.opcodeTypes.Add(messageAttribute.Opcode, type);
                this.typeMessages.Add(messageAttribute.Opcode, Activator.CreateInstance(type));
            }
        }

        /// <summary>
        /// 按照类型获取对应的类型编码
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <returns></returns>
        public ushort GetOpcode(Type type)
        {
            return this.opcodeTypes.GetKeyByValue(type);
        }

        /// <summary>
        /// 按照类型编码获取对应的类型
        /// </summary>
        /// <param name="opcode">类型编码</param>
        /// <returns></returns>
        public Type GetType(ushort opcode)
        {
            return this.opcodeTypes.GetValueByKey(opcode);
        }

        /// <summary>
        /// 获取类型实例（是为了减少GC而创建的）
        /// 客户端为了0GC需要消息池，服务端消息需要跨协程不需要消息池
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        public object GetInstance(ushort opcode)
        {
#if SERVER
            Type type = this.GetType(opcode);
            return Activator.CreateInstance(type);
#else
			return this.typeMessages[opcode];
#endif
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
        }
    }
}