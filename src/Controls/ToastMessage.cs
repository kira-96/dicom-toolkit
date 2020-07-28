using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace SimpleDICOMToolkit.Controls
{
    public enum ToastType
    {
        Info,
        Error
    }

    public class ToastMessageTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                return new ToastMessage()
                {
                    Content = s,
                };
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    [TypeConverter(typeof(ToastMessageTypeConverter))]
    public class ToastMessage : ContentControl
    {
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register(
            nameof(Level), typeof(ToastType), typeof(ToastMessage), new PropertyMetadata(ToastType.Info));

        public ToastType Level
        {
            get => (ToastType)GetValue(LevelProperty);
            set => SetValue(LevelProperty, value);
        }

        static ToastMessage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToastMessage), new FrameworkPropertyMetadata(typeof(ToastMessage)));
        }
    }
}
