using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day21
{
    class Meal
    {
        public List<string> Ingredients { get; set; }
        public List<string> Allergenes { get; private set; }

        public Meal(string mealDescription)
        {
            Ingredients = new List<string>();
            Allergenes = new List<string>();
            var match = Regex.Match(mealDescription, @"^(?:(\w+) )+\(contains (?:(\w+),? ?)+");
            foreach(Capture ingredient in match.Groups[1].Captures)
                Ingredients.Add(ingredient.Value);
            foreach(Capture allergene in match.Groups[2].Captures)
                Allergenes.Add(allergene.Value);
        }
    }
}
