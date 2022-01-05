using AdventOfCode.Tools.Pathfinding.AStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Pathfinding
{
    public delegate void ExpansionDelegate(IReadOnlyCollection<AStarNode> expanded, IReadOnlyCollection<AStarNode> considered, AStarNode active);
    public class PathfindResult
    {
        public AStarNode[] Path { get; }
        public List<AStarNodeConnection> ConnectionList { get; }
        public double TotalDistance { get; }

        public PathfindResult(AStarNode[] path, List<AStarNodeConnection> connectionList, double totalDistance)
        {
            Path = path;
            ConnectionList = connectionList;
            TotalDistance = totalDistance;
        }
    }

    public class AStarPathfinder
    {
        readonly List<AStarNodeConnection> connections;
        readonly List<AStarNode> nodes;
        HashSet<AStarNode> expandedNodes;
        HashSet<AStarNode> consideredNodes;
        Dictionary<AStarNode, HashSet<AStarNodeConnection>> neighbourDictionary;

        public event ExpansionDelegate OnExpanded;

        public double DistanceHeatWeigh { get; set; } = 0.5;


        public AStarPathfinder(List<AStarNodeConnection> connectionList)
        {
            connections = connectionList;
            nodes = connections
                .SelectMany(x => new AStarNode[] { x.NodeA, x.NodeB })
                .Distinct()
                .ToList();
        }

        public AStarNode[] GetPath(AStarNode startNode,
                                  AStarNode endNode,
                                  CancellationToken cancellationToken,
                                  out List<AStarNodeConnection> connectionList,
                                  out double totalDistance)
        {
            AStarNode active = startNode;
            expandedNodes = new HashSet<AStarNode>();
            consideredNodes = new HashSet<AStarNode>();
            neighbourDictionary = new Dictionary<AStarNode, HashSet<AStarNodeConnection>>();

            foreach (var connection in connections)
            {
                AddNeighbourConnectionEntry(connection, connection.NodeA);
                AddNeighbourConnectionEntry(connection, connection.NodeB);
            }


            active.PathLength = 0;
            totalDistance = 0;
            connectionList = new List<AStarNodeConnection>();

            while (active != endNode)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ExpandNode(active);

                var withoutExpanded = consideredNodes.Except(expandedNodes);

                OnExpanded?.Invoke(
                    expandedNodes,
                         withoutExpanded.ToList()
                         .AsReadOnly(),
                    active);

                var inUseNodes = GetPathToNode(active);
                AStarNode nextActive = null;
                foreach(var node in withoutExpanded.Except(inUseNodes))
                    if(nextActive == null || nextActive.ExpansionPriority > node.ExpansionPriority)
                        nextActive = node;
                if (nextActive == active) return new AStarNode[0];
                active = nextActive;

            }
            List<AStarNode> finalPath = GetPathToNode(active);
            totalDistance = active.PathLength;
            for (int i = 0; i < finalPath.Count - 1; ++i)
                connectionList.Add(connections.Single(x => x.HasConnectionTo(finalPath[i]) && x.HasConnectionTo(finalPath[i + 1])));
            return finalPath.ToArray();

        }

        private void AddNeighbourConnectionEntry(AStarNodeConnection connection, AStarNode connTarget)
        {
            if (!neighbourDictionary.ContainsKey(connTarget))
                neighbourDictionary.Add(connTarget, new HashSet<AStarNodeConnection>());
            neighbourDictionary[connTarget].Add(connection);
        }

        private void ExpandNode(AStarNode active)
        {

            IEnumerable<AStarNodeConnection> neighbours = neighbourDictionary[active];
            double totalDistance = active.PathLength;

            foreach (var connection in neighbours)
            {
                if (connection.Distance < 0) continue;
                AStarNode target = connection.GetOtherNode(active);
                double newDistance = totalDistance + connection.Distance;
                if (target == null || target.PathLength <= newDistance) continue;
                target.PathLength = newDistance;
                target.PreviousNode = active;
                expandedNodes.Remove(target);
                consideredNodes.Add(target);
            }
            expandedNodes.Add(active);
        }



        public AStarNode[] GetPath(AStarNode start, AStarNode finish)
        {
            return GetPath(start, finish, out _, out _);
        }

        public AStarNode[] GetPath(AStarNode start, AStarNode finish, out double cost)
        {
            return GetPath(start, finish, out _, out cost);
        }

        public AStarNode[] GetPath(AStarNode startNode,
                                  AStarNode endNode,
                                  out List<AStarNodeConnection> connectionList,
                                  out double totalDistance)
        {
            return GetPath(startNode, endNode, CancellationToken.None, out connectionList, out totalDistance);
        }

        public Task<PathfindResult> GetPathAsync(AStarNode startNode,
                                  AStarNode endNode,
                                  CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                return new PathfindResult(
                    GetPath(startNode, endNode, cancellationToken, out var connectionList, out double totalDistance),
                    connectionList,
                    totalDistance);
            }, cancellationToken);
        }

        private IEnumerable<AStarNodeConnection> GetNeighbours(AStarNode targetNode) =>
            connections.Where(conn => conn.HasConnectionTo(targetNode));

        public static List<AStarNode> GetPathToNode(AStarNode targetNode)
        {
            var path = new Stack<AStarNode>();
            while (targetNode != null)
            {
                path.Push(targetNode);
                targetNode = targetNode.PreviousNode;
            }
            return path.ToList();
        }

        //private List<AStarNode> GetPathToNode(AStarNode targetNode)
        //{
        //    var path = new List<AStarNode>();
        //    if (targetNode == null) return path;

        //    path.AddRange(GetPathToNode(targetNode.PreviousNode));
        //    path.Add(targetNode);
        //    return path;
        //}
    }
}
