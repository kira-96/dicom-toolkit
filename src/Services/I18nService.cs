using System;
using System.Collections;
using System.Windows;
using System.Windows.Data;
using System.Xml;

namespace SimpleDICOMToolkit.Services
{
    public class I18nService : II18nService
    {
        public void ApplyXamlStringsResource(string resourceName)
        {
            //Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
            //{
            //    Source = new Uri(resourceName, UriKind.RelativeOrAbsolute)
            //};
        }

        public void ApplyXmlStringsResource(string resourceName)
        {
            if (!(Application.Current.TryFindResource("Strings") is XmlDataProvider provider))
                return;

            provider.Source = new Uri(resourceName, UriKind.RelativeOrAbsolute);

            provider.Refresh();
        }

        public void ApplyXmlStringsResourceByCode(string languageCode)
        {
            string uri = $"pack://application:,,,/Strings/{languageCode}.xml";
            ApplyXmlStringsResource(uri);
        }

        public string GetSystemLanguageCode()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        }

        public string GetXmlStringByKey(string key)
        {
            if (!(Application.Current.TryFindResource("Strings") is XmlDataProvider provider))
                return key;

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

            return key;
        }
    }
}
