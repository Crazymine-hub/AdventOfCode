using AdventOfCode.Days.Tools.Day15;
using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day15: DayBase
    {
        public override string Title => "Beacon Exclusion Zone";
        private VisualFormHandler visualForm = VisualFormHandler.GetInstance();
        private Bitmap bitmap;
        private const int PixelSize = 5;

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;

            bitmap = new Bitmap(1, 1);
            visualForm.Show(bitmap);
            DynamicGrid<SensorState> sensorGrid = LoadSensorGrid(input);

            bitmap.FillRect(sensorGrid.XOrigin * PixelSize, sensorGrid.YOrigin * PixelSize, PixelSize, PixelSize, Color.Lime);

            var covered = 0;
            for (int x = 0; x < sensorGrid.XDim; ++x)
            {
                var yOffset = 10 + sensorGrid.YOrigin;
                if (sensorGrid[x, yOffset] != SensorState.Empty && sensorGrid[x, yOffset] != SensorState.Beacon)
                {
                    bitmap.FillRect(x * PixelSize, (yOffset) * PixelSize, PixelSize, PixelSize, Color.Yellow);
                    covered++;
                }
            }
            visualForm.Update(bitmap);

            return $"The Selected line has {covered} tiles covered";
        }

        private DynamicGrid<SensorState> LoadSensorGrid(string input)
        {
            DynamicGrid<SensorState> sensorGrid = new DynamicGrid<SensorState>();
            foreach (string reportLine in GetLines(input))
            {
                var report = Regex.Match(reportLine, @"^Sensor at x=(?<sX>-?\d+), y=(?<sY>-?\d+): closest beacon is at x=(?<bX>-?\d+), y=(?<bY>-?\d+)$");
                if (!report.Success) throw new FormatException($"Could not parse line \"{reportLine}\"");
                var sensorPosition = new Point(int.Parse(report.Groups["sX"].Value), int.Parse(report.Groups["sY"].Value));
                var beaconPosition = new Point(int.Parse(report.Groups["bX"].Value), int.Parse(report.Groups["bY"].Value));

                sensorGrid.SetRelative(sensorPosition.X, sensorPosition.Y, SensorState.Sensor);
                sensorGrid.SetRelative(beaconPosition.X, beaconPosition.Y, SensorState.Beacon);
                RenderGrid(sensorGrid);

                var areaOffset = VectorAssist.PointDifference(sensorPosition, beaconPosition);
                var xSpread = Math.Abs(areaOffset.X) + Math.Abs(areaOffset.Y);
                for (int x = -xSpread; x <= xSpread; ++x)
                {
                    var ySpread = xSpread - Math.Abs(x);
                    for (int y = -ySpread; y <= ySpread; ++y)
                        if (sensorGrid.GetRelative(x + sensorPosition.X, y + sensorPosition.Y) == SensorState.Empty)
                        {
                            sensorGrid.SetRelative(x + sensorPosition.X, y + sensorPosition.Y, SensorState.Covered);
                            RenderGrid(sensorGrid);
                        }
                }
            }

            return sensorGrid;
        }

        private void RenderGrid(DynamicGrid<SensorState> sensorGrid)
        {
            if (sensorGrid.XDim * PixelSize != bitmap.Width || sensorGrid.YDim * PixelSize != bitmap.Height)
            {
                bitmap.Dispose();
                bitmap = new Bitmap(sensorGrid.XDim * PixelSize, sensorGrid.YDim * PixelSize);
                bitmap.FillRect(0, 0, bitmap.Width, bitmap.Height, Color.Black);
            }

            foreach (var unit in sensorGrid)
            {
                if (unit.Value == SensorState.Empty) continue;
                Color color = Color.Blue;
                switch (unit.Value)
                {
                    case SensorState.Covered: color = Color.Gray; break;
                    case SensorState.Beacon: color = Color.Red; break;
                    case SensorState.Sensor: color = Color.Green; break;
                }
                bitmap.FillRect(unit.X * PixelSize, unit.Y * PixelSize, PixelSize, PixelSize, color);
            }
            visualForm.Update(bitmap);
        }
    }
}
