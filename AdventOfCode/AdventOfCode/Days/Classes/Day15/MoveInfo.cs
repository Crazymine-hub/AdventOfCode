using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes.Day15
{
    class MoveInfo
    {
        public int Direction { get; set; }
        public bool IsIntersection { get; set; }
        public bool NorthBlocked { get; private set; }
        public bool WestBlocked { get; private set; }
        public bool SouthBlocked { get; private set; }
        public bool EastBlocked { get; private set; }

        public MoveInfo(int direction)
        {
            Direction = direction;
            IsIntersection = false;
            NorthBlocked = false;
            WestBlocked = false;
            SouthBlocked = false;
            EastBlocked = false;
        }

        public void SetDirectionStatus(int direction, bool value)
        {
            switch (direction & 3)
            {
                case 0:
                    NorthBlocked = value;
                    break;
                case 1:
                    WestBlocked = value;
                    break;
                case 2:
                    SouthBlocked = value;
                    break;
                case 3:
                    EastBlocked = value;
                    break;
            }
        }

        public int GetNextFreeDirection()
        {
            int reverse = InvertDirection(Direction);
            if (!NorthBlocked && reverse != 0)
                return 0;
            if (!WestBlocked && reverse != 1)
                return 1;
            if (!SouthBlocked && reverse != 2)
                return 2;
            if (!EastBlocked && reverse != 3)
                return 3;
            return -1;
        }

        public new string ToString()
        {
            string result = "MOVE: ";
            switch (Direction)
            {
                case 0:
                    result += "N";
                    break;
                case 1:
                    result += "W";
                    break;
                case 2:
                    result += "S";
                    break;
                case 3:
                    result += "E";
                    break;
            }

            result += " BLOCKED: ";
            if (NorthBlocked)
                result += "N";
            if (WestBlocked)
                result += "W";
            if (SouthBlocked)
                result += "S";
            if (EastBlocked)
                result += "E";
            if (IsIntersection)
                result += " I";
            return result;
        }

        public static int InvertDirection(int direction)
        {
            direction -= 2;
            if (direction < 0)
                direction += 4;
            return direction;
        }
    }
}
