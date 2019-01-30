using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    public static class ActorMessageSenderHelper
    {
        /// <summary>
        /// 发送一条actor请求
        /// </summary>
        /// <param name="self"></param>
        /// <param name="message"></param>
        public static void Send(this ActorMessageSender self, IActorMessage message)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.Address);
            message.ActorId = self.ActorId;
            session.Send(message);
        }

        /// <summary>
        /// 发送一个可以等待完成的actor请求s
        /// </summary>
        /// <param name="self"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<IActorResponse> Call(this ActorMessageSender self, IActorRequest message)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.Address);
            message.ActorId = self.ActorId;
            return (IActorResponse)await session.Call(message);
        }
    }
}