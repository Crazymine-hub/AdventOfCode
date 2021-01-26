using AdventOfCode.Days.Tools.Day20;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day20 : DayBase
    {
        public override string Title => "Jurassic Jigsaw";

        List<CameraImage> images = new List<CameraImage>();
        int dimension = 0;

        public override string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";
            foreach (var image in GetGroupedLines(input))
                images.Add(new CameraImage(GetLines(image)));
            dimension = (int)Math.Floor(Math.Sqrt(images.Count));

            Render();

            return "";
        }

        private void Render()
        {
            Console.Clear();
            int cellCnt = 0;
            foreach (CameraImage img in images)
            {
                int top = Console.CursorTop;
                int left = Console.CursorLeft;
                //Console.WriteLine("Tile " + img.ID + ":");
                //--Console.CursorTop;
                foreach (string line in GetLines(img.ToString()))
                {
                    Console.WriteLine();
                    Console.CursorLeft = left;
                    Console.Write(line);
                }

                left = Console.CursorLeft;
                if (++cellCnt >= dimension)
                {
                    Console.CursorLeft = 0;
                    cellCnt = 0;
                }
                else
                    Console.SetCursorPosition(left, top);
            }
        }
    }
}
