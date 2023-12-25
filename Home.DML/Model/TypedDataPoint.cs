using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Home.DML.Model
{
    public class TypedDataPoint : ModbusDataPoint
    {
        private static readonly Dictionary<Type, Func<int, string>> Converter = 
            new()
            {
                { typeof(bool), s => (s != 0).ToString() },
                { typeof(byte), 
                    s => s switch
                    {
                        > byte.MaxValue => $"ERROR. {nameof(Byte)} upper limit exceeded.",
                        < byte.MinValue => $"ERROR. {nameof(Byte)} lower limit exceeded.",
                        _ => ((byte) s).ToString()
                    }
                },
                { typeof(sbyte), 
                    s => s switch
                    {
                        > sbyte.MaxValue => $"ERROR. {nameof(SByte)} upper limit exceeded.",
                        < sbyte.MinValue => $"ERROR. {nameof(SByte)} lower limit exceeded.",
                        _ => ((sbyte) s).ToString()
                    }
                },
                { typeof(short),
                    s => s switch
                    {
                        > short.MaxValue => $"ERROR. {nameof(Int16)} upper limit exceeded.",
                        < short.MinValue => $"ERROR. {nameof(Int16)} lower limit exceeded.",
                        _ => ((short) s).ToString()
                    }
                },
                { typeof(ushort),
                    s => s switch
                    {
                        > ushort.MaxValue => $"ERROR. {nameof(Int32)} upper limit exceeded.",
                        < ushort.MinValue => $"ERROR. {nameof(Int32)} lower limit exceeded.",
                        _ => ((ushort) s).ToString()
                    }
                },
                { typeof(int), s => s.ToString() },
                { typeof(uint),
                    s => s switch
                    {
                        < 0 => $"ERROR. {nameof(UInt32)} lower limit exceeded.",
                        _ => ((uint) s).ToString()
                    }
                },
                { typeof(long), s => ((long) s).ToString() },
                {
                    typeof(ulong), 
                    s => s switch
                    {
                        < 0 => $"ERROR. {nameof(UInt64)} lower limit exceeded.",
                        _ => ((ulong) s).ToString()
                    }
                },
                { typeof(float), s => $"{s * Factor:0.00}" },
                { typeof(double), s => $"{s * Factor:0.00}" },
                { typeof(decimal), s => $"{s * Factor:0.00}" }
            };

        public Type Type { get; }

        public TypedDataPoint(string name, ushort order, Type type) : base(name, order)
        {
            Type = type;
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

        public override string? FormattedValue => 
            Converter.TryGetValue(Type, out var converter) 
                ? $"ERROR. Unsupported type {Type.Name}" 
                : converter?.Invoke(IntValue);
    }
}
