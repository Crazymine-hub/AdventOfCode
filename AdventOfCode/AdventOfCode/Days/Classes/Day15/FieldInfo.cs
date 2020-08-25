using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes.Day15
{
    class FieldInfo
    {
        public FieldFlag FieldType { get; set; } = FieldFlag.Unknown;
        public bool ConnectsNorth { get; set; }
        public bool ConnectsWest { get; set; }
        public bool ConnectsSouth { get; set; }
        public bool ConnectsEast { get; set; }
        public bool IsStart { get; set; }
        public bool IsOxygen { get; set; }
        public bool WasBacktracked { get; set; }

        public int GetPathFlag()
        {
            int result = 0;
            result |= ConnectsNorth ? 1 : 0;
            result |= ConnectsSouth ? 2 : 0;
            result |= ConnectsWest ? 4 : 0;
            result |= ConnectsEast ? 8 : 0;
            result |= WasBacktracked ? 16 : 0;
            return result;
        }
    }
}
