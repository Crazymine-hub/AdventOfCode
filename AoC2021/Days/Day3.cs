using AdventOfCode.Tools;
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
            List<string> report = GetLines(input);

            if (part2)
                return GetLifeSupportData(report);
            return GetPowerLevel(report);
        }

        private string GetPowerLevel(List<string> report)
        {
            int[] bitCount = CountBits(report);
            int gamma = 0;
            for (int i = 0; i < bitCount.Length; i++)
                if (IsCommonSet(report.Count, bitCount[i]))
                    gamma = Bitwise.SetBit(gamma, bitCount.Length - 1 - i, true);
            long epsilon = ~gamma & Bitwise.GetBitMask<long>(bitCount.Length);
            return $"Gamma: {gamma} Epsilon: {epsilon} Power Level: {gamma * epsilon}";
        }


        private string GetLifeSupportData(List<string> report)
        {
            int o2Rating = GetSystemData(true, report);
            int co2Rating = GetSystemData(false, report);
            return $"O2 Rating: {o2Rating} Co2 Rating: {co2Rating} {o2Rating * co2Rating}";
        }

        private int GetSystemData(bool mostCommon, List<string> report)
        {
            List<string> searchData = report;
            int position = 0;
            while (searchData.Count > 1)
            {
                int[] bitCount = CountBits(searchData);
                bool isCommonSet = IsCommonSet(searchData.Count, bitCount[position]);
                searchData = searchData.Where(x => x[position] == (mostCommon ^ isCommonSet ? '0' : '1')).ToList();
                ++position;
            }

            string resultBinary = searchData.Single();
            int result = 0;
            for(int i = 0; i < resultBinary.Length; ++i)
            {
                if (resultBinary[i] == '1')
                    result = Bitwise.SetBit(result, resultBinary.Length - 1 - i, true);
            }
            return result;
        }

        private int[] CountBits(IEnumerable<string> report)
        {
            int[] bitCount = new int[report.Max(x => x.Length)];
            foreach (string diagValue in report)
                for (int i = 0; i < diagValue.Length; i++)
                {
                    if (diagValue[i] == '1')
                        ++bitCount[i];
                }
            return bitCount;
        }
        private static bool IsCommonSet(int reportLength, int bitCountValue) => !(bitCountValue < reportLength / 2.0);
    }
}
