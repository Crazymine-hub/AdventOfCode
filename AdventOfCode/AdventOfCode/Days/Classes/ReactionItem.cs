using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Classes
{
    class Reaction
    {
        public ulong Output { get; private set; }
        public string Name { get; private set; }
        public ulong Required { get; set; }
        public ulong Produced { get => Output * Cycles; }
        public ulong Cycles { get; set; }

        public Dictionary<string, ulong> Ingredients { get; private set; } = new Dictionary<string, ulong>();

        public Reaction(string recipeText)
        {
            MatchCollection recipeItems = Regex.Matches(recipeText, @"(?<Quantity>\d+) (?<Element>\w+)");
            for (int i = 0; i < recipeItems.Count - 1; i++)
            {
                Match currItem = recipeItems[i];
                Ingredients.Add(currItem.Groups["Element"].Value, ulong.Parse(currItem.Groups["Quantity"].Value));
            }
            var keyItem = recipeItems[recipeItems.Count - 1];
            Output = ulong.Parse(keyItem.Groups["Quantity"].Value);
            Name = keyItem.Groups["Element"].Value;
        }

        public new string ToString()
        {
            return Output + " " + Name;
        }

        public string DetailString()
        {
            return Name.PadRight(10) + "|" +
                Output.ToString().PadRight(10) + "|" +
                Required.ToString().PadRight(20) + "|" +
                Produced.ToString().PadRight(20) + "|" +
                (Produced - Required).ToString();
        }
    }
}