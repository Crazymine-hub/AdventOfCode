using AdventOfCode.Days.Classes.Day14;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day14 : DayBase
    {
        readonly char[] progressIndicator = new char[] { '|', '/', '─', '\\' };
        int progressPos = 0;
        List<Reaction> recipes = new List<Reaction>();

        public override string Solve(string input, bool part2)
        {
            //if (part2) return "Part 2 is unavailable";
            LoadRecipes(input.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));

            if (part2)
            {
                Console.Write("Berechne FUEL für 1.000.000.000.000 ORE... ");
                ApproachOreTarget();
            }
            else
            {
                Console.Write("Berechne für 1 FUEL... ");
                recipes.Single(x => x.Name == "FUEL").Required = 1;
                TraceRecipe();
            }
            Console.WriteLine();
            return CreateOutput();
        }

        private void TraceRecipe()
        {
            for(int i = 0; i < recipes.Count; i++)
            {
                Console.CursorLeft--;
                Console.Write(NextProgressChar());
                Reaction item = recipes[i];
                item.Cycles = (ulong)Math.Ceiling(item.Required / (double)item.Output);
                if (item.Name != "ORE")
                    foreach (var ingredient in item.Ingredients)
                    {
                        Console.CursorLeft--;
                        Console.Write(NextProgressChar());
                        Reaction ingredientItem = recipes.Single(x => ingredient.Key == x.Name);
                        ingredientItem.Required += item.Cycles * ingredient.Value;
                    }
            }
        }

        private void ApproachOreTarget()
        {
            int loadL = Console.CursorLeft;
            int loadT = Console.CursorTop;
            int logL = loadL + 2;
            ulong requiredOre = 0;
            ulong target = Convert.ToUInt64(1000000000000);
            ulong uBound = target;
            ulong lBound = 0;
            while (lBound < uBound)
            {
                ulong mid = (uBound + lBound) / 2;
                Reset();
                Console.SetCursorPosition(loadL, loadT);
                recipes.Single(x => x.Name == "FUEL").Required = mid;
                TraceRecipe();
                requiredOre = recipes.Single(x=>x.Name == "ORE").Required;
                Console.SetCursorPosition(logL++, loadT);
                if (requiredOre > target)
                {
                    Console.Write("D");
                    uBound = mid;
                }
                else if (requiredOre < target)
                {
                    if (mid == lBound) return;
                    Console.Write("U");
                    lBound = mid;
                }
                else
                    return;
            }
        }

        public void Reset()
        {
            foreach (var recipe in recipes)
            {
                recipe.Required = 0;
                recipe.Cycles = 0;
            }
        }

        private void LoadRecipes(string[] recipeList)
        {
            Console.WriteLine("Lade Rezepte... ");
            recipes.Add(new Reaction("1 ORE => 1 ORE"));
            foreach (var recipeLine in recipeList)
                recipes.Add(new Reaction(recipeLine));
            Console.WriteLine(recipes.Count - 1 + " Rezepte geladen.");
            recipes = new Topological(recipes).Ordered;
        }

        private string CreateOutput()
        {
            string result = "Name".PadRight(10) + "|" +
                "Ergebnis".PadRight(10) + "|" +
                "Benötigt".PadRight(20) + "|" +
                "Produziert".PadRight(20) + "|" +
                "Überschuss\r\n";
            foreach (Reaction reaction in recipes)
            {
                result += reaction.DetailString();
                result += "\r\n";
            }
            return result;
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