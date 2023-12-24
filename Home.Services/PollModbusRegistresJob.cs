using System.Net;
using FluentModbus;
using Home.Common.Configuration;
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

        private readonly ModbusTcpServerOptions _modbusTcpServerOptions;

        public PollModbusRegistresJob(
            HomeHeatingService heatingService, 
            ModbusTcpClient modbusTcpClient, 
            IOptions<ModbusTcpServerOptions> options,
            ILogger<PollModbusRegistresJob> logger)
        {
            _logger = logger;
            _modbusTcpServerOptions = options.Value;

            _heatingService = heatingService;
            _modbusTcpClient = modbusTcpClient;

            if (_modbusTcpClient.IsConnected)
            {
                return;
            }

            try
            {
                _modbusTcpClient.Connect(
                    new IPEndPoint(IPAddress.Parse(_modbusTcpServerOptions.IpAddress), 
                        _modbusTcpServerOptions.Port));
                if (_modbusTcpClient.IsConnected)
                {
                    _logger.LogInformation($@"Successfully connected to the server {_modbusTcpServerOptions.IpAddress}");
                    return;
                }

                _logger.LogError($@"Connection to the server {_modbusTcpServerOptions.IpAddress} failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $@"Connection to the server {_modbusTcpServerOptions.IpAddress} failed");
            }
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var registersData = _modbusTcpClient
                    .ReadHoldingRegisters<byte>(
                        0xFF, 
                        _modbusTcpServerOptions.ReadingHoldingRegisters.StartingAddress, 
                        _modbusTcpServerOptions.ReadingHoldingRegisters.Count).ToArray();
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
