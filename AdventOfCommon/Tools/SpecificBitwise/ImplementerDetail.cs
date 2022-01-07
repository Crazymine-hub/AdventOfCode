using System;
using System.Collections.Generic;

namespace AdventOfCode.Tools.SpecificBitwise
{
    internal struct ImplementerDetail
    {
        public Type Implementer;
        public int Priority;

        public ImplementerDetail(Type implementer, int priority)
        {
            Implementer = implementer;
            Priority = priority;
        }
    }
}
