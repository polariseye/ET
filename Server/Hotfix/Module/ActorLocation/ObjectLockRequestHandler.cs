using System;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 锁实体请求
    /// </summary>
	[MessageHandler(AppType.Location)]
    public class ObjectLockRequestHandler : AMRpcHandler<ObjectLockRequest, ObjectLockResponse>
    {
        protected override void Run(Session session, ObjectLockRequest message, Action<ObjectLockResponse> reply)
        {
            ObjectLockResponse response = new ObjectLockResponse();
            try
            {
                Game.Scene.GetComponent<LocationComponent>().Lock(message.Key, message.InstanceId, message.Time);
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}