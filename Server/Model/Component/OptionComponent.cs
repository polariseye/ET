using System;
using CommandLine;

namespace ETModel
{
    /// <summary>
    /// 程序运行的命令行参数解析加载事件
    /// </summary>
	[ObjectSystem]
    public class OptionComponentSystem : AwakeSystem<OptionComponent, string[]>
    {
        public override void Awake(OptionComponent self, string[] a)
        {
            self.Awake(a);
        }
    }

    /// <summary>
    /// 程序运行的命令行参数解析组件
    /// </summary>
    public class OptionComponent : Component
    {
        /// <summary>
        /// 解析后的命令行参数信息
        /// </summary>
        public Options Options { get; set; }

        /// <summary>
        /// 加载函数
        /// </summary>
        /// <param name="args"></param>
        public void Awake(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithNotParsed(error => throw new Exception($"命令行格式错误!"))
                .WithParsed(options => { Options = options; });
        }
    }
}
