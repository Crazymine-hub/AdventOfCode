using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes.Day14
{
    class Topological
    {
        HashSet<string> isTraced = new HashSet<string>();
        public List<Reaction> Ordered { get; private set; } = new List<Reaction>();

        public Topological(List<Reaction> recipes)
        {
            foreach (Reaction recipe in recipes)
                if (!isTraced.Contains(recipe.Name))
                    Trace(recipe, recipes);
            Ordered.Reverse();
        }

        private void Trace(Reaction recipe, List<Reaction>recipes)
        {
            isTraced.Add(recipe.Name);

            foreach (var ingredient in recipes.Where(x => recipe.Ingredients.ContainsKey(x.Name)))
                if (!isTraced.Contains(ingredient.Name))
                {

                    Trace(ingredient, recipes);
                }

            Ordered.Add(recipe);
        }
    }
}
