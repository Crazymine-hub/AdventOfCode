using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day18
{

    [Serializable]
    public class NumbersChangedException : Exception
    {
        public NumbersChangedException() { }
        public NumbersChangedException(string message) : base(message) { }
        public NumbersChangedException(string message, Exception inner) : base(message, inner) { }
        protected NumbersChangedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
