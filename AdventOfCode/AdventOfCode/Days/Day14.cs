using AdventOfCode.Days.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day14 : IDay
    {
        DataTable recipes = new DataTable();
        readonly char[] progressIndicator = new char[] { '|', '/', '─', '\\' };
        int progressPos = 0;

        public string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";
            recipes.Columns.Add("Amount", 0.GetType());
            recipes.Columns.Add("Result", "".GetType());
            recipes.Columns.Add("Ingredients", new List<ReactionItem>().GetType());
            recipes.Columns.Add("Produced", 0.GetType());
            recipes.Columns.Add("Used", 0.GetType());

            string[] recipeList = input.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            BuildRecipe("1 ORE => 1 ORE");
            foreach (var recipeLine in recipeList)
                BuildRecipe(recipeLine);

            Console.Clear();
            Console.Write("Bitte Warten... ");
            TraceRecipe("FUEL", 1);

            StringBuilder result = new StringBuilder();
            result.Append("Item\t");
            result.Append("Result\t");
            result.Append("Used/");
            result.AppendLine("Produced");
            foreach (DataRow recipe in recipes.Rows)
                result.AppendLine(RecipeToString(recipe));
            Console.Clear();
            return result.ToString();
        }

        private void BuildRecipe(string recipeText)
        {
            MatchCollection recipeItems = Regex.Matches(recipeText, @"(?<Quantity>\d+) (?<Element>\w+)");
            List<ReactionItem> items = new List<ReactionItem>();
            for (int i = 0; i < recipeItems.Count; i++)
            {
                Match currItem = recipeItems[i];
                var itemInfo = new ReactionItem()
                {
                    Amount = int.Parse(currItem.Groups["Quantity"].Value),
                    Name = currItem.Groups["Element"].Value
                };
                items.Add(itemInfo);
            }
            var keyItem = items.Last();
            items.RemoveAt(items.Count - 1);
            DataRow recipe = recipes.NewRow();
            recipe.SetField("Amount", keyItem.Amount);
            recipe.SetField("Result", keyItem.Name);
            recipe.SetField("Ingredients", items);
            recipe.SetField("Produced", 0);
            recipe.SetField("Used", 0);
            recipes.Rows.Add(recipe);
        }

        private void TraceRecipe(string element, int required)
        {
            var elementRecipes = recipes.Select($"Result = '{element}'");
            if (elementRecipes.Length != 1) throw new InvalidOperationException("Can't identify Recipe");


            var reactionAmount = elementRecipes[0].Field<int>("Amount");
            var production = elementRecipes[0].Field<int>("Produced");
            var usage = elementRecipes[0].Field<int>("Used");
            usage += required;
            elementRecipes[0].SetField("Used", usage);

            if (element == "ORE") return;

            var ingredients = elementRecipes[0].Field<List<ReactionItem>>("Ingredients");

            while (production < usage)
            {
                Console.CursorLeft--;
                Console.Write(NextProgressChar());
                foreach (ReactionItem ingredient in ingredients)
                {
                    TraceRecipe(ingredient.Name, ingredient.Amount);
                }
                production += reactionAmount;
                elementRecipes[0].SetField("Produced", production);
            }
        }

        private string RecipeToString(DataRow recipe)
        {
            StringBuilder result = new StringBuilder();
            result.Append(recipe.Field<string>("Result"));
            result.Append("\t|");
            result.Append(recipe.Field<int>("Amount"));
            result.Append("\t|");
            result.Append(recipe.Field<int>("Used"));
            result.Append("/");
            result.Append(recipe.Field<int>("Produced"));
            return result.ToString();
        }

        private char NextProgressChar()
        {
            progressPos++;
            if (progressPos >= progressIndicator.Length)
                progressPos = 0;
            return progressIndicator[progressPos];
        }
    }
}
