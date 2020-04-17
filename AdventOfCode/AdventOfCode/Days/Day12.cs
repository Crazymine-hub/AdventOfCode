using AdventOfCode.Days.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    class Day12 : IDay
    {
        List<MoonInfo3D> Moons = new List<MoonInfo3D>();
        public string Solve(string input, bool part2)
        {

            /*input = "<x=-1, y=0, z=2>\r\n" +
                "<x=2, y=-10, z=-7>\r\n" +
                "<x=4, y=-8, z=8>\r\n" +
                "<x=3, y=5, z=-1>\r\n";*/

            /*input = "<x=-8, y=-10, z=0>\r\n" +
                "<x=5, y=5, z=10>\r\n" +
                "<x=2, y=-7, z=3>\r\n" +
                "<x=9, y=-8, z=-3>\r\n";*/

            MatchCollection moonValues = Regex.Matches(input, @"<x=(?<X>([-]?[0-9]+)), y=(?<Y>([-]?[0-9]+)), z=(?<Z>([-]?[0-9]+))>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            foreach (Match moonInfo in moonValues)
            {
                MoonInfo3D moon = new MoonInfo3D();
                moon.Position.X = int.Parse(moonInfo.Groups["X"].Value);
                moon.Position.Y = int.Parse(moonInfo.Groups["Y"].Value);
                moon.Position.Z = int.Parse(moonInfo.Groups["Z"].Value);
                Moons.Add(moon);
            }


            if (part2)
                return $"Repeating after {FindFirstRepetition()} steps.";
            else
            {
                int steps = Tools.ConsoleAssist.GetUserInput("Enter number of Iterations:");
                return Iterate(steps);
            }
        }

        private string Iterate(int steps)
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine($"After 0 steps:");
            foreach (MoonInfo3D moon in Moons)
                output.AppendLine(moon.ToString());
            output.AppendLine();

            for (int i = 0; i < steps; i++)
            {
                output.AppendLine($"After {i + 1} steps:");
                DoStep(output);
                output.AppendLine();
            }

            output.AppendLine($"Energy:");
            int energy = 0;
            foreach (MoonInfo3D moon in Moons)
            {
                output.AppendLine(moon.EnergyString());
                energy += moon.Energy;
            }

            output.AppendLine("TOTAL: " + energy);
            output.AppendLine();

            return output.ToString();
        }

        private long FindFirstRepetition()
        {
            MoonInfo3D[] startValues = new MoonInfo3D[Moons.Count];
            long xSteps = 0;
            long ySteps = 0;
            long zSteps = 0;
            long steps = 0;
            for(int i = 0; i < Moons.Count; i++)
                startValues[i] = Moons[i].Clone();

            while(xSteps == 0 || ySteps == 0 || zSteps == 0)
            {
                DoStep(null);
                steps++;
                int equality = 255;
                for (int i = 0; i < Moons.Count; i++)
                    equality = equality & Moons[i].findEquality(startValues[i]);
                if ((equality & 1) != 0 && xSteps == 0)
                    xSteps = steps;
                if ((equality & 2) != 0 && ySteps == 0)
                    ySteps = steps;
                if ((equality & 4) != 0 && zSteps == 0)
                    zSteps = steps;
            }

            return Tools.MathHelper.LeastCommonMultiple(new long[] { xSteps, ySteps, zSteps });
        }

        private void DoStep(StringBuilder output)
        {
                foreach (MoonInfo3D originMoon in Moons)
                {
                    foreach (MoonInfo3D gravityMoon in Moons)
                    {
                        originMoon.Velocity.X += Math.Sign(gravityMoon.Position.X - originMoon.Position.X);
                        originMoon.Velocity.Y += Math.Sign(gravityMoon.Position.Y - originMoon.Position.Y);
                        originMoon.Velocity.Z += Math.Sign(gravityMoon.Position.Z - originMoon.Position.Z);
                    }
                }

                foreach (MoonInfo3D moon in Moons)
                {
                    moon.ApplyVelocity();
                    output?.AppendLine(moon.ToString());
                }
        }
    }
}
