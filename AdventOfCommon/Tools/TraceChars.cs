using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools
{
    public static class TraceChars
    {
        //                                                  0    1    2    3    4    5    6    7    8    9    10   11   12   13   14   15   16   17   18   19   20   21   22   23   24   25   26   27   28   29   30   31
        public static readonly char[] paths = new char[] { ' ', '╵', '╷', '│', '╴', '┘', '┐', '┤', '╶', '└', '┌', '├', '─', '┴', '┬', '┼', ' ', '╹', '╻', '┃', '╸', '┛', '┓', '┫', '╺', '┗', '┏', '┣', '━', '┻', '┳', '╋' };
        //UP                                             1  0    1    0    1    0    1    0    1    0    1    0    1    0    1    0    1    0    1    0    1    0    1    0    1    0    1    0    1    0    1    0    1 
        //DOWN                                           2  0    0    1    1    0    0    1    1    0    0    1    1    0    0    1    1    0    0    1    1    0    0    1    1    0    0    1    1    0    0    1    1 
        //LEFT                                           4  0    0    0    0    1    1    1    1    0    0    0    0    1    1    1    1    0    0    0    0    1    1    1    1    0    0    0    0    1    1    1    1 
        //RIGHT                                          8  0    0    0    0    0    0    0    0    1    1    1    1    1    1    1    1    0    0    0    0    0    0    0    0    1    1    1    1    1    1    1    1 
        //BOLD                                          16  0    0    0    0    0    0    0    0    0    0    0    0    0    0    0    0    1    1    1    1    1    1    1    1    1    1    1    1    1    1    1    1 

        [Flags]
        public enum Direction
        {
            None = 0,
            Up = 1,
            Down = 2,
            Left = 4,
            Right = 8,
            Bold = 16,
            
            UpDown = Up | Down,
            LeftRight = Left | Right,
            
            UpLeft = Left | Up,
            DownLeft = Left | Down,
            
            UpRight = Up | Right,
            DownRight = Down | Right,

            LeftRightUP = LeftRight | Up,
            LeftRightDown = LeftRight | Down,

            UpDownLeft = UpDown | Left,
            UpDownRight = UpDown | Right,

            AllDriections = LeftRight | UpDown,
        }

        public static int GetPathNumber(bool up, bool down, bool left, bool right, bool bold)
        {
            int result = 0;
            result |= up ? 1 : 0;
            result |= down ? 2 : 0;
            result |= left ? 4 : 0;
            result |= right ? 8 : 0;
            result |= bold ? 16 : 0;
            return result;
        }

        public static char GetPathChar(bool up, bool down, bool left, bool right, bool bold)
        {
            return paths[GetPathNumber(up, down, left, right, bold)];
        }
    }
}
