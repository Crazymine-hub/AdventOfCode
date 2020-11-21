using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes.Day18
{
    class Node : Tools.Pathfinding.BaseNode
    {
        public char Lock { get; } = '\0';
        public char Key { get; } = '\0';
        bool unlocked;
        public bool IsUnlocked { get => unlocked || Lock == '\0'; set => unlocked = value; }
        public int PathIndex { get; set; }
        public Node BlockedBy { get; set; }

        public Node(int x, int y, char keyChar) : base(x, y)
        {
            if (keyChar >= 65 && keyChar <= 90)
                Lock = keyChar;
            else if (keyChar >= 97 && keyChar <= 122)
                Key = keyChar;
        }

        public override string ToString()
        {
            string koords = base.ToString().Remove(0, 1);
            if (BlockedBy != null)
                koords += "|" + BlockedBy.ToString();

            if (Key != '\0')
                return Key + koords;
            if (Lock != '\0')
                return Lock + koords;

            return "+" + koords;
        }

        public bool CanBeUnlocked(char key)
        {
            return (byte)key - 32 == Lock;
        }

        public bool TryUnlock(char key)
        {
            if (CanBeUnlocked(key))
            {
                unlocked = true;
                return true;
            }
            return false;
        }

        public int GetKeyBitPos()
        {
            return (int)Key - 97;
        }
    }
}
