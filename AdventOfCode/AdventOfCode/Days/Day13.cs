using AdventOfCode.Tools.IntComputer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day13 : IDay
    {
        private readonly string movesFile = Program.path + @"13_moves.txt";

        private  List<long> inputSequence = new List<long>();
        private IntComputer computer = new IntComputer(true);
        private int[] values = new int[3];
        private int pos = 0;
        private readonly char[] tiles = new char[] { '*', '█', '▒', '=', '■' };
        private long drawCount = 0;
        private long score = 0;
        private bool isPart2 = false;
        private int width = 0;
        private int inputPos = 0;
        private int knownGood = 0;

        public string Solve(string input, bool part2)
        {
            isPart2 = part2;
            if (isPart2)
            {
                string[] moves = new string[0];
                if (File.Exists(movesFile))
                    moves = File.ReadAllText(movesFile).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < moves.Length; i++)
                {
                    if (i == 0)
                        knownGood = int.Parse(moves[i]);
                    else
                        inputSequence.Add(int.Parse(moves[i]));
                }
                input = input.Remove(0, 1).Insert(0, "2");
                computer.InputRequested += ControllerRead;
            }

            computer.ReadMemory(input);
            computer.OnOutput += GameRenderer;
            computer.Run();
            Console.CursorTop++;
            if (isPart2)
            {
                Console.CursorTop += 20;
                Console.CursorLeft = 0;
                string save = (inputSequence.Count - 1).ToString() + ';';
                foreach (long move in inputSequence)
                    save += move.ToString() + ';';

                File.WriteAllText(movesFile, save);
                return "GAME OVER\r\nFinal Score: " + score.ToString();
            }
            else
                return "Tiles drawn: " + drawCount.ToString();
        }

        private long ControllerRead()
        {
            Console.SetCursorPosition(width + 1, 0);
            Console.Write(score);
            if (inputPos < inputSequence.Count && inputPos >= 0)
                Console.Write("R");
            else
                Console.Write(" ");

            //if (Console.KeyAvailable)
            {
                ConsoleKey key = ConsoleKey.Enter;
                if (inputPos >= knownGood || inputPos < 0)
                {
                    Thread.Sleep(150);
                    key = Console.ReadKey().Key;
                }
                else
                    Thread.Sleep(10);
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        if(inputPos >= 0)
                        {
                            inputSequence.RemoveRange(inputPos, inputSequence.Count - inputPos);
                            inputPos = -1;
                        }
                        Console.SetCursorPosition(width + 1, 1);
                        Console.Write("<");
                        inputSequence.Add(-1);
                        return -1;
                    case ConsoleKey.RightArrow:
                        if (inputPos >= 0)
                        {
                            inputSequence.RemoveRange(inputPos, inputSequence.Count - inputPos);
                            inputPos = -1;
                        }
                        Console.SetCursorPosition(width + 1, 1);
                        Console.Write(">");
                        inputSequence.Add(1);
                        return 1;
                    default:
                        if (inputPos < inputSequence.Count && inputPos >= 0 && key == ConsoleKey.Enter)
                            return inputSequence[inputPos++];
                        if (inputPos >= 0)
                        {
                            inputSequence.RemoveRange(inputPos, inputSequence.Count - inputPos);
                            inputPos = -1;
                        }
                        Console.SetCursorPosition(width + 1, 1);
                        Console.Write("O");
                        inputSequence.Add(0);
                        return 0;
                }
            }
            /*else
            {
                Console.SetCursorPosition(width + 1, 1);
                Console.Write("O");
                return 0;
            }*/
        }

        private void GameRenderer(long value)
        {
            values[pos] = (int)value;
            pos++;
            if (pos >= 3)
            {
                pos = 0;
                if (values[0] == -1 && values[1] == 0 && isPart2)
                {
                    score = values[2];
                }
                else
                {
                    Console.SetCursorPosition(values[0], values[1]);
                    if (values[2] == 0 && ((values[0] % 2 == 0 && values[1] % 2 == 0)||(values[0] % 2 != 0 && values[1] % 2 != 0)))
                        Console.Write(' ');
                    else
                        Console.Write(tiles[values[2]]);
                    if (values[2] == 2 && !isPart2)
                        drawCount++;
                    if (values[2] == 1 && values[0] > width)
                        width = values[0];
                }
            }
        }
    }
}
