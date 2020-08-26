namespace SimpleDICOMToolkit
{
    public class AppConfiguration
    {
        // MQTT 服务端口
        public int ListenPort { get; set; } = 9629;

        // LiteDB connection string
        public string DbConnectionString { get; set; } = @"Filename=data.db;Password=ms123";
    }
}
