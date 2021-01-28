namespace SimpleDICOMToolkit.Infrastructure
{
    public class MiscConfiguration
    {
        /// <summary>
        /// MQTT 服务端口
        /// </summary>
        public int ListenPort { get; set; } = 9629;

        /// <summary>
        /// LiteDB connection string
        /// </summary>
        public string DbConnectionString { get; } = @"Filename=data.db;Password=ms123";

        /// <summary>
        /// Default Dicom encoding
        /// </summary>
        public string DicomEncoding { get; set; } = "utf-8";
    }
}
