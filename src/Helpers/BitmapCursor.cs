using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace SimpleDICOMToolkit.Helpers
{
    /// <summary>
    /// This class allow you create a Cursor form a Bitmap
    /// https://www.cnblogs.com/mqxs/p/9925203.html
    /// </summary>
    internal class BitmapCursor : SafeHandle
    {
        public override bool IsInvalid => handle == (IntPtr)(-1);

        public static Cursor CreateBmpCursor(Bitmap cursorBitmap)
        {
            var c = new BitmapCursor(cursorBitmap);
            return CursorInteropHelper.Create(c);
        }

        protected BitmapCursor(Bitmap cursorBitmap)
            : base((IntPtr)(-1), true)
        {
            handle = cursorBitmap.GetHicon();
        }

        protected override bool ReleaseHandle()
        {
            bool result = DestroyIcon(handle);

            handle = (IntPtr)(-1);

            return result;
        }

        [DllImport("user32")]
        private static extern bool DestroyIcon(IntPtr hIcon);
    }
}
