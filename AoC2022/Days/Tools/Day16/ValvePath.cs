using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day16
{
    internal class ValvePath
    {
        public List<ValveInfo> Path { get; } = new List<ValveInfo>();
        public int PressureReleased { get; set; } = 0;

        public void Add(ValveInfo valve, int addedRelease)
        {
            Path.Add(valve);
            PressureReleased+= addedRelease;
        }
    }
}
