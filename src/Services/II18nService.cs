using System;

namespace SimpleDICOMToolkit.Services
{
    public interface II18nService
    {
        [Obsolete("Use LoadXmlStringsResource instead.")]
        void ApplyXamlStringsResource(string resourceName);

        void ApplyXmlStringsResource(string resourceName);

        void ApplyXmlStringsResourceByCode(string languageCode);

        string GetSystemLanguageCode();

        string GetXmlStringByKey(string key);

        event EventHandler LanguageChanged;
    }
}
