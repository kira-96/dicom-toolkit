namespace SimpleDICOMToolkit.Services
{
    public interface IConfigurationService
    {
        string Token { get; }

        /// <summary>
        /// 重新加载设置
        /// </summary>
        void Load(string token);

        /// <summary>
        /// 获取当前设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <returns></returns>
        T GetConfiguration<T>(string section = null);
    }
}
