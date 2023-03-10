using AdventOfCode.Days.Tools.Day16;
using AdventOfCode.Tools;
using AdventOfCode.Tools.Graphics;
using AdventOfCode.Tools.Pathfinding;
using AdventOfCode.Tools.Pathfinding.AStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day16: DayBase
    {
        public override string Title => "Proboscidea Volcanium";

        private List<ValveInfo> allValves;
        private List<ValveInfo> flowValves;
        private List<ValveNodeConnection> connections;
        private const string startValveName = "AA";

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            LoadValveMap(input);

            //var mostRelease = ProcessStep(caveValves.Single(x => x.Name == "AA"), 0, 0);

            var mostRlease = FindOptimalReleaseRoute();

            return $"You can release at most {GetReleasedPressure(mostRlease, true)} pressure.";
        }

        private int GetReleasedPressure(List<ValvePathStep> mostRlease, bool writeToConsole)
        {
            int minute = 0;
            int totalReleased = 0;
            List<ValveInfo> openedValves = new List<ValveInfo>();
            foreach (var step in mostRlease.Skip(1))
            {
                ProcessMinute();
                if (writeToConsole)
                {
                    Console.WriteLine($"You move to valve {step.Valve.Name}.");
                    Console.WriteLine();
                }

                if (step.Opened)
                {
                    ProcessMinute();
                    if (writeToConsole)
                    {
                        Console.WriteLine($"You open valve {step.Valve.Name}.");
                        Console.WriteLine();
                    }
                    openedValves.Add(step.Valve);
                    openedValves = openedValves.OrderBy(x => x.Name).ToList();
                }
            }

            while (minute < 30)
            {
                ProcessMinute();
                if (writeToConsole)
                    Console.WriteLine();
            }

            void ProcessMinute()
            {
                minute++;
                if (writeToConsole)
                    Console.WriteLine($"== Minute {minute} ==");
                var minutePressure = openedValves.Sum(x => x.PressureRelease);
                totalReleased += minutePressure;

                if (!writeToConsole) return;

                if (openedValves.Count == 0)
                    Console.WriteLine("No valves are open.");
                else if (openedValves.Count == 1)
                    Console.WriteLine($"Valve {openedValves.First().Name} is open, releasing {minutePressure} pressure.");
                else
                    Console.WriteLine($"Valves {string.Join(", ", openedValves.Take(openedValves.Count - 1).Select(x => x.Name))} and {openedValves.Last().Name} are open, releasing {minutePressure} pressure.");
            }
            return totalReleased;
        }

        private List<ValvePathStep> FindOptimalReleaseRoute()
        {
            var startValve = allValves.Single(x => x.Name == startValveName);
            var initial = new ValvePath();
            initial.Add(startValve, 0);
            var path = FindAllPaths(startValve, 30, initial)
                .OrderByDescending(x => x.PressureReleased)
                .First();
            return GetSteps(path);
        }

        //DFS shamelessly copied from u/nervario https://github.com/amafoas/advent-of-code-2022/blob/8f98c41b8783455a4b47390a6fe88066e8191630/Day-16/main.go#L95
        private List<ValvePath> FindAllPaths(ValveInfo current, int timeLeft, ValvePath path)
        {
            var newPaths = new List<ValvePath>() { path };
            foreach (ValveInfo targetValve in flowValves)
            {
                if (targetValve == current) continue;
                var connection = connections.Single(x => x.HasConnectionTo(current, targetValve));
                var newTimeLeft = timeLeft - (int)connection.Distance - 1; //go to and open valve
                if (newTimeLeft <= 0 || path.Path.Contains(targetValve)) continue;

                var basePath = new ValvePath();
                basePath.Path.AddRange(path.Path);
                basePath.PressureReleased = path.PressureReleased;
                basePath.Add(targetValve, newTimeLeft * targetValve.PressureRelease);
                newPaths.AddRange(FindAllPaths(targetValve, newTimeLeft, basePath));
            }

            return newPaths;
        }

        private List<ValvePathStep> GetSteps(ValvePath valvePath)
        {
            var path = valvePath.Path;
            var steps = new List<ValvePathStep>
            {
                new ValvePathStep(path.First(), false)
            };

            for (int i = 0; i < path.Count - 1; ++i)
            {
                var fromNode = path[i];
                var toNode = path[i + 1];
                var connection = connections.Single(x => x.HasConnectionTo(fromNode, toNode));
                var needsReverse = connection.NodeA == toNode;
                foreach (var betweenNode in connection.PassedNodes.Reverse<ValveInfo>())
                    steps.Add(new ValvePathStep(betweenNode, false));
                var pathStep = new ValvePathStep((ValveInfo)connection.GetOtherNode(fromNode), true);
                steps.Add(pathStep);
            }
            return steps;
        }

        private void LoadValveMap(string input)
        {
            allValves = new List<ValveInfo>();
            var allConnections = new List<ValveNodeConnection>();
            var matches = Regex.Matches(input, @"Valve (?<Valve>\w+) has flow rate=(?<FlowRate>\d+); tunnels? leads? to valves? (?:(?<Tunnels>\w+)(?:, )?)+", RegexOptions.Multiline);
            foreach (Match match in matches)
            {
                var valve = new ValveInfo(match.Groups["Valve"].Value, int.Parse(match.Groups["FlowRate"].Value));
                foreach (Capture tunnel in match.Groups["Tunnels"].Captures)
                    valve.ConnectedValves.Add(tunnel.Value);
                allValves.Add(valve);
            }


            foreach (ValveInfo valve in allValves)
            {
                foreach (string connectedValveName in valve.ConnectedValves)
                {
                    ValveInfo connectedValve = allValves.Single(x => x.Name == connectedValveName);
                    var connection = new ValveNodeConnection(valve, connectedValve, 1, null);
                    if (!allConnections.Any(x => x.IsSameConnection(connection)))
                        allConnections.Add(connection);
                }
            }

            flowValves = allValves.Where(x => x.PressureRelease > 0).ToList();
            connections = new List<ValveNodeConnection>();
            var pathfind = new AStarPathfinder(allConnections.Cast<AStarNodeConnection>().ToList());
            foreach (var valveA in allValves.Where(x => x.Name == startValveName).Concat(flowValves))
            {
                foreach (var valveB in flowValves)
                {
                    if (valveA == valveB || connections.Any(x => x.HasConnectionTo(valveA, valveB))) continue;
                    var path = pathfind.GetPath(valveA, valveB).Cast<ValveInfo>().Skip(1).TakeWhile(x => x != valveB).ToList();
                    connections.Add(new ValveNodeConnection(valveA, valveB, path.Count + 1, path));
                }
            }
        }
    }
}
