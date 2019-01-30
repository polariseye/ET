using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 消息分发组件加载
    /// </summary>
	[ObjectSystem]
    public class MessageDispatherComponentAwakeSystem : AwakeSystem<MessageDispatherComponent>
    {
        public override void Awake(MessageDispatherComponent self)
        {
            self.Load();
        }
    }

    /// <summary>
    /// 热更时，消息分发组件加载
    /// </summary>
	[ObjectSystem]
    public class MessageDispatherComponentLoadSystem : LoadSystem<MessageDispatherComponent>
    {
        public override void Load(MessageDispatherComponent self)
        {
            self.Load();
        }
    }

    /// <summary>
    /// 消息分发组件
    /// 分发给具体actor处理函数处理
    /// </summary>
    public static class MessageDispatherComponentEx
    {
        /// <summary>
        /// 加载消息组件
        /// </summary>
        /// <param name="self"></param>
		public static void Load(this MessageDispatherComponent self)
        {
            self.Handlers.Clear();

            AppType appType = StartConfigComponent.Instance.StartConfig.AppType;

            List<Type> types = Game.EventSystem.GetTypes(typeof(MessageHandlerAttribute));

            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(MessageHandlerAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                MessageHandlerAttribute messageHandlerAttribute = attrs[0] as MessageHandlerAttribute;
                if (!messageHandlerAttribute.Type.Is(appType))
                {
                    continue;
                }

                IMHandler iMHandler = Activator.CreateInstance(type) as IMHandler;
                if (iMHandler == null)
                {
                    Log.Error($"message handle {type.Name} 需要继承 IMHandler");
                    continue;
                }

                Type messageType = iMHandler.GetMessageType();
                ushort opcode = Game.Scene.GetComponent<OpcodeTypeComponent>().GetOpcode(messageType);
                if (opcode == 0)
                {
                    Log.Error($"消息opcode为0: {messageType.Name}");
                    continue;
                }
                self.RegisterHandler(opcode, iMHandler);
            }
        }

        /// <summary>
        /// 注册一个处理对象
        /// </summary>
        /// <param name="self"></param>
        /// <param name="opcode"></param>
        /// <param name="handler"></param>
        public static void RegisterHandler(this MessageDispatherComponent self, ushort opcode, IMHandler handler)
        {
            if (!self.Handlers.ContainsKey(opcode))
            {
                self.Handlers.Add(opcode, new List<IMHandler>());
            }
            self.Handlers[opcode].Add(handler);
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="self"></param>
        /// <param name="session"></param>
        /// <param name="messageInfo"></param>
        public static void Handle(this MessageDispatherComponent self, Session session, MessageInfo messageInfo)
        {
            List<IMHandler> actions;
            if (!self.Handlers.TryGetValue(messageInfo.Opcode, out actions))
            {
                Log.Error($"消息没有处理: {messageInfo.Opcode} {JsonHelper.ToJson(messageInfo.Message)}");
                return;
            }

            foreach (IMHandler ev in actions)
            {
                try
                {
                    ev.Handle(session, messageInfo.Message);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}