using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day5 : DayBase
    {
        public override string Title => "Supply Stacks";

        public override string Solve(string input, bool part2)
        {
            var cargoHandling = GetGroupedLines(input);
            List<Stack<char>> crates = GetCrates(cargoHandling[0]);
            RenderCargo(crates);
            foreach (var instruction in GetLines(cargoHandling[1]))
            {
                ExecuteInstruction(instruction, crates, part2);
                RenderCargo(crates);
                Task.Delay(1).Wait();
            }

            return $"The top containers are {string.Concat(crates.Select(x => x.Any() ? x.Peek() : ' '))}";
        }

        private List<Stack<char>> GetCrates(string cargoLayout)
        {
            List<Stack<char>> layout = new List<Stack<char>>();
            bool init = true;
            foreach (var cargoLine in GetLines(cargoLayout).Reverse<string>().Skip(1))
            {
                var crates = Regex.Match(cargoLine, @"^(?<crate>\[\w\] ?| {3} ?)+$");
                for (int i = 0; i < crates.Groups[1].Captures.Count; i++)
                {
                    var crate = crates.Groups[1].Captures[i];
                    if (init)
                        layout.Add(new Stack<char>());
                    if (string.IsNullOrWhiteSpace(crate.Value)) continue;
                    layout[i].Push(crate.Value[1]);
                }
                init = false;
            }
            return layout;
        }

        private void ExecuteInstruction(string instruction, List<Stack<char>> crates, bool multiCrate)
        {
            var instr = Regex.Match(instruction, @"^move (\d+) from (\d+) to (\d+)$");
            var repetitions = int.Parse(instr.Groups[1].Value);
            var srcIndex = int.Parse(instr.Groups[2].Value) - 1;
            var tgtIndex = int.Parse(instr.Groups[3].Value) -1;
            List<char> liftArm= new List<char>();
            for(int i = 0; i < repetitions; ++i)
                liftArm.Add(crates[srcIndex].Pop());

            if(multiCrate)
                liftArm.Reverse();

            foreach(var elem in liftArm)
                crates[tgtIndex].Push(elem);
        }

        private void RenderCargo(List<Stack<char>> crates)
        {
            Console.Clear();
            var height = crates.Max(x => x.Count);
            List<string> lines = new List<string>();
            lines.Add(string.Concat(Enumerable.Range(1, crates.Count).Select(x => $" {x} ")));
            lines.Add(string.Empty.PadLeft(lines.First().Length, '═'));
            var crateEnum = crates.Select(x => x.GetEnumerator()).Cast<object>().ToList();
            StringBuilder line = new StringBuilder();
            for(int lineIndex = 0; lineIndex < height; lineIndex++)
            {
                line.Clear();
                for(int col = 0; col < crates.Count; ++col)
                {
                    if (crates[col].Count + lineIndex < height)
                    {
                        line.Append(string.Empty.PadLeft(3));
                        continue;
                    }
                    var crate = (IEnumerator<char>)crateEnum[col];
                    if (!crate.MoveNext())
                    {
                        line.Append(string.Empty.PadLeft(3));
                        continue;
                    }
                        line.Append('[');
                        line.Append(crate.Current);
                        line.Append(']');
                }
                lines.Insert(2, line.ToString());
            }

            foreach(var renderLine in lines.Reverse<string>())
                Console.WriteLine(renderLine);
        }
    }
}
