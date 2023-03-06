using System.Collections.Generic;

namespace AdventOfCode.Days.Tools.Day16
{
    internal struct ValvePathStep
    {
        public ValveInfo Valve { get; }
        public bool Opened { get; }

        public ValvePathStep(ValveInfo valve, bool opened)
        {
            this.Valve = valve;
            this.Opened = opened;
        }

        public override bool Equals(object obj)
        {
            return obj is ValvePathStep other &&
                   EqualityComparer<ValveInfo>.Default.Equals(Valve, other.Valve) &&
                   Opened == other.Opened;
        }

        public override int GetHashCode()
        {
            int hashCode = 258834889;
            hashCode = hashCode * -1521134295 + EqualityComparer<ValveInfo>.Default.GetHashCode(Valve);
            hashCode = hashCode * -1521134295 + Opened.GetHashCode();
            return hashCode;
        }

        public void Deconstruct(out ValveInfo valve, out bool opened)
        {
            valve = this.Valve;
            opened = this.Opened;
        }

        public static implicit operator (ValveInfo valve, bool opened)(ValvePathStep value)
        {
            return (value.Valve, value.Opened);
        }

        public static implicit operator ValvePathStep((ValveInfo valve, bool opened) value)
        {
            return new ValvePathStep(value.valve, value.opened);
        }

        public override string ToString() => $"{Valve.Name} O:{Opened}";
    }
}
