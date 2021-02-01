#if FellowOakDicom5
using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.NativeCodec;
using FellowOakDicom.Log;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
#else
using Dicom;
using Dicom.Imaging;
using Dicom.Log;
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

            // Register encoding provider
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#else
            ImageManager.SetImplementation(WPFImageManager.Instance);
            LogManager.SetImplementation(NLogManager.Instance);
#endif

            // Set your own Class UID here
            // DicomImplementation.ClassUID = new DicomUID("My Class UID", "Implementation Class UID", DicomUidType.Unknown);
            // DicomImplementation.Version = "My Version Name";
        }
    }
}
