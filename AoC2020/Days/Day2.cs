using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day2 : DayBase
    {
        public override string Title => "Password Philosophy";


        public override string Solve(string input, bool part2)
        {
            int validPassCnt = 0;
            foreach (string line in GetLines(input))
            {// each line is a password to check
                //Get the information as specified
                Match passInfo = Regex.Match(line, @"(?<MinChars>\d+)-(?<MaxChars>\d+) (?<PassChar>\w): (?<Pass>.*)");
                if (!passInfo.Success) throw new Exception("Something's wrong. I can feel it.");
                //put in variables for easier access
                int min = int.Parse(passInfo.Groups["MinChars"].Value);
                int max = int.Parse(passInfo.Groups["MaxChars"].Value);
                char passChar = passInfo.Groups["PassChar"].Value[0];
                string pass = passInfo.Groups["Pass"].Value;
                int charCnt = 0;
                if (part2)
                {//The Char must be either at the "min" or "max" position
                    if (pass[min - 1] == passChar ^ pass[max - 1] == passChar) validPassCnt++;
                }
                else
                {//"min" and "max" specify how often the char can appear in the password
                    foreach (char currChar in pass)
                        if (currChar == passChar) charCnt++;
                    if (min <= charCnt && charCnt <= max) validPassCnt++;
                }
            }
            return "Valid Passwords: " + validPassCnt;
        }
    }
}
