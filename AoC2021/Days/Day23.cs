using AdventOfCode.Days.Tools.Day23;
using AdventOfCode.Tools.Pathfinding;
using AdventOfCode.Tools.Pathfinding.AStar;
using AdventOfCode.Tools.TopologicalOrder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day23 : DayBase
    {
        public override string Title => "Amphipod";
        private readonly List<char> amphipodNames = new List<char> { '.', 'A', 'B', 'C', 'D' };
        private readonly List<long> amphipodEnergyCost = new List<long> { 0, 1, 10, 100, 1000 };

        private readonly List<AmphipodNode> nodes = new List<AmphipodNode>();
        private readonly List<AStarNodeConnection> connections = new List<AStarNodeConnection>();
        private Dictionary<string, BoardState> states = new Dictionary<string, BoardState>();
        private HashSet<string> exploredStates = new HashSet<string>();
        private Queue<string> unexploredStates = new Queue<string>();

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            LoadLayout(input);

            Console.WriteLine(string.Join("\r\n", VisualizeBoard()));

            Console.WriteLine();

            var startState = new BoardState(SerializeBoard(), false);
            states.Add(startState.StateString, startState);
            unexploredStates.Enqueue(startState.StateString);

            Console.WriteLine();
            Console.WriteLine("Processing Board...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ProcessBoardStates();
            stopwatch.Stop();

            Console.WriteLine($"Genrated {exploredStates.Count} board states in {stopwatch.Elapsed}");

            Console.WriteLine("Calculating...");
            stopwatch.Restart();
            var uncalculatedStates = states.Values.ToHashSet();
            while (uncalculatedStates.Any())
            {
                var current = uncalculatedStates.First(x => x.CanCalculateCost());
                current.StartCheapestMoveCalculation();
                uncalculatedStates.Remove(current);
            }

            stopwatch.Stop();
            return $"The Amphipods will consume {startState.CheapestMoveCost} energy (calculated in {stopwatch.Elapsed})";
        }

        private List<string> TemporaryVisualize(string boardString)
        {
            string oldBoard = SerializeBoard();
            DeserializeBoard(boardString);
            var result = VisualizeBoard();
            DeserializeBoard(boardString);
            return result;
        }

        private List<string> VisualizeBoard()
        {
            var lowest = nodes.Max(x => x.Y) + 1;
            var furthest = nodes.Max(x => x.X) + 1;
            var result = new List<string>();
            for (int y = 0; y <= lowest; ++y)
            {
                string line = string.Empty;
                for (int x = 0; x <= furthest; ++x)
                {
                    var node = nodes.SingleOrDefault(n => n.X == x && n.Y == y);
                    if (node == null)
                    {
                        line += '#';
                        continue;
                    }
                    line += amphipodNames[node.OccupiedBy];
                }
                result.Add(line);
            }
            return result;
        }

        private void ProcessBoardStates()
        {
            while (unexploredStates.Any())
            {
                BoardState currentState = states[unexploredStates.Dequeue()];
                exploredStates.Add(currentState.StateString);
                DeserializeBoard(currentState.StateString);
                var moves = GetMovableAmphipodNodes().SelectMany(x => GetTargetPaths(x)).ToList();
                foreach (var move in moves)
                {
                    DeserializeBoard(currentState.StateString);
                    var moveCost = ApplyMove(move);
                    var boardString = SerializeBoard();

                    if (exploredStates.Contains(boardString) || unexploredStates.Contains(boardString))
                    {
                        currentState.FurtherStates.Add((states[boardString], moveCost));
                        continue;
                    }

                    BoardState nextState = new BoardState(boardString, IsComplete());
                    currentState.FurtherStates.Add((nextState, moveCost));
                    unexploredStates.Enqueue(nextState.StateString);
                    states.Add(nextState.StateString, nextState);
                }
            }
        }

        private long ApplyMove(AmphipodPath path)
        {
            path.To.OccupiedBy = path.From.OccupiedBy;
            path.From.OccupiedBy = AmphipodNode.UnoccupiedNodeValue;
            return path.PathLength * amphipodEnergyCost.ElementAt(path.To.OccupiedBy);
        }

        private IEnumerable<AmphipodPath> GetTargetPaths(AmphipodNode amphipod)
        {
            Queue<(AmphipodNode node, int startDistance)> toExpand = new Queue<(AmphipodNode node, int startDistance)>();
            HashSet<AmphipodNode> exploredTargets = new HashSet<AmphipodNode>();
            toExpand.Enqueue((amphipod, 0));

            while (toExpand.Any())
            {
                (var currentNode,var pathLength) = toExpand.Dequeue();
                ++pathLength;
                foreach (var neighbour in GetNeighbourNodes(currentNode))
                {
                    if (exploredTargets.Contains(neighbour) || neighbour.IsOccupied)
                        continue;
                    if (IsValidTarget(amphipod, neighbour))
                        yield return new AmphipodPath(amphipod, neighbour, pathLength);
                    toExpand.Enqueue((neighbour, pathLength));

                }
                exploredTargets.Add(currentNode);
            }
        }

        private void LoadLayout(string input)
        {
            var lines = GetLines(input);
            int lineLength = lines.Select(l => l.Length).Max();
            for (int lineNr = 0; lineNr < lines.Count; lineNr++)
            {
                byte neighbourIndex = 1;
                AmphipodNode lastNode = null;
                var line = lines[lineNr].PadRight(lineLength);
                for (int charNr = 0; charNr < line.Length; charNr++)
                {
                    if (line[charNr] == ' ' || line[charNr] == '#')
                    {
                        lastNode = null;
                        continue;
                    }
                    bool hasTunnel = lineNr == 1 && lines[lineNr + 1][charNr] != '#';

                    var node = new AmphipodNode(charNr,
                                                lineNr,
                                                !hasTunnel,
                                                lineNr > 1 && line[charNr] != '#' ? neighbourIndex++ : (byte)0,
                                                (byte)(line[charNr] == '.' ? 0 : amphipodNames.IndexOf(line[charNr])));
                    nodes.Add(node);
                    if (lastNode != null)
                        connections.Add(new AStarNodeConnection(lastNode, node));
                    lastNode = node;
                    var nodeAbove = nodes.SingleOrDefault(x => x.X == charNr && x.Y == lineNr - 1);
                    if (nodeAbove != null)
                        connections.Add(new AStarNodeConnection(node, nodeAbove));
                }
            }
        }

        private IEnumerable<AmphipodNode> GetMovableAmphipodNodes() => nodes.Where(n => CanBeMoved(n));
        private bool CanBeMoved(AmphipodNode node) => node.IsOccupied && HasFreeNeighbours(node) && (!node.IsAtHome || (
            node.IsAtHome &&
            nodes.Any(x => x.Y > node.Y && x.HomeTo == node.HomeTo && x.OccupiedBy != node.OccupiedBy)
            ));

        private bool HasFreeNeighbours(AmphipodNode node) => connections
                        .Any(connection => connection.HasConnectionTo(node) && !((AmphipodNode)connection.GetOtherNode(node)).IsOccupied);

        private bool IsComplete() => nodes.All(x => x.HomeTo == AmphipodNode.UnoccupiedNodeValue || x.IsAtHome);

        private bool IsValidTarget(AmphipodNode amphipod, AmphipodNode target) =>
            target.CanOccupy &&
            !target.IsOccupied &&
            (
                (target.HomeTo == AmphipodNode.UnoccupiedNodeValue && amphipod.HomeTo != AmphipodNode.UnoccupiedNodeValue) || //going to hallway? (start node can't be hallway)
                (target.HomeTo == amphipod.OccupiedBy && CanMoveInRoom(target) //is the room right and can we go in?
            && nodes  //is the node we move to the deepest, we can go in that room (i.e. highest Y value)? we don't wan't to move again or leave empty space in this move.
                .Where(x => x.HomeTo == target.HomeTo && x.OccupiedBy == AmphipodNode.UnoccupiedNodeValue)
                .OrderByDescending(x => x.Y).First() == target)
            );

        private bool CanMoveInRoom(AmphipodNode node) =>
            node.HomeTo == AmphipodNode.UnoccupiedNodeValue ?
                throw new InvalidOperationException($"Node ({node.X}|{node.Y}) is not part of a room") :
                //room should be free, or only occupied by correct amphipods
                nodes.Where(x => x.HomeTo == node.HomeTo).All(x => x.OccupiedBy == AmphipodNode.UnoccupiedNodeValue || x.OccupiedBy == x.HomeTo);
        private IEnumerable<AmphipodNode> GetNeighbourNodes(AmphipodNode amphipod)
            => connections.Where(x => x.HasConnectionTo(amphipod)).Select(x => x.GetOtherNode(amphipod)).Cast<AmphipodNode>();

        private string SerializeBoard() =>
            Convert.ToBase64String(nodes
            .OrderBy(x => x.X)
            .ThenBy(x => x.Y)
            .Select(x => x.OccupiedBy)
            .ToArray());


        private void DeserializeBoard(string boardString)
        {
            var board = Convert.FromBase64String(boardString);
            if (board.Length != nodes.Count) throw new ArgumentException("Given board size doesn't match the target board size.");
            foreach (var amphipod in nodes.OrderBy(x => x.X).ThenBy(x => x.Y).Zip(board, (node, boardValue) => (node, boardValue)))
            {
                if (!amphipod.node.CanOccupy && amphipod.boardValue == AmphipodNode.UnoccupiedNodeValue) continue;
                amphipod.node.OccupiedBy = amphipod.boardValue;
            }
        }
    }
}
