namespace SimpleDICOMToolkit.Helpers
{
    using System.Windows;

    /// <summary>
    /// Binding Proxy
    /// https://thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/
    /// 在Style中没用！
    /// </summary>
    public class BindingProxy : Freezable
    {
        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        // Using a DependencyProperty as the backing store for Data.This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty = 
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }
}
