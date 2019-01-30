using System.Net;
using System.Threading.Tasks;

namespace ETModel
{
    /// <summary>
    /// 位置代理组件
    /// </summary>
	public class LocationProxyComponent : Component
    {
        /// <summary>
        /// 位置代理服务的IP地址
        /// </summary>
        public IPEndPoint LocationAddress;
    }
}