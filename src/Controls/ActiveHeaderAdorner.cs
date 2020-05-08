using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace SimpleDICOMToolkit.Controls
{
    public class ActiveHeaderAdorner : Adorner
    {
        public ActiveHeaderAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            SolidColorBrush renderBrush = new SolidColorBrush(Color.FromRgb(0xf0, 0x5b, 0x72)) { Opacity = 0.8 };
            Pen renderPen = new Pen(renderBrush, 0);

            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, 6, 6);
        }
    }
}
