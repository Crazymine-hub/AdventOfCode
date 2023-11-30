using AdventOfCode.Days.Tools.Day19;
using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day19: DayBase
    {
        public override string Title => "Not Enough Minerals";

        private List<BlueprintProcessor> _blueprints;
        private ConsoleAssist _consoleAssist = new ConsoleAssist();

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            _blueprints = GetLines(input).Select(x => new BlueprintProcessor(x)).ToList();
            var processingTasks = _blueprints.Select(x => x.RunBlueprintAsync()).ToList();
            _consoleAssist.WaitForAllTasks(processingTasks);

            Console.WriteLine("BLueprint quality lavels:");
            for (int i = 0; i < _blueprints.Count; ++i)
                Console.WriteLine($"BP {_blueprints[i].ID:00}: {processingTasks[i].Result}");

            return $"All Blueprint quality levels added up: {processingTasks.Sum(x => x.Result)}";
        }
    }
}
