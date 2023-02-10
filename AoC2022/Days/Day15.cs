using AdventOfCode.Days.Tools.Day15;
using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Graphics;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day15: DayBase
    {
        public override string Title => "Beacon Exclusion Zone";
        private const int TestLine = 2_000_000;
        private const int MaxRange = 4_000_000;
        private const int WorkTasks = 10;
        private readonly ConsoleAssist consoleAssist = new ConsoleAssist();

        private int recentLine = 0;


        private struct SensorBeaconPair
        {
            public Point SensorPosition { get; }
            public Point BeaconPosition { get; }

            public SensorBeaconPair(Point sensorPosition, Point beaconPosition)
            {
                SensorPosition = sensorPosition;
                BeaconPosition = beaconPosition;
            }
        }

        private struct TaskInformation
        {
            public SensorBeaconPair? TargetSensor { get; }
            public List<SensorBeaconPair> SensorArray { get; }
            public CancellationTokenSource TokenSource { get; }
            public ConcurrentBag<Point> LocationBag { get; }

            public TaskInformation(List<SensorBeaconPair> sensorArray, CancellationTokenSource tokenSource, ConcurrentBag<Point> locationBag)
            {
                TargetSensor = null;
                this.SensorArray = sensorArray;
                this.TokenSource = tokenSource;
                LocationBag = locationBag;
            }

            public TaskInformation(SensorBeaconPair targetSensor, CancellationTokenSource tokenSource, ConcurrentBag<Point> locationBag)
            {
                TargetSensor = targetSensor;
                this.SensorArray = null;
                this.TokenSource = tokenSource;
                LocationBag = locationBag;
            }
        }

        public override string Solve(string input, bool part2)
        {
            var lines = GetLines(input);
            var sensorArray = GetSensors(lines);


            if (!part2)
            {
                var occupation = new Dictionary<int, SensorState>();
                GetLineOccupation(occupation, TestLine, sensorArray, part2);
                return $"The Selected line has {occupation.Count(x => x.Value == SensorState.Covered)} tiles covered";
            }

            Console.WriteLine("Searching Distress signal");
            var distressToken = new CancellationTokenSource();
            ConcurrentBag<Point> bag = new ConcurrentBag<Point>();

            Console.WriteLine("Finding viable points");
            var locationTasks = sensorArray.Select(sensor =>
            {
                var task = new Task(FindPossiblePoints, new TaskInformation(sensor, distressToken, bag), distressToken.Token);
                task.Start();
                return task;
            }).ToList();
            consoleAssist.WaitForAllTasks(locationTasks, cancellationToken: CancellationToken, progressUpdateCallback: () => $"{bag.Count} locations found.");

            var totalItems = bag.Count;
            Console.WriteLine($"Checking {totalItems} locations");
            var checkTasks = Enumerable.Range(0, WorkTasks).Select(offset =>
            {
                var task = new Task<Point?>(FindDistressSignal, new TaskInformation(sensorArray, distressToken, bag), distressToken.Token);
                task.Start();
                return task;
            }).ToList();
            consoleAssist.WaitForAllTasks(checkTasks, cancellationToken: CancellationToken, progressUpdateCallback: () => $"{totalItems - bag.Count}/{totalItems} ({(totalItems - bag.Count) / (double)totalItems * 100}%)");

            var distress = checkTasks.Where(x => x.Status == TaskStatus.RanToCompletion && x.Result.HasValue).Select(x => x.Result.Value).Single();

            return $"The distress Signal comes from {distress}. The tuning Frequency is {(ulong)distress.X * 4_000_000ul + (ulong)distress.Y}";

        }

        private void FindPossiblePoints(object taskInformation)
        {
            var info = (TaskInformation)taskInformation;
            var ySpread = VectorAssist.ManhattanDistance(info.TargetSensor.Value.SensorPosition, info.TargetSensor.Value.BeaconPosition);
            info.LocationBag.Add(new Point(info.TargetSensor.Value.SensorPosition.X, info.TargetSensor.Value.SensorPosition.Y + ySpread + 1));
            info.LocationBag.Add(new Point(info.TargetSensor.Value.SensorPosition.X, info.TargetSensor.Value.SensorPosition.Y - ySpread - 1));
            for (int y = -ySpread; y <= ySpread; y++)
            {
                var xSpread = ySpread - Math.Abs(y) + 1;
                info.LocationBag.Add(new Point(info.TargetSensor.Value.SensorPosition.X + xSpread, info.TargetSensor.Value.SensorPosition.Y + y));
                info.LocationBag.Add(new Point(info.TargetSensor.Value.SensorPosition.X - xSpread, info.TargetSensor.Value.SensorPosition.Y + y));
            }
        }

        private string GetTotalProgress() => $"{(recentLine * 100 / (double)MaxRange)}";

        private Point? FindDistressSignal(object taskInformation)
        {
            var info = (TaskInformation)taskInformation;
            while (info.LocationBag.TryTake(out Point point))
            {
                CancellationToken.ThrowIfCancellationRequested();
                info.TokenSource.Token.ThrowIfCancellationRequested();

                if (point.X < 0 || point.X > MaxRange || point.Y < 0 || point.Y > MaxRange) continue;

                if (IsPixelEmpty(point.X, point.Y, info.SensorArray))
                {
                    info.TokenSource.Cancel();
                    return point;
                }
            }
            return null;
        }

        private static List<SensorBeaconPair> GetSensors(List<string> lines)
        {
            var sensorArray = new List<SensorBeaconPair>();
            foreach (var reportLine in lines)
            {
                var report = Regex.Match(reportLine.ToString(), @"^Sensor at x=(?<sX>-?\d+), y=(?<sY>-?\d+): closest beacon is at x=(?<bX>-?\d+), y=(?<bY>-?\d+)$");
                if (!report.Success) throw new FormatException($"Could not parse line \"{reportLine}\"");
                sensorArray.Add(
                    new SensorBeaconPair(
                        new Point(int.Parse(report.Groups["sX"].Value), int.Parse(report.Groups["sY"].Value)),
                        new Point(int.Parse(report.Groups["bX"].Value), int.Parse(report.Groups["bY"].Value))
                        )
                    );
            }
            return sensorArray;
        }

        private void GetLineOccupation(Dictionary<int, SensorState> resultBuffer, int line, List<SensorBeaconPair> sensors, bool part2, CancellationToken additionalToken = default)
        {
            resultBuffer.Clear();
            foreach (var sensor in sensors)
            {
                var areaOffset = VectorAssist.PointDifference(sensor.SensorPosition, sensor.BeaconPosition);
                var ySpread = Math.Abs(areaOffset.X) + Math.Abs(areaOffset.Y);
                if (line < sensor.SensorPosition.Y - ySpread || line > sensor.SensorPosition.Y + ySpread) continue;
                if (sensor.SensorPosition.Y == line)
                {
                    if (!resultBuffer.ContainsKey(sensor.SensorPosition.X))
                        resultBuffer.Add(sensor.SensorPosition.X, SensorState.Sensor);
                    else
                        resultBuffer[sensor.SensorPosition.X] = SensorState.Sensor;
                }

                if (sensor.BeaconPosition.Y == line)
                {
                    if (!resultBuffer.ContainsKey(sensor.BeaconPosition.X))
                        resultBuffer.Add(sensor.BeaconPosition.X, SensorState.Beacon);
                    else
                        resultBuffer[sensor.BeaconPosition.X] = SensorState.Beacon;
                }

                var y = sensor.SensorPosition.Y - line;
                var pos = new Point(sensor.SensorPosition.X, line);
                var xSpread = ySpread - Math.Abs(y);
                pos.Offset(-xSpread - 1, 0);
                for (int xOffset = -xSpread; xOffset <= xSpread; ++xOffset)
                {
                    var x = sensor.SensorPosition.X + xOffset;
                    CancellationToken.ThrowIfCancellationRequested();
                    additionalToken.ThrowIfCancellationRequested();
                    pos.Offset(1, 0);
                    if (!resultBuffer.ContainsKey(x))
                        resultBuffer.Add(x, SensorState.Covered);
                }
            }

            if (part2)
            {
                var invalidKeys = resultBuffer.Keys.Where(x => x < 0 || x > MaxRange).ToHashSet();
                foreach (var key in invalidKeys)
                    resultBuffer.Remove(key);
            }
        }

        private bool IsPixelEmpty(int x, int y, List<SensorBeaconPair> sensors)
        {
            foreach (var sensor in sensors)
            {
                if ((sensor.SensorPosition.X == x && sensor.SensorPosition.Y == y) ||
                    (sensor.BeaconPosition.X == x && sensor.BeaconPosition.Y == y)) return false;

                var yOffset = sensor.SensorPosition.Y - y;
                var xOffset = sensor.SensorPosition.X - x;

                var maxDistance = VectorAssist.ManhattanDistance(sensor.SensorPosition, sensor.BeaconPosition);
                var offsetDistance = Math.Abs(xOffset) + Math.Abs(yOffset);

                if (offsetDistance <= maxDistance) return false;
            }
            return true;
        }
    }
}