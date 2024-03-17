namespace Home.Common.Configuration
{
    public class ModbusTcpServerOptions
    {
        public const string ModbusTcpServer = "ModbusTcpServer";

        public string IpAddress { get; set; }

        public int Port { get; set; }

        public required IEnumerable<ReadingHoldingRegistersOptions> ReadingHoldingRegisters { get; set;}
    }
}
