using AdventOfCode.Tools.Pathfinding.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Pathfinding
{
    public class AStar
    {
        readonly List<BaseNodeConnection> connections;
        List<AStarNode> nodes;

        public double DistanceHeatWeigh { get; set; } = 0.5;


        public AStar(List<BaseNodeConnection> connectionList)
        {
            connections = connectionList;
            nodes = connections
                .SelectMany(x => new BaseNode[] { x.NodeA, x.NodeB })
                .Distinct()
                .Select(x => new AStarNode(x))
                .ToList();
        }

        public BaseNode[] GetPath(BaseNode start, BaseNode finish, Func<AStarNode, double> heuristicAnalyzer = null)
        {
            return GetPath(start, finish, out _, out _, heuristicAnalyzer);
        }

        public BaseNode[] GetPath(BaseNode start, BaseNode finish, out double cost, Func<AStarNode, double> heuristicAnalyzer = null)
        {
            return GetPath(start, finish, out _, out cost, heuristicAnalyzer);
        }

        public BaseNode[] GetPath(BaseNode start,
                                  BaseNode finish,
                                  out List<BaseNodeConnection> connectionList,
                                  out double pathCost,
                                  Func<AStarNode, double> heuristicAnalyzer = null)
        {
            List<AStarNode> processingNodes = new List<AStarNode>();
            processingNodes.AddRange(nodes);

            AStarNode active = processingNodes.Single(x => x.Node == start);
            AStarNode endNode = processingNodes.Single(x => x.Node == finish);
            active.PathCost = 0;
            pathCost = 0;
            connectionList = new List<BaseNodeConnection>();

            foreach (var node in processingNodes)
                node.DistanceToTarget = node.Node.GetDistanceTo(finish);

            while (active != endNode)
            {
                processingNodes.Remove(active);
                List<BaseNodeConnection> neighbours = GetNeighbours(active.Node);
                pathCost = active.PathCost;

                foreach (var connection in neighbours)
                {
                    if (connection.Distance < 0) continue;
                    BaseNode connTarget = connection.GetOtherNode(active.Node);
                    AStarNode targetDetails = processingNodes.SingleOrDefault(x => x.Node == connTarget);
                    if (targetDetails == null || targetDetails.PathCost <= pathCost) continue;
                    targetDetails.PathCost = pathCost + connection.Distance;
                    targetDetails.PreviousNode = active;
                    targetDetails.FullCost = heuristicAnalyzer?.Invoke(targetDetails) ?? DefaultHeuristic(targetDetails);
                }
                active = processingNodes.OrderBy(x => x.FullCost).First();
            }
            List<AStarNode> finalPath = GetPathToNode(active);
            pathCost = active.PathCost;
            for (int i = 0; i < finalPath.Count - 1; ++i)
                connectionList.Add(connections.Single(x => x.HasConnectionTo(finalPath[i].Node) && x.HasConnectionTo(finalPath[i + 1].Node)));
            return finalPath.Select(x => x.Node).ToArray();
        }

        private double DefaultHeuristic(AStarNode arg) => arg.DistanceToTarget + arg.PathCost;

        private List<BaseNodeConnection> GetNeighbours(BaseNode targetNode) =>
            connections.Where(conn => conn.HasConnectionTo(targetNode)).ToList();

        private List<AStarNode> GetPathToNode(AStarNode targetNode)
        {
            var path = new List<AStarNode>();
            while (targetNode != null)
            {
                path.Insert(0, targetNode);
                targetNode = targetNode.PreviousNode;
            }
            return path;
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
