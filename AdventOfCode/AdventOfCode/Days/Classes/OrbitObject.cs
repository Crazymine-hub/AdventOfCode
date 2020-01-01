using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes
{
    class OrbitObject
    {
        public string Name;
        public OrbitObject[] ChildObjects;
        public OrbitObject ParentObject;
        public int OrbitCount;

        public string ToString()
        {
            return Name;
        }
    }
}
