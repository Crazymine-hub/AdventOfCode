using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day21
{
    class Ingredient
    {
        public const string NoAllergene = "";
        public const string UnknownAllergene = "?";

        public string Name { get; }
        public Dictionary<string, int> PossibleAllergenes { get; set; }
        public string ContainedAllergene { get; private set; } = NoAllergene;
        public bool Locked { get; set; } = false;

        public Ingredient(string name)
        {
            Name = name;
            PossibleAllergenes = new Dictionary<string, int>();
        }

        public void AddPossibleAllergene(string allergene)
        {
            if (!PossibleAllergenes.ContainsKey(allergene))
                PossibleAllergenes.Add(allergene, 0);
            ++PossibleAllergenes[allergene];
        }

        public void EvaluateAllergene()
        {
            int maxAllergeneCount = PossibleAllergenes.Max(x => x.Value);
            if (PossibleAllergenes.Count == 0 || maxAllergeneCount == -1)
            {
                ContainedAllergene = NoAllergene;
                return;
            }
            try
            {
                var allergene = PossibleAllergenes.SingleOrDefault(x => x.Value == maxAllergeneCount);
                ContainedAllergene = allergene.Key;
            }
            catch (InvalidOperationException)
            {
                ContainedAllergene = UnknownAllergene;
            }
        }
    }
}
