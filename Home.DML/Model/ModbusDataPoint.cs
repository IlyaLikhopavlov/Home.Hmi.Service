using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Home.DML.Model
{
    public abstract class ModbusDataPoint
    {
        protected byte[] DataArray;

        protected ModbusDataPoint(string name, ushort order, uint startingAddress)
        {
            Name = name;
            Order = order;
            TimeStamp = DateTime.Now;
            DataArray = new byte[Constants.RegisterBytesCount];
            StartingAddress = startingAddress;
        }

        public virtual byte[] Data
        {
            get => DataArray;
            set => DataArray = value;
        }

        public Action? ValueChanged;

        public short IntValue
        {
            get => ConstructInt(Data);
            set => Data = DeconstructInt(value);
        }

        public DateTime TimeStamp { get; protected set; }

        public string Name { get; }

        public ushort Order { get; }

        public uint StartingAddress { get; }

        protected static short ConstructInt(byte[] data)
        {
            if (data.Length > Constants.RegisterBytesCount)
            {
                throw new InvalidOperationException("Unexpected length of byte array");
            }

            var buffer = data.ToArray();
            Array.Reverse(buffer);

            return BitConverter.ToInt16(buffer);
        }

        protected static byte[] DeconstructInt(short value)
        {
            var result = BitConverter.GetBytes(value);
            Array.Reverse(result);

            return result;
        }

        public abstract string? FormattedValue { get; }
    }
}
