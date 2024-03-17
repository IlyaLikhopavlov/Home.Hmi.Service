using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Common.Configuration
{
    public class ReadingHoldingRegistersOptions
    {
        public const string ReadingHoldingRegisters = nameof(ModbusTcpServerOptions.ReadingHoldingRegisters);

        public uint StartingAddress { get; set; }

        public int Count { get; set; }

        public required IEnumerable<Register> Registers { get; set; }
    }
}
