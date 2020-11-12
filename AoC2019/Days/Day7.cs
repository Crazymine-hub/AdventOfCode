using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Tools.IntComputer;
using AdventOfCode.Tools;

namespace AdventOfCode.Days
{
    public class Day7 : DayBase
    {
        private List<ulong[]> computerSettings;
        private int settingPos = 0;
        private IntComputer[] computers = new IntComputer[5];

        public override string Solve(string input, bool part2)
        {
            GenerateSettings(part2);
            long maxOut = 0;
            while (settingPos < computerSettings.Count)
            {
                for (int i = 0; i <= 4; i++)
                    computers[i] = new IntComputer(true);
                for (int i = 0; i <= 4; i++)
                {
                    computers[i].ReadMemory(input);
                    computers[i].AddInput((int)(computerSettings[settingPos][i]));
                    if (i == 0)
                        computers[i].AddInput(0);
                    else
                    {
                        computers[i - 1].OnOutput += computers[i].AddInput;
                        if(part2 && i == 4)
                        {
                            computers[i].OnOutput += computers[0].AddInput;
                        }
                    }
                    computers[i].Name = "Computer_"+i;
                }
                for (int i = 0; i <= 4; i++)
                    computers[i].RunAsync();

                IntComputer currComputer = computers[4];
                currComputer.ExecutingTask.Wait();
                if (currComputer.Output.Length >= 1 && currComputer.Output.Last() > maxOut)
                    maxOut = currComputer.Output.Last();
                settingPos++;
            }
            return maxOut.ToString();
        }

        private void GenerateSettings(bool loopMode)
        {
            ulong checkSum = 10;
            ulong start = 1234;
            ulong end = 43210;
            if (loopMode)
            {
                start = 56789;
                end = 98765;
                checkSum = 35;
            }
            computerSettings = new List<ulong[]>();
            for (ulong i = start; i <= end; i++)
            {
                ulong[] currNumber = NumberLists.SetLength(NumberLists.MakeArray(i), 5, false);
                if (NumberLists.CrossSum(currNumber) == checkSum && NumberLists.UniqueDigits(currNumber))
                {
                    computerSettings.Add(currNumber.ToArray());
                }
            }
            if (computerSettings.Count != 120)
                throw new Exception("Wrong amount of settings generated");
        }
    }
}
