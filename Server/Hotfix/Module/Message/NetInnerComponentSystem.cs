using System.Net;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 内网连接加载事件处理
    /// </summary>
    [ObjectSystem]
    public class NetInnerComponentAwakeSystem : AwakeSystem<NetInnerComponent>
    {
        public override void Awake(NetInnerComponent self)
        {
            self.Awake();
        }
    }

    /// <summary>
    /// 内网连接加载处理
    /// </summary>
    [ObjectSystem]
    public class NetInnerComponentAwake1System : AwakeSystem<NetInnerComponent, string>
    {
        public override void Awake(NetInnerComponent self, string a)
        {
            self.Awake(a);
        }
    }

    /// <summary>
    /// 内网连接热更处理
    /// </summary>
    [ObjectSystem]
    public class NetInnerComponentLoadSystem : LoadSystem<NetInnerComponent>
    {
        public override void Load(NetInnerComponent self)
        {
            self.MessagePacker = new MongoPacker();
            self.MessageDispatcher = new InnerMessageDispatcher();
        }
    }

    /// <summary>
    /// 内网连接轮训处理
    /// </summary>
    [ObjectSystem]
    public class NetInnerComponentUpdateSystem : UpdateSystem<NetInnerComponent>
    {
        public override void Update(NetInnerComponent self)
        {
            self.Update();
        }
    }

    /// <summary>
    /// 内网连接处理扩展函数
    /// </summary>
    public static class NetInnerComponentHelper
    {
        /// <summary>
        /// 加载处理
        /// </summary>
        /// <param name="self"></param>
        public static void Awake(this NetInnerComponent self)
        {
            self.Awake(NetworkProtocol.TCP, Packet.PacketSizeLength4);
            self.MessagePacker = new MongoPacker();
            self.MessageDispatcher = new InnerMessageDispatcher();
            self.AppType = StartConfigComponent.Instance.StartConfig.AppType;
        }

        /// <summary>
        /// 加载处理
        /// </summary>
        /// <param name="self"></param>
        /// <param name="address"></param>
        public static void Awake(this NetInnerComponent self, string address)
        {
            self.Awake(NetworkProtocol.TCP, address, Packet.PacketSizeLength4);
            self.MessagePacker = new MongoPacker();
            self.MessageDispatcher = new InnerMessageDispatcher();
            self.AppType = StartConfigComponent.Instance.StartConfig.AppType;
        }

        /// <summary>
        /// 轮训处理
        /// </summary>
        /// <param name="self"></param>
        public static void Update(this NetInnerComponent self)
        {
            self.Update();
        }
    }
}