using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace ETModel
{
    /// <summary>
    /// 启动函数加载事件
    /// </summary>
    [ObjectSystem]
    public class StartConfigComponentSystem : AwakeSystem<StartConfigComponent, string, int>
    {
        public override void Awake(StartConfigComponent self, string a, int b)
        {
            self.Awake(a, b);
        }
    }

    /// <summary>
    /// 启动时的配置参数组件
    /// </summary>
    public class StartConfigComponent : Component
    {
        /// <summary>
        /// 自身实例对象
        /// </summary>
        public static StartConfigComponent Instance { get; private set; }

        /// <summary>
        /// 加载得到的所有配置信息
        /// Key:AppId
        /// Value:配置信息
        /// </summary>
        private Dictionary<int, StartConfig> configDict;

        /// <summary>
        /// 本机的所有内网地址
        /// </summary>
        private Dictionary<int, IPEndPoint> innerAddressDict = new Dictionary<int, IPEndPoint>();

        /// <summary>
        /// 本程序的启动配置
        /// </summary>
        public StartConfig StartConfig { get; private set; }

        /// <summary>
        /// 数据库配置
        /// </summary>
        public StartConfig DBConfig { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public StartConfig RealmConfig { get; private set; }

        /// <summary>
        /// 服务位置配置
        /// </summary>
        public StartConfig LocationConfig { get; private set; }

        /// <summary>
        /// 地图配置
        /// </summary>
        public List<StartConfig> MapConfigs { get; private set; }

        /// <summary>
        /// 网关配置
        /// </summary>
        public List<StartConfig> GateConfigs { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="appId"></param>
        public void Awake(string path, int appId)
        {
            Instance = this;

            this.configDict = new Dictionary<int, StartConfig>();
            this.MapConfigs = new List<StartConfig>();
            this.GateConfigs = new List<StartConfig>();

            string[] ss = File.ReadAllText(path).Split('\n');
            foreach (string s in ss)
            {
                string s2 = s.Trim();
                if (s2 == "")
                {
                    continue;
                }
                try
                {
                    StartConfig startConfig = MongoHelper.FromJson<StartConfig>(s2);
                    this.configDict.Add(startConfig.AppId, startConfig);

                    InnerConfig innerConfig = startConfig.GetComponent<InnerConfig>();
                    if (innerConfig != null)
                    {
                        this.innerAddressDict.Add(startConfig.AppId, innerConfig.IPEndPoint);
                    }

                    if (startConfig.AppType.Is(AppType.Realm))
                    {
                        this.RealmConfig = startConfig;
                    }

                    if (startConfig.AppType.Is(AppType.Location))
                    {
                        this.LocationConfig = startConfig;
                    }

                    if (startConfig.AppType.Is(AppType.DB))
                    {
                        this.DBConfig = startConfig;
                    }

                    if (startConfig.AppType.Is(AppType.Map))
                    {
                        this.MapConfigs.Add(startConfig);
                    }

                    if (startConfig.AppType.Is(AppType.Gate))
                    {
                        this.GateConfigs.Add(startConfig);
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"config错误: {s2} {e}");
                }
            }

            this.StartConfig = this.Get(appId);
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();

            Instance = null;
        }

        public StartConfig Get(int id)
        {
            try
            {
                return this.configDict[id];
            }
            catch (Exception e)
            {
                throw new Exception($"not found startconfig: {id}", e);
            }
        }

        public IPEndPoint GetInnerAddress(int id)
        {
            try
            {
                return this.innerAddressDict[id];
            }
            catch (Exception e)
            {
                throw new Exception($"not found innerAddress: {id}", e);
            }
        }

        public StartConfig[] GetAll()
        {
            return this.configDict.Values.ToArray();
        }

        public int Count
        {
            get
            {
                return this.configDict.Count;
            }
        }
    }
}
