using System.ComponentModel;
using System.Net;
using FluentModbus;
using Home.Common.Configuration;
using Home.DML.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Home.Services
{
    public class HomeHeatingService
    {
        private const int UnitIdentifier = 0xFF;
        
        private readonly Dictionary<string, ModbusDataPoint> _dataPoints;

        private readonly ModbusTcpClient _modbusTcpClient;

        private readonly ModbusTcpServerOptions _modbusTcpServerOptions;

        private readonly ILogger<PollModbusRegistresJob> _logger;

        public HomeHeatingService(
            IOptions<ModbusTcpServerOptions> modbusTcpServerOptions, 
            ModbusTcpClient modbusTcpClient, 
            ILogger<PollModbusRegistresJob> logger)
        {
            _modbusTcpClient = modbusTcpClient;
            _logger = logger;
            _modbusTcpServerOptions = modbusTcpServerOptions.Value;
            _dataPoints =
                modbusTcpServerOptions.Value.ReadingHoldingRegisters
                    .SelectMany(x => x.Registers, (set, register) => new { set, register })
                    .ToDictionary(
                        x => x.register.Id, 
                        x =>
                            (ModbusDataPoint)new TypedDataPoint(
                                x.register.Name,
                                x.register.Order,
                                x.register.Type,
                                x.register.Factor.GetValueOrDefault(),
                                x.set.StartingAddress,
                                x.register.Address,
                                x.register.Id));
            
            foreach (var modbusDataPoint in _dataPoints)
            {
                modbusDataPoint.Value.ValueChanged = OnAnyDataChanged;
            }

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

        public Action? AnyDataChanged;

        private void OnAnyDataChanged()
        {
            AnyDataChanged?.Invoke();
        }

        public IList<ModbusDataPoint> DataPoints => _dataPoints.Values.ToList();

        private void UpdateData(IEnumerable<byte> data, uint startingAddress)
        {
            var dataChunks = data.Chunk(Constants.RegisterBytesCount).ToArray();

            foreach (var modbusDataPoint in 
                     _dataPoints.Values.Where(x => x.StartingAddress == startingAddress))
            {
                modbusDataPoint.Data = dataChunks[modbusDataPoint.Order / Constants.RegisterBytesCount];
            }
        }

        public void UpdateDataPoints()
        {
            try
            {
                foreach (var registersChunk in _modbusTcpServerOptions.ReadingHoldingRegisters)
                {
                    var registersData = _modbusTcpClient
                        .ReadHoldingRegisters<byte>(
                            UnitIdentifier,
                            (int)registersChunk.StartingAddress,
                            registersChunk.Count).ToArray();

                    UpdateData(registersData, registersChunk.StartingAddress);
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reading data from Modbus client failed.");
            }
        }

        public void WriteSingleValue(float value, string id)
        {
            //short value = 220;
            //var result = BitConverter.GetBytes(value);

            //if (result.Length > Constants.RegisterBytesCount)
            //{
            //    throw new InvalidOperationException("Unexpected length of byte array");
            //}

            //Array.Reverse(result);


            //_modbusTcpClient.WriteSingleRegister(UnitIdentifier, 16384, result);
            if (!_dataPoints.TryGetValue(id, out var dataPoint))
            {
                return;
            }

            var typedDataPoint = (TypedDataPoint)dataPoint;
            var dataToWrite = typedDataPoint.GetDataToWrite(value);
            _modbusTcpClient.WriteSingleRegister(UnitIdentifier, (ushort)typedDataPoint.Address, dataToWrite);

            //if (_dataPoints.First(x => x.Name == name) is TypedDataPoint typedDataPoint)
            //{
            //    var dataToWrite = typedDataPoint.GetDataToWrite(value);
            //    typedDataPoint.
            //}


        }
    }
}
