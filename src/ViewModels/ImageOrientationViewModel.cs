#if FellowOakDicom5
using FellowOakDicom.Imaging.Mathematics;
#else
using Dicom.Imaging.Mathematics;
#endif
using Stylet;
using System;
using System.Windows;

namespace SimpleDICOMToolkit.ViewModels
{
    public class ImageOrientationViewModel : Screen
    {
        private Visibility orientationVisibility = Visibility.Hidden;

        public Visibility OrientationVisibility
        {
            get => orientationVisibility;
            private set => SetAndNotify(ref orientationVisibility, value);
        }

        private string leftMajor;
        private string leftMinor;
        private string topMajor;
        private string topMinor;
        private string rightMajor;
        private string rightMinor;
        private string bottomMajor;
        private string bottomMinor;

        public string LeftMajor
        {
            get => leftMajor;
            private set => SetAndNotify(ref leftMajor, value);
        }

        public string LeftMinor
        {
            get => leftMinor;
            private set => SetAndNotify(ref leftMinor, value);
        }

        public string TopMajor
        {
            get => topMajor;
            private set => SetAndNotify(ref topMajor, value);
        }

        public string TopMinor
        {
            get => topMinor;
            private set => SetAndNotify(ref topMinor, value);
        }

        public string RightMajor
        {
            get => rightMajor;
            private set => SetAndNotify(ref rightMajor, value);
        }

        public string RightMinor
        {
            get => rightMinor;
            private set => SetAndNotify(ref rightMinor, value);
        }

        public string BottomMajor
        {
            get => bottomMajor;
            private set => SetAndNotify(ref bottomMajor, value);
        }

        public string BottomMinor
        {
            get => bottomMinor;
            private set => SetAndNotify(ref bottomMinor, value);
        }

        public void UpdateOrientation(double[] orientation, int rotation = 0, bool flipX = false)
        {
            if (orientation == null || orientation.Length != 6)
            {
                return;
            }

            if (rotation != 0 || flipX)
            {
                orientation = ApplyRotationAndFlip(orientation, rotation, flipX);
            }

            (LeftMajor, LeftMinor) = ComputeOrientation(new Vector3D(-orientation[0], -orientation[1], -orientation[2]));
            (TopMajor, TopMinor) = ComputeOrientation(new Vector3D(-orientation[3], -orientation[4], -orientation[5]));
            (RightMajor, RightMinor) = ComputeOrientation(new Vector3D(orientation[0], orientation[1], orientation[2]));
            (BottomMajor, BottomMinor) = ComputeOrientation(new Vector3D(orientation[3], orientation[4], orientation[5]));
            OrientationVisibility = Visibility.Visible;
        }

        public static double[] ApplyRotationAndFlip(double[] orientation, int rotation, bool flipX)
        {
            Vector3D forward = new Vector3D(orientation[0], orientation[1], orientation[2]);
            Vector3D down = new Vector3D(orientation[3], orientation[4], orientation[5]);

            Orientation3D orientation3D = new Orientation3D(forward, down);

            if (rotation != 0)
            {
                orientation3D.Pitch(rotation * Math.PI / 180);
            }

            return new double[]
            {
                flipX ? -orientation3D.Forward.X : orientation3D.Forward.X,
                flipX ? -orientation3D.Forward.Y : orientation3D.Forward.Y,
                flipX ? -orientation3D.Forward.Z : orientation3D.Forward.Z,
                orientation3D.Down.X,
                orientation3D.Down.Y,
                orientation3D.Down.Z,
            };
        }

        public static (string major, string minor) ComputeOrientation(Vector3D vector)
        {
            char x = vector.X < 0 ? 'R' : 'L';
            char y = vector.Y < 0 ? 'A' : 'P';
            char z = vector.Z < 0 ? 'F' : 'H';

            double x1 = Math.Abs(vector.X);
            double y1 = Math.Abs(vector.Y);
            double z1 = Math.Abs(vector.Z);

            string result = "";

            for (int i = 0; i < 3; i++)
            {
                if (x1 > 0.0001 && x1 > y1 && x1 > z1)
                {
                    result += x;
                    x1 = 0;
                }
                else if (y1 > 0.0001 && y1 > x1 && y1 > z1)
                {
                    result += y;
                    y1 = 0;
                }
                else if (z1 > 0.0001 && z1 > x1 && z1 > y1)
                {
                    result += z;
                    z1 = 0;
                }
                else
                {
                    break;
                }
            }

            if (result.Length == 0)
            {
                return ("", "");
            }
            else
            {
                return (result.Substring(0, 1), result.Substring(1));
            }
        }
    }
}
