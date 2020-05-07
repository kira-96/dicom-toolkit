namespace SimpleDICOMToolkit.ViewModels
{
    using Dicom;
    using Dicom.Imaging;
    using Stylet;
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class PreviewImageViewModel : Screen
    {
        private WriteableBitmap _imageSource;

        public WriteableBitmap ImageSource
        {
            get => _imageSource;
            private set => SetAndNotify(ref _imageSource, value);
        }

        private readonly DicomImage dicomImage;

        private Point previousPoint;
        private bool isMovingImage = false;
        private bool isUpdatingImage = false;

        public PreviewImageViewModel(string dcmfilepath)
        {
            if (!File.Exists(dcmfilepath))
                return;

            DicomFile dcmFile = DicomFile.Open(dcmfilepath);

            DisplayName = dcmFile.Dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty);

            dicomImage = new DicomImage(dcmFile.Dataset)
            {
                OverlayColor = unchecked((int)0xFFF05B72)  // 自定义Overlay的颜色
            };

            RenderImage();
        }

        public PreviewImageViewModel(DicomDataset dataset)
        {
            DisplayName = dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty);

            dicomImage = new DicomImage(dataset)
            {
                OverlayColor = unchecked((int)0xFFF05B72)  // 自定义Overlay的颜色
            };

            RenderImage();
        }

        public void OnMouseWheel(Image s, MouseWheelEventArgs e)
        {
            ImageScaleTransform(s, e.GetPosition(s), e.Delta / 300.0);
        }

        public void OnMouseLDown(Image s, MouseButtonEventArgs e)
        {
            if (isUpdatingImage)
            {
                return;
            }

            isMovingImage = true;
            previousPoint = e.GetPosition(s);
        }

        public void OnMouseLUp(Image s, MouseButtonEventArgs e)
        {
            if (isUpdatingImage)
            {
                return;
            }

            isMovingImage = false;
        }

        public void OnMouseRDown(Image s, MouseButtonEventArgs e)
        {
            if (isMovingImage)
            {
                return;
            }

            isUpdatingImage = true;
            previousPoint = e.GetPosition(s);
        }

        public void OnMouseRUp(Image s, MouseButtonEventArgs e)
        {
            if (isMovingImage)
            {
                return;
            }

            if (isUpdatingImage)
            {
                Point position = e.GetPosition(s);

                double widthOffset = position.X - previousPoint.X;
                double centerOffset = position.Y - previousPoint.Y;

                TransformWidthAndCenter(widthOffset, centerOffset);

                isUpdatingImage = false;
            }
        }

        public void OnMouseMove(Image s, MouseEventArgs e)
        {
            if (isMovingImage && e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(s);

                double offsetX = position.X - previousPoint.X;
                double offsetY = position.Y - previousPoint.Y;

                ImageMoveTranform(s, offsetX, offsetY);
            }
        }

        private void ImageMoveTranform(Image image, double offsetX, double offsetY)
        {
            TransformGroup transformGroup = (image.RenderTransform as TransformGroup).CloneCurrentValue();
            TranslateTransform tt = transformGroup.Children[0] as TranslateTransform;

            tt.X += offsetX;
            tt.Y += offsetY;

            image.RenderTransform = transformGroup;
        }

        private void ImageScaleTransform(Image image, Point origin, double scale)
        {
            TransformGroup transformGroup = (image.RenderTransform as TransformGroup).CloneCurrentValue();

            ScaleTransform st = transformGroup.Children[1] as ScaleTransform;

            if (st.ScaleX + scale <= 0 ||
                st.ScaleY + scale <= 0)
            {
                return;
            }

            st.CenterX = origin.X;
            st.CenterY = origin.Y;
            st.ScaleX += scale;
            st.ScaleY += scale;

            image.RenderTransform = transformGroup;
        }

        private void MoveImage(Image image, double offsetX, double offsetY)
        {
            double left = image.Margin.Left + offsetX;
            double top = image.Margin.Top + offsetY;
            double right = image.Margin.Right - offsetX;
            double bottom = image.Margin.Bottom - offsetY;

            image.Margin = new Thickness(left, top, right, bottom);
        }

        private void TransformWidthAndCenter(double widthOffset, double centerOffset)
        {
            if (dicomImage == null)
            {
                return;
            }

            dicomImage.WindowWidth += widthOffset;
            dicomImage.WindowCenter += centerOffset;

            RenderImage();
        }

        private void RenderImage()
        {
            if (dicomImage == null)
            {
                return;
            }

            using (IImage iimage = dicomImage.RenderImage())
            {
                ImageSource = iimage.AsWriteableBitmap();
            }
        }
    }
}
