using System;

namespace SimpleDICOMToolkit.Services
{
    public interface II18nService
    {
        /// <summary>
        /// Apply xaml resource
        /// </summary>
        /// <param name="resourceName">resource uri</param>
        [Obsolete("Use LoadXmlStringsResource instead.")]
        void ApplyXamlStringsResource(string resourceName);

        /// <summary>
        /// Apply xml resource
        /// </summary>
        /// <param name="resourceName">resource uri</param>
        void ApplyXmlStringsResource(string resourceName);

        /// <summary>
        /// Apply xml resource by language code
        /// </summary>
        /// <param name="languageCode">language code</param>
        void ApplyXmlStringsResourceByCode(string languageCode);

        /// <summary>
        /// Get current system language code
        /// </summary>
        /// <returns>language code</returns>
        string GetSystemLanguageCode();

        /// <summary>
        /// Get text by key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>text</returns>
        string GetXmlStringByKey(string key);

        /// <summary>
        /// invoke on language changed
        /// </summary>
        event EventHandler LanguageChanged;
    }
}
