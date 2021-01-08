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
         * floating bits contain all values (schrödingers bit?)
         */
        private long force0 = 0;
        private long force1 = 0;
        private List<int> floating;
        public override string Solve(string input, bool part2)
        {
            Dictionary<long, long> memory = new Dictionary<long, long>();
            foreach (string[] instruction in GetLines(input).Select(x => x.Split(new string[] { " = " }, StringSplitOptions.RemoveEmptyEntries)))
            {
                if (instruction[0] == "mask") RefreshMasks(instruction[1]);
                if (instruction[0].StartsWith("mem"))
                {//Get the masked value and write it to the right memory address
                    long value = long.Parse(instruction[1]);
                    List<long> addresses = new List<long>() { long.Parse(instruction[0].Split('[', ']')[1]) };
                    if (part2)
                        addresses = ApplyAddressMask(addresses[0]);
                    else
                        value = ApplyValueMask(value);
                    foreach (long address in addresses)
                    {
                        //increase memory if it is to small
                        if (!memory.ContainsKey(address)) memory.Add(address, 0);
                        memory[address] = value;
                    }
                }
            }

            long sum = 0;
            foreach (var number in memory) sum += number.Value;
            return "Memory Sum is: " + sum;
        }

        private void RefreshMasks(string mask)
        {
            //our number is only 36 bits long. Get a 36 bit long fully set mask
            force0 = Bitwise.GetBitMask(36);
            force1 = 0;
            floating = new List<int>();
            for (int i = 0; i < mask.Length; i++)
            {//set the bits as given in the mask
                if (mask[i] == '1')
                    force1 = Bitwise.SetBit(force1, mask.Length - 1 - i, true);
                if (mask[i] == '0')
                    force0 = Bitwise.SetBit(force0, mask.Length - 1 - i, false);
                if (mask[i] == 'X')
                    floating.Add(mask.Length - 1 - i);
            }
            floating.Reverse();
        }

        private long ApplyValueMask(long value)
        {
            //aply the bitmask to the number as described
            value &= force0;
            value |= force1;
            return value;
        }

        private List<long> ApplyAddressMask(long address)
        {
            // apply a mask, to the floating array where each bit represents an entry and whether it is active
            // then check all bits and set the address bit at the index held in floating to whether the bit in the mask that belongs to that index is set.
            List<long> addresses = new List<long>();
            address |= force1;
            //maskValues counts upwards to get all combinations of floating numbers (Mask XX becomes 00 to 11)
            long floatCountMask = Bitwise.GetBitMask(floating.Count);
            for (int maskValues = 0; maskValues <= floatCountMask; maskValues++)
            {
                long newAddress = address;
                //check each bit in the current floating mask and set the bit in the adress that is referenced in the position this bit belongs to
                for (int i = 0; i < floating.Count; i++)
                    newAddress = Bitwise.SetBit(newAddress, floating[i], Bitwise.IsBitSet(maskValues, i));
                addresses.Add(newAddress);
            }
            return addresses;
        }
    }
}
