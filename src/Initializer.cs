using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.NativeCodec;
using FellowOakDicom.Log;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace SimpleDICOMToolkit
{
    public class Initializer
    {
        public static void Initialize()
        {
#if DEBUG
            Stylet.Logging.LogManager.Enabled = true;
#endif

            IServiceCollection services = new ServiceCollection();
            services.AddFellowOakDicom()
                .AddTranscoderManager<NativeTranscoderManager>()
                .AddImageManager<WPFImageManager>()
                .AddLogManager<NLogManager>();

            DicomSetupBuilder.UseServiceProvider(services.BuildServiceProvider());

            // Register encoding provider
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Set your own Class UID here
            // DicomImplementation.ClassUID = new DicomUID("My Class UID", "Implementation Class UID", DicomUidType.Unknown);
            // DicomImplementation.Version = "My Version Name";
        }
    }
}
