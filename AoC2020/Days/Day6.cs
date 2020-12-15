using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day6 : DayBase
    {
        public override string Title => "Custom Customs";

        public override string Solve(string input, bool part2)
        {
            long answerCnt = 0;
            foreach(string answerGroup in GetGroupedLines(input))
            {//Get the group answerkey for each group.
                //Initialize the group key for the right operation below.
                int groupKey = part2 ? -1 : 0;
                foreach (string personAnswer in GetLines(answerGroup)) {
                    //Get a single persons answers
                    int answerKey = 0;
                    foreach (Match answer in Regex.Matches(personAnswer.ToLower(), @"\w"))
                        answerKey = Bitwise.SetBit(answerKey, (byte)answer.Value[0] - (byte)'a', true);
                    //Add Persons answers to its group Key
                    if (part2)
                        groupKey &= answerKey;
                    else
                        groupKey |= answerKey;
                }
                //count the answers in this group
                answerCnt += Bitwise.CountSetBits(groupKey);
            }
            return "Sum of all Answers: " + answerCnt;
        }
    }
}
