using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Common.Configuration
{
    public class ReadingHoldingRegistersOptions
    {
        public const string ReadingHoldingRegisters = nameof(ModbusTcpServerOptions.ReadingHoldingRegisters);//"ReadingHoldingRegisters";

        public int StartingAddress { get; set; }

        public int Count { get; set; }
    }
}
