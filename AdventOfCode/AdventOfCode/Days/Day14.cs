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
		public string Solve(string input, bool part2)
		{
			if (part2) return "Part 2 is unavailable";
			DataTable recipes = new DataTable();
			recipes.Columns.Add("Amount", 0.GetType());
			recipes.Columns.Add("Result", "".GetType());
			recipes.Columns.Add("Ingredients", new List<ReactionItem>().GetType());

			string[] recipeList = input.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var recipeLine in recipeList)
				BuildRecipe(recipeLine, recipes);
			return "";
		}

		private void BuildRecipe(string recipeText, DataTable recipeBook)
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
			DataRow recipe = recipeBook.NewRow();
			recipe.SetField("Amount", keyItem.Amount);
			recipe.SetField("Result", keyItem.Name);
			recipe.SetField("Ingredients", items);
			recipeBook.Rows.Add(recipe);
		}
	}
}
