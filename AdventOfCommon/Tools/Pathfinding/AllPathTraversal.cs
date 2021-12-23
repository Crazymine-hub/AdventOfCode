using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Pathfinding
{
    public class AllPathTraversal
    {
        private readonly BaseNodeConnection[] connections;
        private readonly BaseNode start;
        private readonly BaseNode end;
        private readonly Func<IReadOnlyList<BaseNode>, BaseNode, bool> pathAllowedCheck;

        public AllPathTraversal(IEnumerable<BaseNodeConnection> connections, BaseNode start, BaseNode end) : this(connections, start, end, DefaultPathChecker)
        {

        }

        public AllPathTraversal(IEnumerable<BaseNodeConnection> connections, BaseNode start, BaseNode end, Func<IReadOnlyList<BaseNode>, BaseNode, bool> pathAllowedCheck)
        {
            this.connections = connections?.ToArray() ?? throw new ArgumentNullException(nameof(connections));
            this.start = start ?? throw new ArgumentNullException(nameof(start));
            this.end = end ?? throw new ArgumentNullException(nameof(end));
            this.pathAllowedCheck = pathAllowedCheck;
        }

        public List<List<BaseNode>> FindAllPaths()
        {
            return FollowAllConnections(start, new List<BaseNode>() { start });
        }

        private List<List<BaseNode>> FollowAllConnections(BaseNode originNode, List<BaseNode> path)
        {
            var foundConnections = new List<List<BaseNode>>();
            foreach (var connection in connections.Where(x => x.HasConnectionTo(originNode)))
            {
                var targetNode = connection.GetOtherNode(originNode);
                if (!pathAllowedCheck(path.AsReadOnly(), targetNode)) continue;
                var basePath = new List<BaseNode>();
                basePath.AddRange(path);
                basePath.Add(targetNode);
                if (connection.HasConnectionTo(end))
                {
                    foundConnections.Add(basePath);
                    continue;
                }
                foundConnections.AddRange(FollowAllConnections(targetNode, basePath));
            }
            return foundConnections;
        }
        private static bool DefaultPathChecker(IReadOnlyList<BaseNode> path, BaseNode targetNode)
        {
            return !path.Contains(targetNode);
        }
    }
}
