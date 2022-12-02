using AdventOfCode.Days.Tools.Day8;
using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    internal class Day8 : DayBase
    {
        public override string Title => "Seven Segment Search";

        /* Mapping
         *  _      1
         * |_|   2 4 8
         * |_| 16 32 64
         */

        private Dictionary<byte, DisplaySegmentInfo> digits = new Dictionary<byte, DisplaySegmentInfo>()
        {
            {0b1111011, new DisplaySegmentInfo(0, 0)},  //0
            {0b1001000, new DisplaySegmentInfo(0, 1)},  //1
            {0b0111101, new DisplaySegmentInfo(0, 2)},  //2
            {0b1101101, new DisplaySegmentInfo(0, 3)},  //3
            {0b1001110, new DisplaySegmentInfo(0, 4)},  //4
            {0b1100111, new DisplaySegmentInfo(0, 5)},  //5
            {0b1110111, new DisplaySegmentInfo(0, 6)},  //6
            {0b1001001, new DisplaySegmentInfo(0, 7)},  //7
            {0b1111111, new DisplaySegmentInfo(0, 8)},  //8
            {0b1101111, new DisplaySegmentInfo(0, 9)}   //9
        };

        private const int segmentCount = 7;

        public override string Solve(string input, bool part2)
        {
            foreach (var digit in digits)
                digit.Value.BitCount = Bitwise.CountSetBits(digit.Key);
            int[] uniqueDigitSegmentCount = digits.Values
                .Where(x => digits.Values.Count(y => y.BitCount == x.BitCount) == 1)
                .Select(x => x.BitCount)
                .ToArray();

            int sum = 0;
            foreach (string display in GetLines(input))
            {
                var displayData = display.Split('|');
                char[] segmentSplitter = { ' ' };
                var samples = displayData[0].Split(segmentSplitter, StringSplitOptions.RemoveEmptyEntries);
                var values = displayData[1].Split(segmentSplitter, StringSplitOptions.RemoveEmptyEntries);
                if (!part2)
                {
                    sum += values.Select(x => x.Length).Count(x => uniqueDigitSegmentCount.Contains(x));
                    continue;
                }

                var wiring = GetWiring(samples);
                int number = 0;
                foreach (var value in values)
                {
                    byte displayState = 0;
                    foreach (char segment in value)
                        displayState |= wiring[(byte)(1 << (segment - 'a'))];
                    number *= 10;
                    number += digits[displayState].Digit;
                }
                sum += number;
            }
            if (part2)
                return $"The Sum of all Values: {sum}";
            return $"Output digits with unique segment count: {sum}";
        }

        private Dictionary<byte, byte> GetWiring(string[] samples)
        {
            var mappings = samples.Select(sample =>
                {
                    byte map = 0;
                    foreach (char segment in sample)
                        map |= (byte)(1 << (segment - 'a'));
                    return map;
                })
                .ToDictionary<byte, byte, (int BitCount, byte[] Digits)>(map => map, map =>
                {
                    int bitCount = Bitwise.CountSetBits(map);
                    byte[] possibleDigits = digits
                        .Where(digit => digit.Value.BitCount == bitCount)
                        .Select(digit => digit.Key)
                        .ToArray();
                    return (bitCount, possibleDigits);
                });

            // Real segment -> lettered segment
            Dictionary<byte, SegmentGroupInfo> segmentMapping = Enumerable.Range(0, segmentCount).ToDictionary(x => (byte)(1 << x), x => new SegmentGroupInfo());
            Dictionary<int, byte> groupWires = Enumerable.Range(0, segmentCount).ToDictionary(x => x, x => (byte)127);


            for (int i = 0; i < digits.Count; ++i)
            {
                var digit = digits.ElementAt(i);
                var patterns = mappings.Where(mapping => mapping.Value.Digits.Contains(digit.Key));
                byte commonSegments = 127;
                int group = Bitwise.CountSetBits(digit.Key) - 1;
                foreach (byte availableDigit in patterns.Select(x => x.Value.Digits).SelectMany(x => x).Distinct())
                    commonSegments &= availableDigit;
                for (int segmentIndex = 0; segmentIndex < segmentCount; ++segmentIndex)
                {
                    if (!Bitwise.IsBitSet(commonSegments, segmentIndex)) continue;
                    segmentMapping.ElementAt(segmentIndex).Value.Group |= 1 << group;
                }
                foreach (var pattern in patterns)
                    groupWires[group] &= pattern.Key;
            }

            byte alreadySet = 0;
            while (Bitwise.CountSetBits(alreadySet) < 7)
            {
                foreach (byte segment in Enumerable.Range(0, segmentCount).Select(x => 1 << x))
                {
                    if (Bitwise.CountSetBits(segmentMapping[segment].Wire) == 1) continue;
                    byte wireMapping = 127;
                    foreach (var group in groupWires.Where(x => ((1 << x.Key) & segmentMapping[segment].Group) != 0))
                        wireMapping &= group.Value;
                    wireMapping &= (byte)~alreadySet;
                    if (Bitwise.CountSetBits(wireMapping) == 1)
                    {
                        alreadySet |= wireMapping;
                        segmentMapping[segment].Wire = wireMapping;
                    }
                }
            }

            return segmentMapping.ToDictionary(x => x.Value.Wire, x => x.Key);
        }
    }
}
