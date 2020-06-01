namespace SimpleDICOMToolkit.Utils
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Data;
    using System.Xml;

    /// <summary>
    /// 动态加载语言资源
    /// </summary>
    public static class LanguageHelper
    {
        [Obsolete("Use LoadXmlStringsResource instead.")]
        public static void LoadXamlStringsResource(string resourceName)
        {
            Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
            {
                Source = new Uri(resourceName, UriKind.RelativeOrAbsolute)
            };
        }

        public static void LoadXmlStringsResource(string resourceName)
        {
            if (!(Application.Current.TryFindResource("Strings") is XmlDataProvider provider))
                return;

            provider.Source = new Uri(resourceName, UriKind.RelativeOrAbsolute);

            provider.Refresh();
        }

        public static void LoadXmlStringResourceByCode(string languageCode)
        {
            string uri = $"pack://application:,,,/Strings/{languageCode}.xml";
            LoadXmlStringsResource(uri);
        }

        public static string GetXmlStringByKey(string key)
        {
            if (!(Application.Current.TryFindResource("Strings") is XmlDataProvider provider))
                return string.Empty;

            IEnumerator enumerator = (provider.Data as IEnumerable).GetEnumerator();
            enumerator.MoveNext();

            XmlElement strings = enumerator.Current as XmlElement;

            foreach (var node in strings.ChildNodes)
            {
                if (node is XmlNode astring && astring.Attributes["key"].Value == key)
                {
                    return astring.FirstChild.Value;
                }
            }

            return string.Empty;
        }
    }
}
