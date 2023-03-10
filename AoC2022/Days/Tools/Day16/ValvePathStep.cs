using System.Collections.Generic;

namespace AdventOfCode.Days.Tools.Day16
{
    internal class ValvePathStep
    {
        public ValveInfo Valve { get; }
        public bool Opened { get; set; }

        public ValvePathStep(ValveInfo valve, bool opened)
        {
            this.Valve = valve;
            this.Opened = opened;
        }

        public override string ToString() => $"{Valve.Name} O:{Opened}";
    }
}
