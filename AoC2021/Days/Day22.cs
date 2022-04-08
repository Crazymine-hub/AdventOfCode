using AdventOfCode.Days.Tools.Day22;
using AdventOfCode.Tools.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day22 : DayBase
    {
        public override string Title => "Reactor Reboot";

        HashSet<CubeArea> cuboids = new HashSet<CubeArea>();
        private bool limitRange;
        private readonly CubeArea rangeLimit = new CubeArea(new Point3(-50, -50, -50), new Point3(50, 50, 50));

        public override string Solve(string input, bool part2)
        {
            limitRange = !part2;
            foreach (string instruction in GetLines(input))
            {
                Console.Write("Processing instruction ");
                Console.WriteLine(instruction);
                var parseInfo = Regex.Match(instruction, @"(?<state>on|off) (?:(?:x|y|z)=(?<start>-?\d+)..(?<end>-?\d+),?)+");
                if (!parseInfo.Success) throw new InvalidOperationException("Unable to parse input.");
                ApplyInstruction(parseInfo);
            }

            var duplicates = cuboids.Select(x => cuboids.Except(new CubeArea[] { x }).Select(y => (x, y, x.GetIntersectingCube(y))).Where(e => e.Item3 != null)).Where(x => x.Any());
            System.IO.File.WriteAllText(@"C:\Temp\cuboids.ply", Point3.GetStanfordPly(cuboids.SelectMany(x => CubeArea.GetAllCorners(x.Start, x.End)).Distinct()));

            var cubeVolumes = cuboids.Select(x => x.Volume());
            var totalVolume = cubeVolumes.Aggregate((long accumulator, long next) => accumulator + next);
            return $"There are {totalVolume} cubes enabled in the reactor";
        }

        private void ApplyInstruction(Match parseInfo)
        {
            bool targetValue = parseInfo.Groups["state"].Value == "on";
            int zStart = int.Parse(parseInfo.Groups["start"].Captures[2].Value);
            int zEnd = int.Parse(parseInfo.Groups["end"].Captures[2].Value);
            int yStart = int.Parse(parseInfo.Groups["start"].Captures[1].Value);
            int yEnd = int.Parse(parseInfo.Groups["end"].Captures[1].Value);
            int xStart = int.Parse(parseInfo.Groups["start"].Captures[0].Value);
            int xEnd = int.Parse(parseInfo.Groups["end"].Captures[0].Value);
            CubeArea collider = new CubeArea(new Point3(xStart, yStart, zStart), new Point3(xEnd, yEnd, zEnd));

            if (limitRange)
            {
                var newCollider = collider.GetIntersectingCube(rangeLimit);
                collider = newCollider;
                if (collider == null)
                    return;
            }

            HashSet<CubeArea> newCuboids = new HashSet<CubeArea>();
            foreach (CubeArea cuboid in cuboids)
            {
                foreach (CubeArea segment in cuboid.Substract(collider))
                    newCuboids.Add(segment);
            }
            if (targetValue)
                newCuboids.Add(collider);

            cuboids = newCuboids;
        }
    }
}
