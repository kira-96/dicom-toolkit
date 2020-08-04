namespace SimpleDICOMToolkit.Services
{
    public interface IConfigurationService
    {
        void Load(string section = null);

        T GetConfiguration<T>(string section = null);
    }
}
