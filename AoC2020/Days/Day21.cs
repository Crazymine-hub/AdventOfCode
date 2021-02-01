using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Days.Tools.Day21;

namespace AdventOfCode.Days
{
    public class Day21 : DayBase
    {
        public override string Title => "Allergen Assessment";

        List<Meal> meals = new List<Meal>();
        List<Ingredient> ingredients = new List<Ingredient>();
        Dictionary<string, Ingredient> allergenes = new Dictionary<string, Ingredient>();
        public override string Solve(string input, bool part2)
        {
            foreach (var line in GetLines(input))
                meals.Add(ExtractIngredients(new Meal(line)));
            foreach (Ingredient ingredient in ingredients)
                ingredient.EvaluateAllergene();

            const int startIndex = 0x2080;

            bool wasReevaluated = true;
            while (wasReevaluated)
            {
                foreach (Meal meal in meals)
                {
                    var mealIngredients = meal.Ingredients.Select(x => ingredients.Single(y => y.Name == x));
                    wasReevaluated = false;
                    foreach (string allergene in meal.Allergenes)
                    {
                        int maxAllergene = mealIngredients.Max(x => x.PossibleAllergenes[allergene]);
                        var allergeneIngredient = mealIngredients.Where(x => x.PossibleAllergenes[allergene] == maxAllergene).ToList();
                        if (allergeneIngredient.Count == 1)
                        {
                            foreach (var ingredient in mealIngredients)
                            {
                                if (ingredient != allergeneIngredient[0])
                                {
                                    if (ingredient.PossibleAllergenes[allergene] != -1)
                                    {
                                        ingredient.PossibleAllergenes[allergene] = -1;
                                        wasReevaluated = true;
                                    }
                                }
                                else
                                {
                                    var otherAllergenes = ingredient.PossibleAllergenes.Where(x => x.Key != allergene).ToList();
                                    foreach (var otherAllergene in otherAllergenes)
                                        ingredient.PossibleAllergenes[otherAllergene.Key] = -1;
                                }
                                ingredient.EvaluateAllergene();
                            }
                        }
                    }
                }
            }

            var allergeneFree = ingredients.Where(x => x.ContainedAllergene == Ingredient.NoAllergene);
            var allergeneUnknown = ingredients.Where(x => x.ContainedAllergene == Ingredient.UnknownAllergene);
            var allergeneContained = ingredients.Where(x => x.ContainedAllergene != Ingredient.UnknownAllergene && x.ContainedAllergene != Ingredient.NoAllergene);
            if (allergeneUnknown.Count() != 0) throw new Exception("OOPS");
            int occurences = 0;
            foreach (Ingredient ingredient in allergeneFree)
                occurences += meals.Count(x => x.Ingredients.Contains(ingredient.Name));


            foreach (Meal meal in meals)
            {
                foreach (Ingredient ingredient in ingredients.Where(x => meal.Ingredients.Contains(x.Name)))
                {
                    Console.Write(ingredient.Name);
                    int index = meal.Allergenes.IndexOf(ingredient.ContainedAllergene);
                    if (index >= 0)
                        Console.Write(Convert.ToChar(startIndex + index));
                    Console.Write(" ");
                }
                Console.WriteLine("");
                Console.Write("(");
                for (int i = 0; i < meal.Allergenes.Count; ++i)
                {
                    Console.Write(i + "=");
                    Console.Write(meal.Allergenes[i]);
                    Console.Write(" ");
                }
                Console.WriteLine(")");
                Console.WriteLine("");
            }

            Console.WriteLine("Dangerous ingredients:");
            foreach(var ingredient in allergeneContained.OrderBy(x => x.ContainedAllergene))
            {
                Console.Write(ingredient.Name);
                Console.Write(",");
            }
            --Console.CursorLeft;

            Console.WriteLine(" ");

            return string.Format("Found {0} allergene free ingredient occurences.", occurences);
        }

        private Meal ExtractIngredients(Meal meal)
        {
            foreach (string ingredient in meal.Ingredients)
            {
                Ingredient currIngredient = ingredients.SingleOrDefault(x => x.Name == ingredient);
                if (currIngredient == null)
                {
                    currIngredient = new Ingredient(ingredient);
                    ingredients.Add(currIngredient);
                }
                foreach (string allergene in meal.Allergenes)
                {
                    currIngredient.AddPossibleAllergene(allergene);
                    if (!allergenes.ContainsKey(allergene))
                        allergenes.Add(allergene, null);
                }
            }
            return meal;
        }
    }
}

//foreach (Meal meal in meals)
//{
//    var mealIngredients = meal.Ingredients.Select(x => ingredients.Single(y => y.Name == x));
//    bool wasReevaluated = true;
//    while (wasReevaluated)
//    {
//        wasReevaluated = false;
//        foreach (string allergene in meal.Allergenes)
//        {
//            int maxAllergene = mealIngredients.Max(x => x.PossibleAllergenes[allergene]);
//            var allergeneIngredient = mealIngredients.Where(x => x.PossibleAllergenes[allergene] == maxAllergene).ToList();
//            if (allergeneIngredient.Count == 1)
//            {
//                foreach (var ingredient in mealIngredients)
//                {
//                    if (ingredient != allergeneIngredient[0])
//                    {
//                        if (ingredient.PossibleAllergenes[allergene] != -1)
//                        {
//                            ingredient.PossibleAllergenes[allergene] = -1;
//                            wasReevaluated = true;
//                        }
//                    }
//                    else
//                    {
//                        var otherAllergenes = ingredient.PossibleAllergenes.Where(x => x.Key != allergene).ToList();
//                        foreach (var otherAllergene in otherAllergenes)
//                            ingredient.PossibleAllergenes[otherAllergene.Key] = -1;
//                    }
//                    ingredient.EvaluateAllergene();
//                }
//            }
//        }
//    }
//}