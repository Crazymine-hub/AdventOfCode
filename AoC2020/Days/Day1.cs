using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day1 : DayBase
    {
        public override string Title => "Report Repair";

        int[] positions;
        private List<int> numbers;

        public override string Solve(string input, bool part2)
        {
            //Convert Input into List<int> (valid ints assumed)
            numbers = GetLines(input).Select(x => int.Parse(x)).ToList();

            string result = null;
            bool numberMoved = true;
            positions = new int[part2 ? 3 : 2];
            //Initialize positions
            for (int pos = 0; pos < positions.Length; pos++)
                positions[pos] = pos;
            while (string.IsNullOrWhiteSpace(result) && numberMoved)
            {//repeat until found or done
                result = GetProduct();
                numberMoved = MoveNextNumber(positions.Length - 1);
            }
            if (!numberMoved)
                return "No match found!";
            else
                return result;
        }

        private string GetProduct()
        {
            int sum = 0;
            int product = 1;
            string formula = "";
            foreach(int pos in positions)
            {//build sum and product of numbers at given positions
                if (pos >= numbers.Count) return null;
                sum += numbers[pos];
                product *= numbers[pos];
                formula += numbers[pos] + " x ";
            }
            //Make the outout string fancy and a valid math expression
            formula += "1 = ";
            if (sum == 2020)
                return formula + product;
            else return null;
        }

        private bool MoveNextNumber(int position)
        {
            //tried to change position below range. We're done.
            if (position == -1) return false;
            //increase the number at the given position.
            if (++positions[position] >= numbers.Count)
            {//increased above range. increase previous digit and reset given digit (RECURSION)
                if (!MoveNextNumber(position - 1)) return false; //Previous digit could not be changed. abort.
                positions[position] = positions[position - 1] + 1;
            }
            return true;
        }
    }
}
