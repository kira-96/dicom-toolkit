namespace Config.Infrastructure
{
    public class MiscConfiguration
    {
        /// <summary>
        /// MQTT 服务端口
        /// </summary>
        public int ListenPort { get; set; } = 9629;

        /// <summary>
        /// Default Dicom encoding
        /// </summary>
        public string DicomEncoding { get; set; } = "utf-8";
    }
}
