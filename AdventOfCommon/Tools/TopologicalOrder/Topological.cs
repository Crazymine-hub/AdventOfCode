using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.TopologicalOrder
{
    public class Topological<T> where T : TopoItem
    {
        HashSet<string> isTraced = new HashSet<string>();
        public List<T> Ordered { get; private set; } = new List<T>();

        public Topological(List<T> items)
        {
            foreach (T item in items)
                if (!isTraced.Contains(item.Name))
                    Trace(item, items);
            Ordered.Reverse();
        }

        private void Trace(T item, List<T>items)
        {
            isTraced.Add(item.Name);

            foreach (var dependency in items.Where(x => item.IsDependantOn(x.Name)))
                if (!isTraced.Contains(dependency.Name))
                {
                    Trace(dependency, items);
                }

            Ordered.Add(item);
        }
    }
}
