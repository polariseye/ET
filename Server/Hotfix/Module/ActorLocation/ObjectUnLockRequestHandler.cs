using System;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 请求解除锁使用
    /// </summary>
	[MessageHandler(AppType.Location)]
    public class ObjectUnLockRequestHandler : AMRpcHandler<ObjectUnLockRequest, ObjectUnLockResponse>
    {
        protected override void Run(Session session, ObjectUnLockRequest message, Action<ObjectUnLockResponse> reply)
        {
            ObjectUnLockResponse response = new ObjectUnLockResponse();
            try
            {
                Game.Scene.GetComponent<LocationComponent>().UnLockAndUpdate(message.Key, message.OldInstanceId, message.InstanceId);
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}