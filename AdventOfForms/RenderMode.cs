using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Visualization
{
    [Flags]
    public enum RenderMode
    {
        None = 0,
        Console = 1,
        Image = 2,
        Both = Console | Image,
    }
}
