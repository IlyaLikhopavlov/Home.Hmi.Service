using System.Net;
using FluentModbus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Home.Services
{
    public class PollModbusRegistresJob : IJob
    {
        private readonly HomeHeatingService _heatingService;

        private readonly ModbusTcpClient _modbusTcpClient;

        private readonly ILogger<PollModbusRegistresJob> _logger;

        private const string ServerAddress = @"192.168.0.101";

        public PollModbusRegistresJob(HomeHeatingService heatingService, ModbusTcpClient modbusTcpClient, ILogger<PollModbusRegistresJob> logger)
        {
            _logger = logger;

            _heatingService = heatingService;
            _modbusTcpClient = modbusTcpClient;

            if (_modbusTcpClient.IsConnected)
            {
                return;
            }

            try
            {
                _modbusTcpClient.Connect(IPAddress.Parse(ServerAddress));
                if (_modbusTcpClient.IsConnected)
                {
                    _logger.LogInformation($@"Successfully connected to the server {ServerAddress}");
                    return;
                }

                _logger.LogError($@"Connection to the server {ServerAddress} failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $@"Connection to the server {ServerAddress} failed");
            }
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var registersData = _modbusTcpClient.ReadHoldingRegisters<byte>(0xFF, 8959, 12).ToArray();
                _heatingService.UpdateData(registersData);

                _logger.LogDebug("Reading data from Modbus client succeed.");
                foreach (var heatingServiceDataPoint in _heatingService.DataPoints)
                {
                    _logger.LogDebug(message: $"{heatingServiceDataPoint.Name} = {heatingServiceDataPoint.FormattedValue}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reading data from Modbus client failed.");
            }

            return Task.FromResult(true);
        }
    }
}
