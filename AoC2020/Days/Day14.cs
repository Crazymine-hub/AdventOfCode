using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day14 : DayBase
    {
        public override string Title => "Docking Data";

        /*
         * The bitmask can contain the following values:
         * X = this bit doesn't change
         * 1 = force this bit to be 1
         * 0 = force this bit to be 0
         * 0 can be forced by creating a full (all 1) bitmask and setting the forced bits to 0. It will be applied by & (Bitwise AND)
         * 1 can be forced by creating an empty (all 0) bitmask and setting the forced bits to 1. It will be applied by | (Btwise OR).
         */
        private long force0 = 0;
        private long force1 = 0;
        public override string Solve(string input, bool part2)
        {
            if (part2) return "part2 is unavailable";
            List<long> memory = new List<long>();
            foreach(string[] instruction in GetLines(input).Select(x => x.Split(new string[] { " = " }, StringSplitOptions.RemoveEmptyEntries)))
            {
                if (instruction[0] == "mask") RefreshMasks(instruction[1]);
                if (instruction[0].StartsWith("mem"))
                {//Get the masked value and write it to the right memory address
                    long masked = ApplyMask(instruction[1]);
                    int address = int.Parse(instruction[0].Split('[', ']')[1]);
                    //increase memory if it is to small
                    if (address >= memory.Count) memory.AddRange(new long[address - memory.Count + 1]);
                    memory[address] = masked;
                }
            }

            long sum = 0;
            foreach (long number in memory) sum += number;
            return "Memory Sum is: " + sum;
        }

        private void RefreshMasks(string mask)
        {
            force0 = Bitwise.GetBitMask(36);
            force1 = 0;
            for(int i = 0; i < mask.Length; i++)
            {//set the bits as given in the mask
                if(mask[i] == '1')
                    force1 = Bitwise.SetBit(force1, mask.Length - 1 - i, true); 
                if(mask[i] == '0')
                    force0 = Bitwise.SetBit(force0, mask.Length - 1 - i, false); 
            }
        }

        private long ApplyMask(string number)
        {
            long baseNumber = long.Parse(number);
            //aply the bitmask to the number as described
            baseNumber &= force0;
            baseNumber |= force1;
            return baseNumber;
        }
    }
}
