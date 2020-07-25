namespace SimpleDICOMToolkit
{
    public class Initializer
    {
        public static void Initialize()
        {
#if DEBUG
            Stylet.Logging.LogManager.Enabled = true;
#endif

            Dicom.Imaging.ImageManager.SetImplementation(Dicom.Imaging.WPFImageManager.Instance);
            Dicom.Log.LogManager.SetImplementation(Dicom.Log.NLogManager.Instance);
        }
    }
}
