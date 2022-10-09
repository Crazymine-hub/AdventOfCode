using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AdventOfCode.Days
{
    public class Day24 : DayBase
    {
        public override string Title => "Arithmetic Logic Unit";

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;

            return InterpretCode(input);
        }

        public string InterpretCode(string input)
        {
            StringBuilder code = new StringBuilder();
            var program = GetLines(input);
            code.AppendLine("private int SubmarineAlu(params int[] inputs)");
            code.AppendLine("{");
            code.AppendLine("    int inputIndex = 0;");
            code.AppendLine("    int w = 0;");
            code.AppendLine("    int x = 0;");
            code.AppendLine("    int y = 0;");
            code.AppendLine("    int z = 0;");

            code.AppendLine();
            code.AppendLine();

            Dictionary<string, bool> variableState = new Dictionary<string, bool>()
            {
                { "w", false},
                { "x", false},
                { "y", false},
                { "z", false},
            };

            int lineCounter = 0;
            int executedCnt = 0;
            foreach (var instruction in program)
            {
                ++lineCounter;
                var parts = instruction.Split(' ');
                switch (parts[0])
                {
                    case "inp":
                        if (CanSkipInput(variableState, parts)) continue;
                        code.AppendLine($"    {parts[1]} = inputs[inputIndex++];");
                        break;
                    case "add":
                        if (CanSkipAddition(variableState, parts)) continue;
                        code.AppendLine($"    {parts[1]} = {parts[1]} + {parts[2]};");
                        break;
                    case "mul":
                        if (CanSkipMultiplication(variableState, parts)) continue;
                        code.AppendLine($"    {parts[1]} = {parts[1]} * {parts[2]};");
                        break;
                    case "div":
                        if (CanSkipDiv(variableState, parts)) continue;
                        code.AppendLine($"    {parts[1]} = {parts[1]} / {parts[2]};");
                        break;
                    case "mod":
                        if (CanSkipMod(variableState, parts)) continue;
                        code.AppendLine($"    {parts[1]} = {parts[1]} % {parts[2]};");
                        break;
                    case "eql":
                        if (CanSkipCompare(variableState, parts)) continue;
                        code.AppendLine($"    {parts[1]} = {parts[1]} == {parts[2]} ? 1 : 0;");
                        break;
                }
                ++executedCnt;
            }

            code.AppendLine();
            code.AppendLine();

            code.AppendLine($"// Interpreted {executedCnt}/{lineCounter} instructions (Skipped {lineCounter - executedCnt})");
            code.AppendLine("return z;");
            code.AppendLine("}");
            return code.ToString();
        }

        private bool CanSkipInput(Dictionary<string, bool> variableState, string[] parts)
        {
            variableState[parts[1]] = true;
            return false;
        }

        private bool CanSkipAddition(Dictionary<string, bool> variableState, string[] parts)
        {
            if (variableState.ContainsKey(parts[2]))
                variableState[parts[1]] = variableState[parts[2]]; //assignment from somewhere else
            else
                variableState[parts[1]] = true; //uses a set value
            return !variableState[parts[1]];
        }

        private bool CanSkipMultiplication(Dictionary<string, bool> variableState, string[] parts)
        {
            if (parts[2] == "0")
                variableState[parts[1]] = false;
            else if (variableState.ContainsKey(parts[2]))
                variableState[parts[1]] = variableState[parts[2]]; //assignment from somewhere else
            else
                variableState[parts[1]] = true; //uses a set value
            return !variableState[parts[1]];
        }

        private bool CanSkipDiv(Dictionary<string, bool> variableState, string[] parts)
        {
            if (variableState.ContainsKey(parts[2]))
                variableState[parts[1]] = variableState[parts[2]]; //assignment from somewhere else
            return !variableState[parts[1]];
        }

        private bool CanSkipMod(Dictionary<string, bool> variableState, string[] parts)
        {
            if (variableState.ContainsKey(parts[2]))
                variableState[parts[1]] = variableState[parts[2]]; //assignment from somewhere else
            return !variableState[parts[1]];
        }

        private bool CanSkipCompare(Dictionary<string, bool> variableState, string[] parts)
        {
            variableState[parts[1]] = true;
            return false;
        }
    }
}
