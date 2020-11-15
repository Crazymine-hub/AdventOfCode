using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes.Day18
{
    class Node: Tools.Pathfinding.BaseNode
    {
        public char Lock { get; set; } = '\0';
        public char Key { get; set; } = '\0';
        public int PathIndex { get; set; }

        public Node(int x, int y, char keyChar): base(x, y)
        {
            if (keyChar >= 65 && keyChar <= 90)
                Lock = keyChar;
            else if (keyChar >= 97 && keyChar <= 122)
                Key = keyChar;
        }

        public override string ToString()
        {
            string koords = base.ToString().Remove(0, 1);
            if (Key != '\0')
                return Key +koords;
            if (Lock != '\0')
                return Lock + koords;
            return "+" + koords;
        }
    }
}
