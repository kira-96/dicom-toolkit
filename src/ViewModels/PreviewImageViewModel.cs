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
        public ImageOrientationViewModel ImageOrientationViewModel { get; private set; }

        private readonly DicomDataset Dataset;
        private DicomImage dicomImage;

        private Point previousPoint;
        private bool isMovingImage = false;
        private bool isUpdatingImage = false;

        private WriteableBitmap _imageSource;

        public WriteableBitmap ImageSource
        {
            get => _imageSource;
            private set => SetAndNotify(ref _imageSource, value);
        }

        public PreviewImageViewModel(string dcmfilepath)
        {
            if (!File.Exists(dcmfilepath))
                return;

            DicomFile dcmFile = DicomFile.Open(dcmfilepath);

            ImageOrientationViewModel = SimpleIoC.Get<ImageOrientationViewModel>();
            Dataset = dcmFile.Dataset;
        }

        public PreviewImageViewModel(DicomDataset dataset)
        {
            ImageOrientationViewModel = SimpleIoC.Get<ImageOrientationViewModel>();
            Dataset = dataset;
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            DisplayName = Dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty);

            dicomImage = new DicomImage(Dataset);

            RenderImage();

            Dataset.TryGetValues<double>(DicomTag.ImageOrientationPatient, out var orientation);

            ImageOrientationViewModel.UpdateOrientation(orientation);
        }

        public void OnMouseWheel(Image s, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.LeftShift))
            {
                ImageScaleTransform(s, e.GetPosition(s), e.Delta / 1000.0);
            }
            else
            {
                if (e.Delta == 0) return;
                ShowPrevOrNextFrame(e.Delta < 0);
            }
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

#pragma warning disable IDE0060
        public void OnMouseLUp(Image s, MouseButtonEventArgs e)
#pragma warning restore IDE0060
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

#pragma warning disable IDE0060
        public void OnMouseRUp(Image s, MouseButtonEventArgs e)
#pragma warning restore IDE0060
        {
            if (isMovingImage)
            {
                return;
            }

            isUpdatingImage = false;
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
            else if (isUpdatingImage && e.RightButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(s);

                double widthOffset = position.X - previousPoint.X;
                double centerOffset = position.Y - previousPoint.Y;

                previousPoint.X = position.X;
                previousPoint.Y = position.Y;

                TransformWidthAndCenter(widthOffset, centerOffset);
            }
        }

        private void ShowPrevOrNextFrame(bool isNext)
        {
            int currentFrame = dicomImage.CurrentFrame;
            int numberOfFrames = dicomImage.NumberOfFrames;

            int targetFrame = isNext ? currentFrame + 1 : currentFrame - 1;

            if (targetFrame < 0 || targetFrame >= numberOfFrames)
            {
                return;
            }

            RenderImage(targetFrame);
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

            if (st.ScaleX + scale <= 0.25 ||
                st.ScaleX + scale >= 2.5)
            {
                return;
            }

            st.CenterX = origin.X;
            st.CenterY = origin.Y;
            st.ScaleX += scale;
            st.ScaleY += scale;

            image.RenderTransform = transformGroup;
        }

        [Obsolete("Use ImageMoveTranform instead.")]
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

            dicomImage.WindowCenter += centerOffset;

            // https://dicom.innolitics.com/ciods/mr-image/voi-lut/00281050
            // Window Width (0028,1051) shall always be greater than or equal to 1.
            if (dicomImage.WindowWidth + widthOffset < 1)
            {
                dicomImage.WindowWidth = 1;
            }
            else
            {
                dicomImage.WindowWidth += widthOffset;
            }

            RenderImage();
        }

        private void RenderImage(int frame = 0)
        {
            if (dicomImage == null)
            {
                return;
            }

            using (IImage iimage = dicomImage.RenderImage(frame))
            {
                ImageSource = iimage.AsWriteableBitmap();
                ImageSource.Freeze();
            }
        }
    }
}
