using System;
using ETModel;
using Google.Protobuf;

namespace ETHotfix
{
    /// <summary>
    /// 外网消息分发处理
    /// </summary>
    public class OuterMessageDispatcher : IMessageDispatcher
    {
        /// <summary>
        /// 消息分发处理接口
        /// </summary>
        /// <param name="session">会话对象</param>
        /// <param name="opcode">消息类型编码</param>
        /// <param name="message">消息对象</param>
        public async void Dispatch(Session session, ushort opcode, object message)
        {
            try
            {
                switch (message)
                {
                    case IFrameMessage iFrameMessage: // 如果是帧消息，构造成OneFrameMessage发给对应的unit
                        {
                            long unitId = session.GetComponent<SessionPlayerComponent>().Player.UnitId;
                            ActorLocationSender actorLocationSender = Game.Scene.GetComponent<ActorLocationSenderComponent>().Get(unitId);

                            // 这里设置了帧消息的id，防止客户端伪造
                            iFrameMessage.Id = unitId;

                            OneFrameMessage oneFrameMessage = new OneFrameMessage
                            {
                                Op = opcode,
                                AMessage = ByteString.CopyFrom(session.Network.MessagePacker.SerializeTo(iFrameMessage))
                            };
                            actorLocationSender.Send(oneFrameMessage);
                            return;
                        }
                    case IActorLocationRequest actorLocationRequest: // gate session收到actor rpc消息，先向actor 发送rpc请求，再将请求结果返回客户端
                        {
                            long unitId = session.GetComponent<SessionPlayerComponent>().Player.UnitId; //// 获取连接关联的玩家Id
                            ActorLocationSender actorLocationSender = Game.Scene.GetComponent<ActorLocationSenderComponent>().Get(unitId);

                            int rpcId = actorLocationRequest.RpcId; // 这里要保存客户端的rpcId
                            IResponse response = await actorLocationSender.Call(actorLocationRequest);
                            response.RpcId = rpcId;

                            session.Reply(response);
                            return;
                        }
                    case IActorLocationMessage actorLocationMessage:
                        {
                            long unitId = session.GetComponent<SessionPlayerComponent>().Player.UnitId;
                            ActorLocationSender actorLocationSender = Game.Scene.GetComponent<ActorLocationSenderComponent>().Get(unitId);
                            actorLocationSender.Send(actorLocationMessage);
                            return;
                        }
                }

                Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, new MessageInfo(opcode, message));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
