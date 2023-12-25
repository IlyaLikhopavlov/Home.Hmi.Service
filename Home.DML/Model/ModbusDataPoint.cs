namespace Home.DML.Model
{
    public abstract class ModbusDataPoint
    {
        protected const float Factor = 0.1f;

        protected byte[] DataArray;

        protected ModbusDataPoint(string name, ushort order)
        {
            Name = name;
            Order = order;
            TimeStamp = DateTime.Now;
            DataArray = new byte[Constants.RegisterBytesCount];
        }

        public virtual byte[] Data
        {
            get => DataArray;
            set => DataArray = value;
        }

        public Action? ValueChanged;

        public int IntValue => ConstructInt(Data);

        public DateTime TimeStamp { get; protected set; }

        public string Name { get; }

        public ushort Order { get; }

        public static int ConstructInt(byte[] data)
        {
            if (data.Length > Constants.RegisterBytesCount)
            {
                throw new InvalidOperationException("Unexpected length of byte array");
            }

            var buffer = data.ToArray();
            Array.Reverse(buffer);

            return BitConverter.ToInt16(buffer);
        }

        public abstract string? FormattedValue { get; }
    }
}
