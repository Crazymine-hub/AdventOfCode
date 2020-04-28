using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes
{
    struct ReactionItem
    {
        public int Amount { get; set; }
        public string Name { get; set; }

        public new string ToString()
        {
            return Amount + " " + Name;
        }
    }
}
