using AdventOfCode.Days.Tools.Day15;
using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Concurrent;
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
        private const int PixelSize = 5;
        private const int TestLine = 2_000_000;
        private readonly ConsoleAssist consoleAssist = new ConsoleAssist();

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;

            return $"The Selected line has {ProcessSensorReadings(input)} tiles covered";
        }

        private int ProcessSensorReadings(string input)
        {
            ConcurrentDictionary<int, SensorState> sensorGrid = new ConcurrentDictionary<int, SensorState>();
            var tasks = new List<Task>();
            var lines = GetLines(input);
            for (int i = 0; i < lines.Count; ++i)
            {
                //var task = new Task((object reportLine) =>
                //{
                Console.CursorLeft = 0;
                Console.Write($"Processing line {i + 1:00}/{lines.Count:00} ");
                Console.Write(consoleAssist.GetNextProgressChar());
                var reportLine = lines[i];
                var report = Regex.Match(reportLine.ToString(), @"^Sensor at x=(?<sX>-?\d+), y=(?<sY>-?\d+): closest beacon is at x=(?<bX>-?\d+), y=(?<bY>-?\d+)$");
                if (!report.Success) throw new FormatException($"Could not parse line \"{reportLine}\"");
                var sensorPosition = new Point(int.Parse(report.Groups["sX"].Value), int.Parse(report.Groups["sY"].Value));
                var beaconPosition = new Point(int.Parse(report.Groups["bX"].Value), int.Parse(report.Groups["bY"].Value));

                if (sensorPosition.Y == TestLine)
                    sensorGrid.AddOrUpdate(sensorPosition.X, SensorState.Sensor, (x, state) => SensorState.Sensor);
                if (beaconPosition.Y == TestLine)
                    sensorGrid.AddOrUpdate(beaconPosition.X, SensorState.Sensor, (x, state) => SensorState.Beacon);

                var areaOffset = VectorAssist.PointDifference(sensorPosition, beaconPosition);
                var ySpread = Math.Abs(areaOffset.X) + Math.Abs(areaOffset.Y);
                for (int y = -ySpread; y <= ySpread; ++y)
                {
                    if (y + sensorPosition.Y != TestLine) continue;
                    Console.CursorLeft--;
                    Console.Write(consoleAssist.GetNextProgressChar());
                    var xSpread = ySpread - Math.Abs(y);
                    for (int x = -xSpread; x <= xSpread; ++x)
                        sensorGrid.AddOrUpdate(x + sensorPosition.X, SensorState.Covered, (_, state) => state == SensorState.Empty ? SensorState.Covered : state);
                }
                //}, lines[i]);
                //tasks.Add(task);
                //task.Start();
            }

            //consoleAssist.WaitForAllTasks(tasks);
            Console.WriteLine();
            return sensorGrid.Count(x => x.Value == SensorState.Covered);
        }
    }
}
