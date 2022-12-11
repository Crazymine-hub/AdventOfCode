using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day8
{
    internal class Tree
    {
        public int Size { get; }
        public bool IsVisible { get; set; }
        public int ScenicScore { get; set; }
        public Tree(int size)
        {
            Size = size;
            IsVisible = false;
            ScenicScore = 0;
        }
    }
}
