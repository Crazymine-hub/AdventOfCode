using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day7
{
    internal class Directory : IFileSystemEntry
    {
        int? _size = null;
        public int Size
        {
            get
            {
                if (_size == null)
                    _size = Children.Sum(x => x.Size);
                return _size.Value;
            }
        }

        public string Name { get; }

        public Directory ParentEntry { get; }

        public List<IFileSystemEntry> Children { get; private set; } = new List<IFileSystemEntry>();

        public Directory(string name, Directory parentEntry)
        {
            Name = name;
            ParentEntry = parentEntry;
        }

        public void SortChildren() => Children = Children.OrderBy(x => x.Name).ToList();
        public override string ToString() => Name + " (DIR) " + Size.ToString();
    }
}
