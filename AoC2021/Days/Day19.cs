using AdventOfCode.Days.Tools.Day19;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day19 : DayBase
    {
        public override string Title => "Beacon Scanner";

        List<SensorData> sensors = new List<SensorData>();

        public override string Solve(string input, bool part2)
        {
            foreach(var lineGroup in GetGroupedLines(input))
            {
                sensors.Add(new SensorData(GetLines(lineGroup).Skip(1)));
            }
            return "";
        }
    }
}
