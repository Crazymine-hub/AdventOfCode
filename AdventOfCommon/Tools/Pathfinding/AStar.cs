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
        List<BaseNode> path;
        List<BaseNode> ignored;


        public AStar(List<BaseNodeConnection> connectionList)
        {
            connections = connectionList;
        }

        public BaseNode[] GetPath(BaseNode start, BaseNode finish)
        {
            return GetPath(start, finish, out List<BaseNodeConnection> cons, out int cost);
        }

        public BaseNode[] GetPath(BaseNode start, BaseNode finish, out List<BaseNodeConnection> connectionList, out int pathCost)
        {
            path = new List<BaseNode>();
            ignored = new List<BaseNode>();
            path.Add(start);
            var active = start;
            pathCost = 0;
            connectionList = new List<BaseNodeConnection>();

            while (active != finish)
            {
                var neighbours = GetUntracedConnections(active);

                foreach (var currConn in neighbours)
                {
                    var connTarget = currConn.GetOtherNode(active);
                    currConn.Rating = pathCost + currConn.Distance + connTarget.DistanceToTarget;
                    if (currConn.Distance < 0)
                        currConn.Rating = -1;
                }

                var chosen = neighbours.OrderBy(conn => conn.Rating).FirstOrDefault(conn => conn.Rating >= 0);
                if (chosen == null)
                {
                    if (active == start) break;
                    ignored.Add(active);
                    path.Remove(active);
                    pathCost -= connectionList.Last().Distance;
                    connectionList.Remove(connectionList.Last());
                }
                else
                {
                    path.Add(chosen.GetOtherNode(active));
                    pathCost += chosen.Distance;
                    connectionList.Add(chosen);
                }
                active = path.Last();
            }
            return path.ToArray();
        }

        private List<BaseNodeConnection> GetUntracedConnections(BaseNode targetNode)
        {
            var cons = connections.Where(conn => conn.HasConnectionTo(targetNode)).ToList();
            foreach (var usedNode in path.Concat(ignored))
                cons = cons.Where(conn => conn.GetOtherNode(targetNode) != usedNode).ToList();
            return cons;
        }
    }
}
