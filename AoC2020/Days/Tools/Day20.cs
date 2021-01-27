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
            Console.WriteLine();

            List<(int, int, char)> borders = new List<(int, int, char)>();

            foreach (CameraImage image in images)
            {
                for (int i = 0; i < 2; ++i)
                {
                    image.RotateLeft();
                    image.RotateLeft();
                    borders.Add((image.ID, image.TopBorderNr, 'U'));
                    borders.Add((image.ID, image.BottomBorderNr, 'D'));
                    borders.Add((image.ID, image.LeftBorderNr, 'L'));
                    borders.Add((image.ID, image.RightBorderNr, 'R'));
                }
            }

            Lookup<int, (int, int, char)> tileGroups = (Lookup<int, (int, int, char)>)borders.ToLookup(k => k.Item2, v => v);
            Lookup<int, (int, int, char)> tileBorders = (Lookup<int, (int, int, char)>)borders.ToLookup(k => k.Item1, v => v);

            var borderConnections = tileGroups.Where(x => x.Count() == 1);
            var innerConnections = tileGroups.Where(x => x.Count() == 2);
            var undetermined = tileGroups.Where(x => x.Count() > 2);

            List<(int, char, int, char, int, int)> uniqueInnerConnections = new List<(int, char, int, char, int, int)>();
            foreach (var connection in innerConnections)
            {
                (int id1, int border1, char side1) = connection.ElementAt(0);
                (int id2, int border2, char side2) = connection.ElementAt(1);

                var detected = uniqueInnerConnections.Where(x =>
                    (id1 == x.Item1 || id1 == x.Item3) &&
                    (id2 == x.Item1 || id2 == x.Item3)
                ).ToList();

                if (detected.Count() == 0)
                    uniqueInnerConnections.Add((id1, side1, id2, side2, connection.Key, 0));
                else
                {
                    (int dId1, char dSide1, int dId2, char dSide2, int dBorder1, int dBorder2) = detected[0];
                    uniqueInnerConnections[uniqueInnerConnections.IndexOf((dId1, dSide1, dId2, dSide2, dBorder1, dBorder2))] =
                        (dId1, dSide1, dId2, dSide2, dBorder1, connection.Key);
                }
            }

            List<(int, char, int, int, int, int)> uniqueBorderConnections = new List<(int, char, int, int, int, int)>();
            foreach (var connection in borderConnections)
            {
                (int id1, int border1, char side1) = connection.ElementAt(0);

                var detected = uniqueBorderConnections.Where(x =>
                    id1 == x.Item1 || id1 == x.Item3
                ).ToList();

                if (detected.Count() == 0)
                    uniqueBorderConnections.Add((id1, side1, connection.Key, -1, -1, -1));
                else
                {
                    (int dId1, char dSide1, int dBorder1, int dBorder2, int dBorder3, int dBorder4) = detected[0];
                    int index = uniqueBorderConnections.IndexOf((dId1, dSide1, dBorder1, dBorder2, dBorder3, dBorder4));
                    if (dBorder2 == -1) dBorder2 = connection.Key;
                    else if (dBorder3 == -1) dBorder3 = connection.Key;
                    else if (dBorder4 == -1) dBorder4 = connection.Key;
                    uniqueBorderConnections[index] = (dId1, dSide1, dBorder1, dBorder2, dBorder3, dBorder4);
                }
            }

            long product = 1;
            foreach (var connection in uniqueBorderConnections.Where(x => x.Item6 != -1))
                product *= connection.Item1;


            return "Product of corner tile-IDs: " + product;
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
