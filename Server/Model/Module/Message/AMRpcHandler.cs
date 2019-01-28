using System;

namespace ETModel
{
    /// <summary>
    /// 远程过程调用的实现基类
    /// </summary>
    /// <typeparam name="Request">请求</typeparam>
    /// <typeparam name="Response">应答</typeparam>
	public abstract class AMRpcHandler<Request, Response> : IMHandler where Request : class, IRequest where Response : class, IResponse
    {
        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="response">应答对象</param>
        /// <param name="e">错误信息</param>
        /// <param name="reply">应答函数</param>
		protected static void ReplyError(Response response, Exception e, Action<Response> reply)
        {
            Log.Error(e);
            response.Error = ErrorCode.ERR_RpcFail;
            response.Message = e.ToString();
            reply(response);
        }

        /// <summary>
        /// 具体处理函数
        /// </summary>
        /// <param name="session">会话对象</param>
        /// <param name="message">请求的消息</param>
        /// <param name="reply">应答处理函数</param>
        protected abstract void Run(Session session, Request message, Action<Response> reply);

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="session">会话对象</param>
        /// <param name="message">消息实体</param>
        public void Handle(Session session, object message)
        {
            try
            {
                Request request = message as Request;
                if (request == null)
                {
                    Log.Error($"消息类型转换错误: {message.GetType().Name} to {typeof(Request).Name}");
                }

                int rpcId = request.RpcId;

                long instanceId = session.InstanceId;

                this.Run(session, request, response =>
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
                throw new Exception($"解释消息失败: {message.GetType().FullName}", e);
            }
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        /// <returns></returns>
        public Type GetMessageType()
        {
            return typeof(Request);
        }
    }
}