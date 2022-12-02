using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Intersects many <see cref="IEnumerable{T}"/> into one.
        /// </summary>
        /// <typeparam name="T">The Type contained in the <see cref="IEnumerable{T}"/></typeparam>
        /// <param name="lists">A Collection of the collections to intersect</param>
        /// <param name="fill">Wether to fill in default values if the collection doesn't contain an element.</param>
        /// <returns>A collection of the collections elements intersected.</returns>
        public static IEnumerable<T> IntersectMany<T>(IEnumerable<IEnumerable<T>> lists, bool fill = false)
        {
            int index = 0;
            int maxLength = lists.Max(x => x.Count());
            do
            {
                foreach (IEnumerable<T> list in lists)
                {
                    T elem = list.ElementAtOrDefault(index);
                    T defaultElement = default(T);
                    if (!elem.Equals(defaultElement))
                        yield return elem;
                    if(fill) yield return defaultElement;
                }
                index++;
            } while (index < maxLength);
        }

    }
}
