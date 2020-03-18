using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleDICOMToolkit.Controls
{
    public class TextBoxPlaceholderBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty placeholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(TextBoxPlaceholderBehavior), new PropertyMetadata(null));

        public string Placeholder
        {
            get => (string)GetValue(placeholderProperty);
            set => SetValue(placeholderProperty, value);
        }

        private bool hasContent = false;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {

            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.LostFocus -= OnLostFocus;
            base.OnDetaching();
        }

        private void OnLoaded(object s, RoutedEventArgs e)
        {
            TextBox textBox = s as TextBox;
            textBox.GotFocus += OnGotFocus;
            textBox.LostFocus += OnLostFocus;

            if (!string.IsNullOrEmpty(textBox.Text))
            {
                hasContent = true;
            }
            else
            {
                hasContent = false;

                if (!string.IsNullOrEmpty(Placeholder))
                {
                    textBox.Foreground = Brushes.Gray;
                    textBox.Text = Placeholder;
                }
            }
        }

        private void OnGotFocus(object s, RoutedEventArgs e)
        {
            TextBox textBox = s as TextBox;
            if (!string.IsNullOrEmpty(Placeholder) && !hasContent)
            {
                textBox.Foreground = Brushes.Black;
                textBox.Text = "";
            }
        }

        private void OnLostFocus(object s, RoutedEventArgs e)
        {
            TextBox textBox = s as TextBox;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                hasContent = false;
                if (!string.IsNullOrEmpty(Placeholder))
                {
                    textBox.Foreground = Brushes.Gray;
                    textBox.Text = Placeholder;
                }
            }
            else
            {
                hasContent = true;
            }
        }
    }
}
