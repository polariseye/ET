using System;
using System.Threading.Tasks;

namespace ETModel
{
    /// <summary>
    /// 获取Actor位置的基类（RPC方式）
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="Request"></typeparam>
    /// <typeparam name="Response"></typeparam>
	public abstract class AMActorLocationRpcHandler<E, Request, Response> : IMActorHandler where E : Entity where Request : class, IActorLocationRequest where Response : class, IActorLocationResponse
    {
        /// <summary>
        /// 应答为出错
        /// </summary>
        /// <param name="response"></param>
        /// <param name="e"></param>
        /// <param name="reply"></param>
        protected static void ReplyError(Response response, Exception e, Action<Response> reply)
        {
            Log.Error(e);
            response.Error = ErrorCode.ERR_RpcFail;
            response.Message = e.ToString();
            reply(response);
        }

        /// <summary>
        /// 具体执行函数
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="message"></param>
        /// <param name="reply"></param>
        /// <returns></returns>
        protected abstract Task Run(E unit, Request message, Action<Response> reply);

        /// <summary>
        /// 处理函数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entity"></param>
        /// <param name="actorMessage"></param>
        /// <returns></returns>
        public async Task Handle(Session session, Entity entity, object actorMessage)
        {
            try
            {
                Request request = actorMessage as Request;
                if (request == null)
                {
                    Log.Error($"消息类型转换错误: {actorMessage.GetType().FullName} to {typeof(Request).Name}");
                    return;
                }
                E e = entity as E;
                if (e == null)
                {
                    Log.Error($"Actor类型转换错误: {entity.GetType().Name} to {typeof(E).Name}");
                    return;
                }

                int rpcId = request.RpcId;

                long instanceId = session.InstanceId;

                await this.Run(e, request, response =>
                {
                    // 等回调回来,session可以已经断开了,所以需要判断session InstanceId是否一样
                    if (session.InstanceId != instanceId)
                    {
                        return;
                    }
                    response.RpcId = rpcId;

                    session.Reply(response);
                });
            }
            catch (Exception e)
            {
                throw new Exception($"解释消息失败: {actorMessage.GetType().FullName}", e);
            }
        }

        public Type GetMessageType()
        {
            return typeof(Request);
        }
    }
}