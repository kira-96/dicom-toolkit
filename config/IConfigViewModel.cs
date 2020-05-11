using Nett;

namespace Config
{
    public interface IConfigViewModel : System.IDisposable
    {
        void LoadConfigs(TomlTable table);

        void SaveConfig();
    }
}
