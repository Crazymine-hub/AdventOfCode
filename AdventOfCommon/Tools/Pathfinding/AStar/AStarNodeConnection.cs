using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Pathfinding.AStar
{
    public class AStarNodeConnection : BaseNodeConnection
    {
        public new AStarNode NodeA { get => (AStarNode)base.NodeA; protected set => base.NodeA = value; }
        public new AStarNode NodeB { get => (AStarNode)base.NodeB; protected set => base.NodeB = value; }

        public AStarNodeConnection(AStarNode a, AStarNode b, ConnectionDirection direction = ConnectionDirection.Both) : base(a, b, direction)
        {
        }

        private new BaseNode GetOtherNode(BaseNode node) => base.GetOtherNode(node);
        public virtual AStarNode GetOtherNode(AStarNode node) =>(AStarNode)GetOtherNode((BaseNode)node);
    }
}
