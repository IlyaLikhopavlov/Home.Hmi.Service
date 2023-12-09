namespace Home.DML.Model
{
    public class DataPoint<T> : ModbusDataPoint
    {
        private const float Factor = 0.1f;

        public DataPoint(string name, ushort order) : base(name, order)
        {
        }

        private static bool IsFraction() => new[] { typeof(float), typeof(double) }.Any(t => t == typeof(T));

        public T Value => (T)Convert.ChangeType(IntValue * (IsFraction() ? Factor : 1.0f), typeof(T));
            
        public override byte[] Data
        {
            get => DataArray;
            set
            {
                var newValue = ConstructInt(value);
                var oldValue = IntValue;

                var isChanged =
                    IsFraction()
                        ? Math.Abs(newValue * Factor - oldValue * Factor) > Constants.FloatAperture
                        : typeof(T) == typeof(bool) && newValue != oldValue;

                if (!isChanged)
                {
                    return;
                }

                DataArray = value.ToArray();
                TimeStamp = DateTime.Now;
                ValueChanged?.Invoke();
            }
        }

        public override string? FormattedValue => IsFraction() ? $"{Value:0.00}" : Value?.ToString();
    }
}
