namespace SimpleDICOMToolkit.Services
{
    public interface IConfigurationService
    {
        /// <summary>
        /// Token 用于避免重复读取配置文件
        /// </summary>
        string Token { get; }

        /// <summary>
        /// 重新加载设置
        /// </summary>
        void Load(string token);

        /// <summary>
        /// 获取设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <returns></returns>
        T Get<T>(string section = null);
    }
}
