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

            // Set your own Class UID here
            // Dicom.DicomImplementation.ClassUID = new Dicom.DicomUID("My Class UID", "Implementation Class UID", Dicom.DicomUidType.Unknown);
            // Dicom.DicomImplementation.Version = "My Version Name";
        }
    }
}
