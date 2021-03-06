﻿using System;
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
