using System;
using System.Collections.Generic;
using System.Text;

namespace TinyService.ReactiveRabbit.Brocker
{
    public class PayloadMessage
    {
        
        public ReadOnlyMemory<byte> Body { get; set; }

    }
}
