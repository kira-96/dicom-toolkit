using System.Windows;

namespace SimpleDICOMToolkit.Controls
{
    public class ToastMessageEventArgs : RoutedEventArgs
    {
        public ToastMessage Message;

        public ToastMessageEventArgs(ToastMessage message)
        {
            Message = message;
        }

        public ToastMessageEventArgs(RoutedEvent routedEvent, ToastMessage message) : base(routedEvent)
        {
            Message = message;
        }

        public ToastMessageEventArgs(RoutedEvent routedEvent, object source, ToastMessage message) : base(routedEvent, source)
        {
            Message = message;
        }
    }
}
