﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Common.Configuration
{
    public class Register
    {
        public string Id { get; set; }
        
        public string Name { get; set; }

        public uint Address { get; set; }

        public ushort Order { get; set; }

        public string Type { get; set; }

        public float? Factor { get; set; }
    }
}
