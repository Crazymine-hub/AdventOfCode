using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day2 : DayBase
    {
        public override string Title => "Rock Paper Scissors";


        private readonly List<char> myShapes = new List<char> { 'X', 'Y', 'Z' };
        private readonly List<char> oppShapes = new List<char> { 'A', 'B', 'C' };

        public override string Solve(string input, bool part2)
        {
            int score = 0;
            foreach (var line in GetLines(input))
            {
                Console.Write(line);
                Console.Write(' ');
                var opp = line[0];
                var me = line[2];
                if (part2)
                    me = ChooseSymbol(opp, me);
                score += myShapes.IndexOf(me) + 1;
                switch(myShapes.IndexOf(me) - oppShapes.IndexOf(opp))
                {
                    case -2:
                        Console.Write("WIN");
                        score += 6;
                        break;
                    case -1:
                        Console.Write("LOOSE");
                        break;
                    case 0:
                        Console.Write("DRAW");
                        score += 3;
                        break;
                    case 1:
                        Console.Write("WIN");
                        score += 6;
                        break;
                    case 2:
                        Console.Write("LOOSE");
                        break;
                }
                Console.Write(' ');
                Console.WriteLine(score);
            }

            return $"Your Score is {score}";
        }

        private char ChooseSymbol(char opp, char outcome)
        {
            int oppIndex = oppShapes.IndexOf(opp);
            switch (outcome)
            {
                case'X'://loose
                    --oppIndex;
                    break;
                case'Y'://draw
                    break;
                case'Z'://win
                    ++oppIndex;
                    break;
            }
            if(oppIndex < 0) oppIndex = myShapes.Count - 1;
            if(oppIndex >= myShapes.Count) oppIndex = 0;
            return myShapes[oppIndex];
        }
    }
}
