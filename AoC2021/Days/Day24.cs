using AdventOfCode.Days.Tools.Day24;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AdventOfCode.Days
{
    public class Day24 : DayBase
    {

        public override string Title => "Arithmetic Logic Unit";

        private List<ProgramStep> parameters;

        private const string programPattern =
            @"inp w\r\n" +
            @"mul x 0\r\n" +
            @"add x z\r\n" +
            @"mod x 26\r\n" +
            //lang=regex
            @"div z (?<zLimit>-?\d+)\r\n" +
            //lang=regex
            @"add x (?<xOffset>-?\d+)\r\n" +
            @"eql x w\r\n" +
            @"eql x 0\r\n" +
            @"mul y 0\r\n" +
            @"add y 25\r\n" +
            @"mul y x\r\n" +
            @"add y 1\r\n" +
            @"mul z y\r\n" +
            @"mul y 0\r\n" +
            @"add y w\r\n" +
            //lang=regex
            @"add y (?<yOffset>-?\d+)\r\n" +
            @"mul y x\r\n" +
            @"add z y";

        public override string Solve(string input, bool part2)
        {
            LoadCode(input);
            return GetModelNumber(!part2);
        }

        public void LoadCode(string input)
        {
            var code = Regex.Matches(input, programPattern);
            if (code.Count != 14) throw new FormatException("The Code doesn't match the expected pattern 14 times.");

            parameters = new List<ProgramStep>();

            var result = 0;
            var inEncodeLevel = 0;
            for (int i = 0; i < code.Count; ++i)
            {
                var section = code[i];
                int xOffset = int.Parse(section.Groups["xOffset"].Value);
                int yOffset = int.Parse(section.Groups["yOffset"].Value);
                int zLimit = int.Parse(section.Groups["zLimit"].Value);
                if (zLimit != 1 && zLimit != 26) throw new ArgumentException($"The provided ZLimit value must be either 1 or 26 (was {zLimit})");
                var step = new ProgramStep(xOffset, yOffset, zLimit, inEncodeLevel);
                parameters.Add(step);
                if (step.WantsEncode)
                    ++inEncodeLevel;
                else --inEncodeLevel;
            }
        }

        private string GetModelNumber(bool largest)
        {
            var digits = Enumerable.Range(1, 9);
            var states = new List<ModelNumberState>()
            {
                new ModelNumberState(-1, -1, 0, null)
            };

            for (int i = 0; i < parameters.Count; i++)
            {
                ProgramStep param = parameters[i];
                var previousStates = states.Where(x => x.Level == i - 1).ToHashSet();
                foreach (var state in previousStates)
                {
                    foreach (var digit in digits.Reverse())
                    {
                        if (WillEncode(digit, state.ZResult, param.XOffset) != param.WantsEncode) continue;
                        var result = VerifyDigit(digit, state.ZResult, param);
                        states.Add(new ModelNumberState(i, digit, result, state.Level < 0 ? null : state));
                    }
                }
            }

            var modelNumber = new int[14];

            var stateGroups = states.GroupBy(x => x.Level).ToHashSet();
            var validStates = states.Where(x => x.Level == parameters.Count - 1 && x.ZResult == 0).ToHashSet();
            var modelNumbers = validStates.Select(state =>
            {
                int[] modelNr = new int[state.Level + 1];
                var currentState = state;
                while(currentState != null)
                {
                    modelNr[currentState.Level] = currentState.Digit;
                    currentState = currentState.ParentState;
                }
                return string.Concat(modelNr);
            }).Distinct().OrderByDescending(x => x).ToList();

            return largest ? modelNumbers.First() : modelNumbers.Last();
        }

        private int VerifyDigit(int inputDigit, int z, ProgramStep param)
        {
            bool x = WillEncode(inputDigit, z, param.XOffset);

            z /= param.ZLimit;

            if (x)
            {
                z *= 26;
                z += inputDigit + param.YOffset;
            }
            return z;
        }

        private static bool WillEncode(int inputDigit, int z, int xOffset)
        {
            return ((z % 26) + xOffset) != inputDigit;
        }
    }
}
