using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day1 : DayBase
    {
        public override string Solve(string input, bool part2)
        {
            return GetMassFuel(input.Split('\n'), part2).ToString();
        }

        int GetMassFuel(string[] values, bool addFuel)
        {
            int fuel = 0;
            foreach (string value in values)
            {
                if (!int.TryParse(value.Trim('#'), out int mass))
                    throw new Exception("Ungültiger Wert: '" + value + "'");
                fuel += addFuel ? GetFuelFuel(mass / 3 - 2) : mass / 3 - 2;
            }
            return fuel;
        }

        int GetFuelFuel(int fuel)
        {
            if (fuel <= 0) return 0;
            return fuel + GetFuelFuel(fuel / 3 - 2);
        }
    }
}
