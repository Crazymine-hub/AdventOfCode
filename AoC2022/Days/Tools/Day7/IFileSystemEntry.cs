using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day7
{
    internal interface IFileSystemEntry
    {
        int Size { get; }
        string Name { get; }
        Directory ParentEntry { get; }
    }
}
