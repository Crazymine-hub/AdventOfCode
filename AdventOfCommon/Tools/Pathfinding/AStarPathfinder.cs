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
    public delegate bool PathCheckerDelegate(AStarNodeConnection nodeConnection, AStarNode currentNode);

    public class AStarPathfinder
    {
        readonly List<AStarNodeConnection> connections;
        readonly List<AStarNode> nodes;

        public event ExpansionDelegate? OnExpanded;
        public PathCheckerDelegate? CanUsePathCallback { get; set; }

        public AStarPathfinder(List<AStarNodeConnection> connectionList)
        {
            connections = connectionList;
            nodes = connections
                .SelectMany(x => new AStarNode[] { x.NodeA, x.NodeB })
                .Distinct()
                .ToList();
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

        public AStarNode[] GetPath(AStarNode startNode,
                                  AStarNode endNode,
                                  CancellationToken cancellationToken,
                                  out List<AStarNodeConnection> connectionList,
                                  out double totalDistance)
        {
            AStarNode active = startNode;
            HashSet<AStarNode> expandedNodes = new HashSet<AStarNode>();
            HashSet<AStarNode> consideredNodes = new HashSet<AStarNode>();
            Dictionary<AStarNode, HashSet<AStarNodeConnection>> neighbourDictionary = new Dictionary<AStarNode, HashSet<AStarNodeConnection>>();
            foreach (var connection in connections)
            {
                if (connection.Direction.HasFlag(ConnectionDirection.AToB))
                {
                    AddNeighbourConnectionEntry(neighbourDictionary, connection, connection.NodeA);
                    connection.NodeA.PathLength = double.PositiveInfinity;
                    connection.NodeA.PreviousNode = null;
                }

                if (connection.Direction.HasFlag(ConnectionDirection.BToA))
                {
                    AddNeighbourConnectionEntry(neighbourDictionary, connection, connection.NodeB);
                    connection.NodeB.PathLength = double.PositiveInfinity;
                    connection.NodeB.PreviousNode = null;
                }
            }


            active.PathLength = 0;
            totalDistance = 0;
            connectionList = new List<AStarNodeConnection>();

            while (active != endNode)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ExpandNode(expandedNodes, consideredNodes, neighbourDictionary, active);

                var withoutExpanded = consideredNodes.Except(expandedNodes);

                OnExpanded?.Invoke(
                    expandedNodes,
                         withoutExpanded.ToList()
                         .AsReadOnly(),
                    active);

                var inUseNodes = GetPathToNode(active);
                AStarNode? nextActive = null;
                foreach(var node in withoutExpanded.Except(inUseNodes))
                    if(nextActive == null || nextActive.ExpansionPriority > node.ExpansionPriority)
                        nextActive = node;
                if (nextActive == active || nextActive == null) return new AStarNode[0];
                active = nextActive;

            }
            List<AStarNode> finalPath = GetPathToNode(active);
            totalDistance = active.PathLength;
            for (int i = 0; i < finalPath.Count - 1; ++i)
                connectionList.Add(connections.Single(x => x.HasConnectionTo(finalPath[i]) && x.HasConnectionTo(finalPath[i + 1])));
            return finalPath.ToArray();

        }

        private void AddNeighbourConnectionEntry(Dictionary<AStarNode, HashSet<AStarNodeConnection>> neighbourDictionary, AStarNodeConnection connection, AStarNode connTarget)
        {
            if (!neighbourDictionary.ContainsKey(connTarget))
                neighbourDictionary.Add(connTarget, new HashSet<AStarNodeConnection>());
            neighbourDictionary[connTarget].Add(connection);
        }

        private void ExpandNode(HashSet<AStarNode> expandedNodes,
                                HashSet<AStarNode> consideredNodes,
                                Dictionary<AStarNode, HashSet<AStarNodeConnection>> neighbourDictionary,
                                AStarNode active)
        {
            if(!neighbourDictionary.TryGetValue(active, out HashSet<AStarNodeConnection>? neighbours))
                neighbours = new HashSet<AStarNodeConnection>();
            double totalDistance = active.PathLength;

            foreach (var connection in neighbours)
            {
                if (connection.Distance < 0 || !(CanUsePathCallback?.Invoke(connection, active) ?? true)) continue;
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
    }
}
