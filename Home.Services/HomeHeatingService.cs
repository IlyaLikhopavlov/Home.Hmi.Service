using Home.DML.Model;

namespace Home.Services
{
    public class HomeHeatingService
    {
        private readonly List<ModbusDataPoint> _dataPoints;

        public HomeHeatingService()
        {
            _dataPoints = new List<ModbusDataPoint>
            {
                new DataPoint<float>("Living Room Temperature (C)", 0),
                new DataPoint<float>("Bed room Temperature (C)", 2),
                new DataPoint<float>("Martin's room Temperature (C)", 4),
                new DataPoint<float>("Study 2nd floor Temperature (C)", 6),
                new DataPoint<bool>("Boiler control signal (On/Off)", 8),
                new DataPoint<float>("Outdoor Temperature (C)", 10),
            };

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
