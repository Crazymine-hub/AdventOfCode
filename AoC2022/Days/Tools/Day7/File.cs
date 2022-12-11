using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day7
{
    internal class File : IFileSystemEntry
    {
        public int Size { get; }

        public string Name { get; }

        public Directory ParentEntry { get; }

        public File(string name, int size, Directory parentEntry)
        {
            Name = name;
            Size = size;
            ParentEntry = parentEntry;
        }

        public override string ToString() => Name + " (FILE) " + Size.ToString();
    }
}
