using System;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 移除一个对象的请求
    /// </summary>
	[MessageHandler(AppType.Location)]
    public class ObjectRemoveRequestHandler : AMRpcHandler<ObjectRemoveRequest, ObjectRemoveResponse>
    {
        protected override void Run(Session session, ObjectRemoveRequest message, Action<ObjectRemoveResponse> reply)
        {
            ObjectRemoveResponse response = new ObjectRemoveResponse();
            try
            {
                Game.Scene.GetComponent<LocationComponent>().Remove(message.Key);
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}