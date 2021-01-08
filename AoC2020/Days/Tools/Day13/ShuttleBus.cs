using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day13
{
    class ShuttleBus
    {
        public int ID { get; }
        public long Departures { get; set; }
        public int Offset { get; }

        public ShuttleBus(int id, int offset)
        {
            ID = id;
            Offset = offset;
            Departures = 0;
        }
    }
}
