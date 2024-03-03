using System.ComponentModel;
using Home.Common.Configuration;
using Home.DML.Model;
using Microsoft.Extensions.Options;

namespace Home.Services
{
    public class HomeHeatingService
    {
        private readonly List<ModbusDataPoint> _dataPoints;

        public HomeHeatingService(IOptions<ModbusTcpServerOptions> modbusTcpServerOptions)
        {
            var a = modbusTcpServerOptions;
            
            _dataPoints =
                modbusTcpServerOptions.Value.ReadingHoldingRegisters.Registers
                    .Select(x => 
                        (ModbusDataPoint) new TypedDataPoint(x.Name, x.Order, TypedDataPoint.RecognizeType(x.Type), x.Factor.GetValueOrDefault()))
                    .ToList();
            
            foreach (var modbusDataPoint in _dataPoints)
            {
                modbusDataPoint.ValueChanged = OnAnyDataChanged;
            }
        }

        public Action? AnyDataChanged;

        private void OnAnyDataChanged()
        {
            AnyDataChanged?.Invoke();
        }

        public IList<ModbusDataPoint> DataPoints => _dataPoints;

        public void UpdateData(byte[] allData)
        {
            var dataChunks = allData.Chunk(Constants.RegisterBytesCount).ToArray();

            foreach (var modbusDataPoint in _dataPoints)
            {
                modbusDataPoint.Data = dataChunks[modbusDataPoint.Order / Constants.RegisterBytesCount];
            }
        }
    }
}
