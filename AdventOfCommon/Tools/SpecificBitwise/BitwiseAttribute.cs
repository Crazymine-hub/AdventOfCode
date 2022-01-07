using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.SpecificBitwise
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    internal sealed class BitwiseHandlerAttribute : Attribute
    {
        public Type HandledType { get; }
        public int Priority { get; set; } = 0;
        
        public BitwiseHandlerAttribute(Type handledType)
        {
            HandledType = handledType;
        }
    }
}
