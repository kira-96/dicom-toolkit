using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace SimpleDICOMToolkit.Controls
{
    public class EffectBehavior : Behavior<FrameworkElement>
    {
        public static DependencyProperty effectProperty =
            DependencyProperty.Register(nameof(Effect), typeof(Effect), typeof(EffectBehavior), new PropertyMetadata(default(Effect)));

        public Effect Effect
        {
            get => (Effect)GetValue(effectProperty);
            set => SetValue(effectProperty, value);
        }

        private Effect defaultEffect;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseEnter -= OnMouseEnter;
            AssociatedObject.MouseLeave -= OnMouseLeave;
        }

        private void OnMouseEnter(object s, MouseEventArgs e)
        {
            FrameworkElement element = s as FrameworkElement;
            defaultEffect = element.Effect;
            element.Effect = Effect;
        }

        private void OnMouseLeave(object s, MouseEventArgs e)
        {
            FrameworkElement element = s as FrameworkElement;
            element.Effect = defaultEffect;
        }
    }
}
