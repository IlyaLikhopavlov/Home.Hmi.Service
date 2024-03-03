namespace Home.DML.Model
{
    public class TypedDataPoint : ModbusDataPoint
    {
        private static readonly Dictionary<Type, Func<int, float, string>> Converter = 
            new()
            {
                { typeof(bool), (s, f) => (s != 0).ToString() },
                { typeof(byte), 
                    (s, f) => s switch
                    {
                        > byte.MaxValue => $"ERROR. {nameof(Byte)} upper limit exceeded.",
                        < byte.MinValue => $"ERROR. {nameof(Byte)} lower limit exceeded.",
                        _ => ((byte) s).ToString()
                    }
                },
                { typeof(sbyte),
                    (s, f) => s switch
                    {
                        > sbyte.MaxValue => $"ERROR. {nameof(SByte)} upper limit exceeded.",
                        < sbyte.MinValue => $"ERROR. {nameof(SByte)} lower limit exceeded.",
                        _ => ((sbyte) s).ToString()
                    }
                },
                { typeof(short),
                    (s, f) => s switch
                    {
                        > short.MaxValue => $"ERROR. {nameof(Int16)} upper limit exceeded.",
                        < short.MinValue => $"ERROR. {nameof(Int16)} lower limit exceeded.",
                        _ => ((short) s).ToString()
                    }
                },
                { typeof(ushort),
                    (s, f) => s switch
                    {
                        > ushort.MaxValue => $"ERROR. {nameof(Int32)} upper limit exceeded.",
                        < ushort.MinValue => $"ERROR. {nameof(Int32)} lower limit exceeded.",
                        _ => ((ushort) s).ToString()
                    }
                },
                { typeof(int), (s, f) => s.ToString() },
                { typeof(uint),
                    (s, f) => s switch
                    {
                        < 0 => $"ERROR. {nameof(UInt32)} lower limit exceeded.",
                        _ => ((uint) s).ToString()
                    }
                },
                { typeof(long), (s, f) => ((long) s).ToString() },
                {
                    typeof(ulong),
                    (s, f) => s switch
                    {
                        < 0 => $"ERROR. {nameof(UInt64)} lower limit exceeded.",
                        _ => ((ulong) s).ToString()
                    }
                },
                { typeof(float), (s, f) => $"{s * f:0.00}" },
                { typeof(double), (s, f) => $"{s * f:0.00}" },
                { typeof(decimal), (s, f) => $"{s * f:0.00}" }
            };

        private static readonly Dictionary<string, Type> StringToTypeCorrespondence = Converter.Keys.ToDictionary(x => x.Name, x => x);

        private static Type RecognizeType(string typeName)
        {
            if (!StringToTypeCorrespondence.TryGetValue(typeName, out var type))
            {
                throw new ArgumentOutOfRangeException($"Type \"{typeName}\" not recognized");
            }

            return type;
        }

        public Type Type { get; }

        public float? Factor { get; }

        public TypedDataPoint(string name, ushort order, string type, float factor) : base(name, order)
        {
            Type = RecognizeType(type);
            Factor = factor;
        }

        public override byte[] Data
        {
            get => DataArray;
            set
            {
                var newValue = ConstructInt(value);
                var oldValue = IntValue;

                var isChanged = Math.Abs(newValue - oldValue) > Constants.IntAperture;

                if (!isChanged)
                {
                    return;
                }

                DataArray = value.ToArray();
                TimeStamp = DateTime.Now;
                ValueChanged?.Invoke();
            }
        }

        public override string FormattedValue => 
            Converter.TryGetValue(Type, out var converter) 
                ? converter.Invoke(IntValue, Factor ?? 1) 
                : $"ERROR. Unsupported type {Type.Name}";
    }
}
