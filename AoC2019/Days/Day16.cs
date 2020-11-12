using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day16 : DayBase
    {
        private int[] pattern;
        private readonly int[] chain = new int[] { 0, 1, 0, -1 };
        private int startPos = 0;
        private bool part2 = false;

        readonly char[] progressIndicator = new char[] { '|', '/', '─', '\\' };
        int progressPos = 0;
        public override string Solve(string input, bool part2)
        {
            //input = "80871224585914546619083218645595";
            this.part2 = part2;
            Console.WriteLine("Loading...");
            LoadInput(input);

            if (part2)
            {
                startPos = int.Parse(input.Substring(0, 7));
                List<int> updatePattern = new List<int>();
                for (int i = 0; i < 10000; i++)
                    updatePattern.AddRange(pattern);
                pattern = updatePattern.ToArray();
            }

            Console.WriteLine("Processing...");

            for (int i = 0; i < 100; i++)
            {
                Console.Write("i = " + i + " ");
                if (part2)
                    TransmitLarge();
                else
                    Transmit();
            }

            Console.WriteLine();
            Console.WriteLine("".PadLeft(10, '-'));
            Console.WriteLine("DONE");
            Console.WriteLine("".PadLeft(10, '-'));
            Console.WriteLine();

            if (part2)
                return WritePattern(8);
            else
                return WritePattern() + "\r\n\r\nVALUE: " + WritePattern(8);
        }

        private void Transmit()
        {
            Console.WriteLine("Pattern: " + (part2 ? "" : WritePattern()));

            int[] newPat = new int[pattern.Length];
            for (int field = startPos; field < pattern.Length; field++)
            {
                int[] currChain = GetPosChain((int)field);
                int value = 0;
                for (int patLocation = field; patLocation < pattern.Length; patLocation++)
                    value += pattern[patLocation] * currChain[patLocation % (currChain.Length - 1) + 1];
                newPat[field] = Math.Abs(value) % 10;
                if (field % 100 == 0)
                {
                    Console.CursorLeft = 0;
                    Console.Write(NextProgressChar() + ((field - startPos) * 100 / (pattern.Length - startPos) + "%").PadRight(5));
                }
            }
            pattern = newPat;
            Console.CursorLeft = 0;
        }

        private void TransmitLarge()
        {
            Console.WriteLine("Pattern: " + (part2 ? "" : WritePattern()));

            int[] newPat = new int[pattern.Length];
            int sum = 0;
            for (int field = pattern.Length - 1; field >= startPos; field--)
            {
                int value = pattern[field] + sum;
                newPat[field] = Math.Abs(value) % 10;
                sum = value;
                if (field % 100 == 0)
                {
                    Console.CursorLeft = 0;
                    Console.Write(NextProgressChar() + ((field - startPos) * 100 / (pattern.Length - startPos) + "%").PadRight(5));
                }
            }
            pattern = newPat;
            Console.CursorLeft = 0;
        }



        private int[] GetPosChain(int position)
        {
            List<int> outChain = new List<int>();
            for (int i = 0; i < chain.Length; i++)
                for (int repeat = 0; repeat <= position; repeat++)
                {
                    if (outChain.Count > pattern.Length)
                        return outChain.ToArray();
                    outChain.Add(chain[i]);
                }
            outChain.Add(chain[0]);
            return outChain.ToArray();
        }

        private void LoadInput(string input)
        {
            pattern = new int[input.Length];
            for (int i = 0; i < input.Length; i++)
                pattern[i] = int.Parse(input[i].ToString());
        }

        private string WritePattern(int count = -1)
        {
            string result = "";
            for (int i = (count == -1 ? 0 : startPos); i < (count == -1 ? pattern.Length : count + startPos); i++)
                result += pattern[i].ToString();
            return result;
        }


        private char NextProgressChar()
        {
            progressPos++;
            if (progressPos >= progressIndicator.Length)
                progressPos = 0;
            return progressIndicator[progressPos];
        }
    }
}
