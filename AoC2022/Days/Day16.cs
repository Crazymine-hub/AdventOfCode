using AdventOfCode.Days.Tools.Day16;
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

        private List<ValveInfo> caveValves;
        private List<ValveNodeConnection> connections;

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            LoadValveMap(input);

            //var mostRelease = ProcessStep(caveValves.Single(x => x.Name == "AA"), 0, 0);

            var mostRlease = FindOptimalReleaseRoute();

            return $"You can release at most {GetReleasedPressure(mostRlease)} pressure.";
        }

        private int GetReleasedPressure(List<ValvePathStep> mostRlease)
        {
            int minute = 0;
            int totalReleased = 0;
            List<ValveInfo> openedValves = new List<ValveInfo>();
            foreach (var step in mostRlease.Skip(1))
            {
                ProcessMinute();
                Console.WriteLine($"You move to valve {step.Valve.Name}.");
                Console.WriteLine();

                if (step.Opened)
                {
                    ProcessMinute();
                    Console.WriteLine($"You open valve {step.Valve.Name}.");
                    Console.WriteLine();
                    openedValves.Add(step.Valve);
                    openedValves = openedValves.OrderBy(x => x.Name).ToList();
                }
            }

            while (minute < 30)
            {
                ProcessMinute();
                Console.WriteLine();
            }

            void ProcessMinute()
            {
                minute++;
                Console.WriteLine($"== Minute {minute} ==");
                var minutePressure = openedValves.Sum(x => x.PressureRelease);
                totalReleased += minutePressure;

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
            var finalPath = new List<ValvePathStep>();
            var pathFind = new AStarPathfinder(connections.Cast<AStarNodeConnection>().ToList());
            var currentNode = caveValves.Single(x => x.Name == "AA");
            var possibleValves = caveValves.Where(x => x.PressureRelease > 0).ToHashSet();
            ValveInfo nextValve = null;
            var timeSpent = 0;

            finalPath.Add(new ValvePathStep(currentNode, false));
            while (timeSpent < 30)
            {
                var bestPath = new List<ValveInfo>();
                var timeAdded = int.MaxValue;
                var mostReleased = 0;
                foreach (var targetValve in possibleValves
                    .Where(x => !x.IsOpened && x != currentNode)
                    .OrderByDescending(x => x.PressureRelease))
                {
                    var path = pathFind.GetPath(currentNode, targetValve)
                        .Skip(1)
                        .Cast<ValveInfo>()
                        .ToList();
                    var lifeTime = 30 - (timeSpent + path.Count + 1);
                    var valveRelease = lifeTime * targetValve.PressureRelease;
                    if (valveRelease > mostReleased)
                    {
                        bestPath = path;
                        timeAdded = path.Count;
                        mostReleased = valveRelease;
                    }
                }
                if (bestPath == null) throw new Exception("Something went wrong.");
                if (timeSpent + timeAdded + 1 > 30) break;
                currentNode = bestPath.Last();
                currentNode.IsOpened = true;
                timeSpent += timeAdded + 1;
                finalPath.AddRange(bestPath.Select((x, i) => new ValvePathStep(x, i == bestPath.Count - 1)));
                if (possibleValves.All(x => x.IsOpened))
                    break;
            }

            return finalPath;
        }

        private void LoadValveMap(string input)
        {
            caveValves = new List<ValveInfo>();
            connections = new List<ValveNodeConnection>();
            var matches = Regex.Matches(input, @"Valve (?<Valve>\w+) has flow rate=(?<FlowRate>\d+); tunnels? leads? to valves? (?:(?<Tunnels>\w+)(?:, )?)+", RegexOptions.Multiline);
            foreach (Match match in matches)
            {
                var valve = new ValveInfo(match.Groups["Valve"].Value, int.Parse(match.Groups["FlowRate"].Value));
                foreach (Capture tunnel in match.Groups["Tunnels"].Captures)
                    valve.ConnectedValves.Add(tunnel.Value);
                caveValves.Add(valve);
            }

            foreach (ValveInfo valve in caveValves)
            {
                foreach (string connectedValveName in valve.ConnectedValves)
                {
                    ValveInfo connectedValve = caveValves.Single(x => x.Name == connectedValveName);
                    var connection = new ValveNodeConnection(valve, connectedValve);
                    if (!connections.Any(x => x.IsSameConnection(connection)))
                        connections.Add(connection);
                }
            }
        }
    }
}
