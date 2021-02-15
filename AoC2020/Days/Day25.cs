using AdventOfCode.Days.Tools.Day25;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day25 : DayBase
    {
        public override string Title => "Combo Breaker";

        RfidClient Card;
        RfidClient Door;
        public override string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailoable.";
            List<string> keys = GetLines(input);
            Card = new RfidClient(long.Parse(keys[0]));
            Door = new RfidClient(long.Parse(keys[1]));

            long cardEnc = Card.GetEncriptionKey(Door.PublicKey);
            long doorEnc = Door.GetEncriptionKey(Card.PublicKey);

            if (cardEnc == doorEnc) return "The encription key is " + cardEnc;
            else return "No encription key found!";
        }
    }
}
