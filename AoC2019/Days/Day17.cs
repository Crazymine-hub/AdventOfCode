using AdventOfCode.Tools.IntComputer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day17 : DayBase
    {
        private IntComputer camera = new IntComputer(true);
        private bool startedFeed = false;
        private bool lastCrLf = false;
        private int bottom = 0;
        private long result;

        public Day17()
        {
            UsesAdditionalContent = true;
        }

        public override string Solve(string input, bool part2)
        {
            //if (part2) return "Part 2 is unavailable";
            camera.ReadMemory(input);
                camera.OnOutput += Camera_OnOutput;

            camera.InputRequested += Camera_InputRequested;
            if (part2)
            {
                camera.Memory[0] = 2;
                if (AdditionalContent != null)
                    foreach (char inp in AdditionalContent.Replace("\r", ""))
                        camera.AddInput(inp);
            }
            camera.Run();
            Console.SetCursorPosition(0, bottom + 1);
            if (part2)
                return "DUST: " + result;
            return "";
        }

        private long Camera_InputRequested()
        {
            startedFeed = false;
            if (camera.HasInputsQueued) return 0;
            var input = Console.ReadKey();
            if (input.Key == ConsoleKey.Enter)
                return 10;
            return input.KeyChar;
        }

        private void Camera_OnOutput(long value)
        {
            if(value > char.MaxValue)
            {
                result = value;
                return;
            }
            string display = Convert.ToChar(value).ToString();
            if (display == "\n")
            {
                if (!lastCrLf)
                {
                    lastCrLf = true;
                    return;
                }
                display = "\r\n";
            }
            if (display == "." && !startedFeed)
            {
                startedFeed = true;
                Console.Clear();
            }

            if (lastCrLf)
            {
                display = "\r\n" + display;
                lastCrLf = false;
                if (display == "\r\n\r\n")
                {
                    if (Console.CursorTop > bottom)
                        bottom = Console.CursorTop;
                    Console.SetCursorPosition(0, 0);
                    Thread.Sleep(50);
                    return;
                }
            }
            Console.Write(display);
        }
    }
}
