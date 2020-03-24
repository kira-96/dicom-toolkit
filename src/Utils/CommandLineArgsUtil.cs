using System;
using System.Linq;

namespace SimpleDICOMToolkit.Utils
{
    public static class CommandLineArgsUtil
    {
        public static string[] ClientModeArgs = new[] { "-c", "/c", "--client" };
        public static string[] ServerModeArgs = new[] { "-s", "/s", "--server" };

        /// <summary>
        /// 获取命令行参数
        /// </summary>
        public static string[] CommandLineArgs => Environment.GetCommandLineArgs();

        /// <summary>
        /// 是否为客户端模式
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool IsClientModeOnly(string arg)
        {
            foreach (string item in ClientModeArgs)
            {
                if (item == arg) return true;
            }

            return false;
        }

        /// <summary>
        /// 是否为服务端模式
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool IsServerModeOnly(string arg)
        {
            return ServerModeArgs.Contains(arg);
        }
    }
}
