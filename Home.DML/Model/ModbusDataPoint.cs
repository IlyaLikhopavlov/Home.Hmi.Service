namespace Home.DML.Model
{
    public abstract class ModbusDataPoint
    {
        protected byte[] DataArray;

        protected ModbusDataPoint(string name, ushort order, uint startingAddress, uint address, string id)
        {
            Name = name;
            Order = order;
            TimeStamp = DateTime.Now;
            DataArray = new byte[Constants.RegisterBytesCount];
            StartingAddress = startingAddress;
            Address = address;
            Id = id;
        }

        public virtual byte[] Data
        {
            get => DataArray;
            set => DataArray = value;
        }

        public Action? ValueChanged;

        public short IntValue
        {
            get => ConstructShort(Data);
            set => Data = DeconstructShort(value);
        }

        public DateTime TimeStamp { get; protected set; }

        public string Id { get; }
        
        public string Name { get; }

        public ushort Order { get; }

        public uint StartingAddress { get; }

        public uint Address { get; }

        protected static short ConstructShort(byte[] data)
        {
            if (data.Length > Constants.RegisterBytesCount)
            {
                throw new InvalidOperationException("Unexpected length of byte array");
            }

            var buffer = data.ToArray();
            Array.Reverse(buffer);

            return BitConverter.ToInt16(buffer);
        }

        protected static byte[] DeconstructShort(short value)
        {
            var result = BitConverter.GetBytes(value);
            Array.Reverse(result);

            return result;
        }

        public abstract string? FormattedValue { get; }
    }
}
