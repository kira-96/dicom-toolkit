namespace SimpleDICOMToolkit.Services
{
    public interface IConfigurationService
    {
        /// <summary>
        /// 重新加载设置
        /// </summary>
        /// <param name="section"></param>
        void Load(string section = null);

        /// <summary>
        /// 获取当前设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <returns></returns>
        T GetConfiguration<T>(string section = null);
    }
}
