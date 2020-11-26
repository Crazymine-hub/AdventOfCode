using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Pathfinding
{
    public class BaseNodeConnection
    {
        public virtual BaseNode NodeA { get; protected set; }
        public virtual BaseNode NodeB { get; protected set; }
        protected double distance;
        public virtual double Distance => distance;
        public bool IsHorizontal { get; protected set; }
        public double Rating { get; set; }

        public BaseNodeConnection(BaseNode a, BaseNode b)
        {
            NodeA = a;
            NodeB = b;
            RefreshDistance();
            IsHorizontal = NodeA.X != NodeB.X;
        }

        public void RefreshDistance()
        {
            distance = Math.Sqrt(Math.Pow(NodeB.X - NodeA.X, 2) + Math.Pow(NodeB.Y - NodeA.Y, 2));
        }

        public bool HasConnectionTo(BaseNode target)
        {
            return (NodeA == target || NodeB == target);
        }

        public BaseNode GetOtherNode(BaseNode target)
        {
            if (NodeA == target)
                return NodeB;
            else if (NodeB == target)
                return NodeA;
            else
                throw new ArgumentException("Node not in this connection");
        }

        public static void PrintConnections(List<BaseNodeConnection> connections, int vOffset, bool bold = false)
        {
            foreach (BaseNodeConnection con in connections)
            {
                Console.SetCursorPosition(con.NodeA.X, con.NodeA.Y + vOffset);
                if (con.NodeA.CharRepresentation != '\0')
                    Console.Write(con.NodeA.CharRepresentation);
                else
                    Console.Write(TraceChars.paths[con.NodeA.PathIndex | (bold ? 16 : 0)]);


                if (con.IsHorizontal)
                {
                    for (int i = Math.Min(con.NodeA.X, con.NodeB.X) + 1; i < Math.Max(con.NodeA.X, con.NodeB.X); i++)
                    {
                        Console.CursorLeft = i;
                        Console.Write(TraceChars.GetPathChar(false, false, true, true, bold));
                    }
                }
                else
                {
                    for (int i = Math.Min(con.NodeA.Y, con.NodeB.Y) + 1; i < Math.Max(con.NodeA.Y, con.NodeB.Y); i++)
                    {
                        Console.CursorLeft--;
                        Console.CursorTop = i + vOffset;
                        Console.Write(TraceChars.GetPathChar(true, true, false, false, bold));
                    }
                }

                Console.SetCursorPosition(con.NodeB.X, con.NodeB.Y + vOffset);
                if (con.NodeB.CharRepresentation != '\0')
                    Console.Write(con.NodeB.CharRepresentation);
                else
                    Console.Write(TraceChars.paths[con.NodeB.PathIndex | (bold ? 16 : 0)]);
            }
        }

        public new string ToString()
        {
            return NodeA + " <-> " + NodeB + " @" + distance;
        }
    }
}
