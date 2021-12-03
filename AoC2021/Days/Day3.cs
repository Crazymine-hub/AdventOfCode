using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day3 : DayBase
    {
        public override string Title => "Binary Diagnostic";

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            List<string> report = GetLines(input);
            int[] bitCount = new int[report.Max(x => x.Length)];
            foreach (string diagValue in report)
                CountBits(diagValue, ref bitCount);
            int gamma = 0;
            for (int i = 0; i < bitCount.Length; i++)
                if (bitCount[i] > report.Count / 2)
                    gamma = Tools.Bitwise.SetBit(gamma, bitCount.Length - 1 - i, true);
            long epsilon = ~gamma & Tools.Bitwise.GetBitMask(bitCount.Length);
            return $"Gamma: {gamma} Epsilon: {epsilon} Power Level: {gamma * epsilon}";
        }

        private void CountBits(string diagValue, ref int[] bitCount)
        {
            for (int i = 0; i < diagValue.Length; i++)
            {
                if (diagValue[i] == '1')
                    ++bitCount[i];
            }
        }
    }
}
