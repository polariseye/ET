using System;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 新增一个实体时，添加锁的请求
    /// </summary>
	[MessageHandler(AppType.Location)]
    public class ObjectAddRequestHandler : AMRpcHandler<ObjectAddRequest, ObjectAddResponse>
    {
        protected override void Run(Session session, ObjectAddRequest message, Action<ObjectAddResponse> reply)
        {
            ObjectAddResponse response = new ObjectAddResponse();
            try
            {
                Game.Scene.GetComponent<LocationComponent>().Add(message.Key, message.InstanceId);
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}