using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 外网组件初始化
    /// </summary>
	[ObjectSystem]
    public class NetOuterComponentAwakeSystem : AwakeSystem<NetOuterComponent>
    {
        public override void Awake(NetOuterComponent self)
        {
            self.Awake(self.Protocol);
            self.MessagePacker = new ProtobufPacker();
            self.MessageDispatcher = new OuterMessageDispatcher();
        }
    }

    /// <summary>
    /// 外网组件初始化
    /// </summary>
    [ObjectSystem]
    public class NetOuterComponentAwake1System : AwakeSystem<NetOuterComponent, string>
    {
        public override void Awake(NetOuterComponent self, string address)
        {
            self.Awake(self.Protocol, address);
            self.MessagePacker = new ProtobufPacker();
            self.MessageDispatcher = new OuterMessageDispatcher();
        }
    }

    /// <summary>
    /// 外网组件热更处理
    /// </summary>
    [ObjectSystem]
    public class NetOuterComponentLoadSystem : LoadSystem<NetOuterComponent>
    {
        public override void Load(NetOuterComponent self)
        {
            self.MessagePacker = new ProtobufPacker();
            self.MessageDispatcher = new OuterMessageDispatcher();
        }
    }

    /// <summary>
    /// 外网组件定时处理
    /// </summary>
    [ObjectSystem]
    public class NetOuterComponentUpdateSystem : UpdateSystem<NetOuterComponent>
    {
        public override void Update(NetOuterComponent self)
        {
            self.Update();
        }
    }
}