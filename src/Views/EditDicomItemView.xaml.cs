using System;
using System.Windows;
using System.Windows.Media;

namespace SimpleDICOMToolkit.Views
{
    /// <summary>
    /// EditDicomItemView.xaml 的交互逻辑
    /// </summary>
    public partial class EditDicomItemView
    {
        public EditDicomItemView()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void Window_Activated(object s, EventArgs e)
        {
            ApplyTheme();
        }

        private void Window_Deactivated(object s, EventArgs e)
        {
            ApplyTheme();
        }

        private ResourceDictionary CommonResources
        {
            get
            {
                return Application.Current.Resources.MergedDictionaries[2].MergedDictionaries[1];
            }
        }

        private void ApplyTheme()
        {
            if (IsActive)
            {
                Resources["ButtonBackground"] = new SolidColorBrush((Color)Application.Current.Resources["AccentColor"]);
                Resources["ButtonForeground"] = new SolidColorBrush((Color)Application.Current.Resources["AccentForegroundColor"]);
            }
            else
            {
                Resources["ButtonBackground"] = new SolidColorBrush((Color)CommonResources["NonactiveControlBackgroundColor"]);
                Resources["ButtonForeground"] = new SolidColorBrush((Color)CommonResources["NonactiveControlForegroundColor"]);
            }
        }
    }
}
