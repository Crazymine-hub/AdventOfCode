using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day18: DayBase
    {
        public override string Title => "Boiling Boulders";
        public override bool UsesAdditionalContent => true;
        private DynamicGrid<bool?> lavaDrops;


        public override string Solve(string input, bool part2)
        {
            ReadLavaPositions(input, part2);
            lavaDrops.AddMargin(1, true);
            if (part2)
                FloodFill();
            return $"There are {CountFreeSides()} free sides on the lava drops";
        }

        private void FloodFill()
        {
            lavaDrops[0, 0, 0] = false;
            bool updated = false;
            do
            {
                updated = false;
                foreach (var spot in lavaDrops)
                {
                    if (spot.Value != null) continue;
                    var neighbours = lavaDrops.GetNeighbours(spot.X, spot.Y, spot.Z, false).Where(x => !(x.X == spot.X && x.Y == spot.Y && x.Z == spot.Z));
                    if (neighbours.Any(x => x.Value == false))
                    {
                        lavaDrops[spot.X, spot.Y, spot.Z] = false;
                        updated = true;
                    }
                }
            } while (updated);
        }

        private int CountFreeSides()
        {
            var totalFreeSides = 0;
            foreach (var drop in lavaDrops)
            {
                if (drop.Value != true) continue;
                var freeSides = lavaDrops.GetNeighbours(drop.X, drop.Y, drop.Z, false)
                    .Where(x => !(x.X == drop.X && x.Y == drop.Y && x.Z == drop.Z) && x.Value == false)
                    .Count();
                totalFreeSides += freeSides;
            }
            return totalFreeSides;
        }

        private void ReadLavaPositions(string input, bool part2)
        {
            lavaDrops = new DynamicGrid<bool?>();
            if (!part2)
                lavaDrops.GetDefault += GetFalseDefault;
            List<Point3> dropPoints = new List<Point3>();
            foreach (string line in GetLines(input))
            {
                var lavaDrop = line.Split(',');
                var point3 = new Point3(int.Parse(lavaDrop[0]), int.Parse(lavaDrop[1]), int.Parse(lavaDrop[2]));
                lavaDrops.SetRelative(point3.X, point3.Y, point3.Z, true);
                dropPoints.Add(point3);
            }
            AdditionalContent = Point3.GetStanfordPly(dropPoints, "This is a PLY file of the final Points for AdventOfCode 2022 Day 18.\n" +
                "It can be opened with 3D Visualisation software, e.g. Blender");
        }

        private bool? GetFalseDefault() => false;
    }
}
