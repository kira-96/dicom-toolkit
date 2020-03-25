using System;
using System.Linq;
using System.Threading;

namespace SimpleDICOMToolkit.Utils
{
    public static class ApplicationUtil
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

        /// <summary>
        /// 进程互斥
        /// 必须定义在类内部，一旦被释放就无效了
        /// </summary>
        private static Mutex mutex;

        public static bool CheckSingletonPattern(string mutexName, string[] args)
        {
            if (args == null || args.Length == 0 || !IsClientModeOnly(args[0]))
            {
                mutex = new Mutex(true, mutexName, out bool createNew);
                return createNew;
            }

            return true;
        }
    }
}
