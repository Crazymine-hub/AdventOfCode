using AdventOfCode.Tools.Pathfinding.AStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Pathfinding
{
    public class AStarPathfinder
    {
        readonly List<AStarNodeConnection> connections;
        readonly List<AStarNode> nodes;
        List<AStarNode> expandedNodes;

        public double DistanceHeatWeigh { get; set; } = 0.5;


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
            AStarNode active = startNode;
            expandedNodes = new List<AStarNode>();


            active.PathLength = 0;
            totalDistance = 0;
            connectionList = new List<AStarNodeConnection>();
            AStarNode previous = null;

            while (active != endNode)
            {
                ExpandNode(active);
                var inUseNodes = GetPathToNode(active);
                var nextActive = nodes
                    .Except(inUseNodes)
                    .Except(expandedNodes)
                    .OrderBy(x => x.ExpansionPriority)
                    .First(x => x != previous);
                if (nextActive == active) return new AStarNode[0];
                previous = active;
                active = nextActive;

            }
            List<AStarNode> finalPath = GetPathToNode(active);
            totalDistance = active.PathLength;
            for (int i = 0; i < finalPath.Count - 1; ++i)
                connectionList.Add(connections.Single(x => x.HasConnectionTo(finalPath[i]) && x.HasConnectionTo(finalPath[i + 1])));
            return finalPath.ToArray();
        }

        private void ExpandNode(AStarNode active)
        {

            List<AStarNodeConnection> neighbours = GetNeighbours(active);
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
            }
            expandedNodes.Add(active);
        }

        private List<AStarNodeConnection> GetNeighbours(AStarNode targetNode) =>
            connections.Where(conn => conn.HasConnectionTo(targetNode)).ToList();

        private List<AStarNode> GetPathToNode(AStarNode targetNode)
        {
            var path = new Queue<AStarNode>();
            while (targetNode != null)
            {
                path.Enqueue(targetNode);
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
