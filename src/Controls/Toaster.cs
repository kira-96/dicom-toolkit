using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SimpleDICOMToolkit.Controls
{
    [ContentProperty(nameof(Message))]
    public class Toaster : Control
    {
        private const string ActivateStoryboardName = "ActivateStoryboard";
        private const string DeactivateStoryboardName = "DeactivateStoryboard";

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message), typeof(ToastMessage), typeof(Toaster), new PropertyMetadata(default(ToastMessage)));

        public ToastMessage Message
        {
            get => (ToastMessage)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            nameof(IsActive), typeof(bool), typeof(Toaster), new PropertyMetadata(default(bool), IsActivePropertyChangedCallback));

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public static readonly RoutedEvent IsActiveChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(IsActiveChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(Toaster));

        public event RoutedPropertyChangedEventHandler<bool> IsActiveChanged
        {
            add => AddHandler(IsActiveChangedEvent, value);
            remove => RemoveHandler(IsActiveChangedEvent, value);
        }

        public static readonly RoutedEvent DeactivateStoryboardCompletedEvent = EventManager.RegisterRoutedEvent(
            nameof(DeactivateStoryboardCompleted), RoutingStrategy.Bubble, typeof(ToastMessageEventArgs), typeof(Toaster));

        public event RoutedPropertyChangedEventHandler<ToastMessage> DeactivateStoryboardCompleted
        {
            add => AddHandler(DeactivateStoryboardCompletedEvent, value);
            remove => RemoveHandler(DeactivateStoryboardCompletedEvent, value);
        }

        public TimeSpan ActivateStoryboardDuration { get; private set; }
        public TimeSpan DeactivateStoryboardDuration { get; private set; }

        private static void IsActivePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toaster = d as Toaster;
            var args = new RoutedPropertyChangedEventArgs<bool>((bool)e.OldValue, (bool)e.NewValue)
            { RoutedEvent = IsActiveChangedEvent };

            toaster.RaiseEvent(args);

            if ((bool)e.NewValue) return;

            if (toaster.Message == null) return;

            var dispatcherTime = new DispatcherTimer()
            {
                Tag = new Tuple<Toaster, ToastMessage>(toaster, toaster.Message),
                Interval = toaster.DeactivateStoryboardDuration
            };
            dispatcherTime.Tick += DeactivateStoryboardDispatcherTimerOnTick;
            dispatcherTime.Start();
        }

        static Toaster()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Toaster), new FrameworkPropertyMetadata(typeof(Toaster)));
        }

        public override void OnApplyTemplate()
        {
            ActivateStoryboardDuration = GetStoryboardResourceDuration(ActivateStoryboardName);
            DeactivateStoryboardDuration = GetStoryboardResourceDuration(DeactivateStoryboardName);

            base.OnApplyTemplate();
        }

        private static void DeactivateStoryboardDispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            var dispatcherTimer = (DispatcherTimer)sender;
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= DeactivateStoryboardDispatcherTimerOnTick;
            var source = (Tuple<Toaster, ToastMessage>)dispatcherTimer.Tag;
            OnDeactivateStoryboardCompleted(source.Item1, source.Item2);
        }

        private static void OnDeactivateStoryboardCompleted(IInputElement toaster, ToastMessage message)
        {
            var args = new ToastMessageEventArgs(DeactivateStoryboardCompletedEvent, message);
            toaster.RaiseEvent(args);
        }

        private TimeSpan GetStoryboardResourceDuration(string resourceName)
        {
            var storyboard = Template.Resources.Contains(resourceName)
                ? (Storyboard)Template.Resources[resourceName]
                : null;

            return storyboard != null && storyboard.Duration.HasTimeSpan
                ? storyboard.Duration.TimeSpan
                : new Func<TimeSpan>(() =>
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Warning, no Duration was specified at root of storyboard '{resourceName}'.");
                    return TimeSpan.Zero;
                })();
        }
    }
}
