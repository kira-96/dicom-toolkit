#if FellowOakDicom5
using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.NativeCodec;
using FellowOakDicom.Log;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace SimpleDICOMToolkit
{
    public class Initializer
    {
        public static void Initialize()
        {
#if DEBUG
            Stylet.Logging.LogManager.Enabled = true;
#endif

#if FellowOakDicom5
            IServiceCollection services = new ServiceCollection();
            services.AddFellowOakDicom()
                .AddTranscoderManager<NativeTranscoderManager>()
                .AddImageManager<WPFImageManager>()
                .AddLogManager<NLogManager>();

            DicomSetupBuilder.UseServiceProvider(services.BuildServiceProvider());
#else
            Dicom.Imaging.ImageManager.SetImplementation(Dicom.Imaging.WPFImageManager.Instance);
            Dicom.Log.LogManager.SetImplementation(Dicom.Log.NLogManager.Instance);

            // Set your own Class UID here
            // Dicom.DicomImplementation.ClassUID = new Dicom.DicomUID("My Class UID", "Implementation Class UID", Dicom.DicomUidType.Unknown);
            // Dicom.DicomImplementation.Version = "My Version Name";
#endif
        }
    }
}
