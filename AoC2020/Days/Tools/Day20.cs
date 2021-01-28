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
        const int invalidBorderValue = -1;

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
                    image.RotateRight();
                    image.RotateRight();
                    borders.Add((image.ID, image.TopBorderNr, 'U'));
                    borders.Add((image.ID, image.BottomBorderNr, 'D'));
                    borders.Add((image.ID, image.LeftBorderNr, 'L'));
                    borders.Add((image.ID, image.RightBorderNr, 'R'));
                }
            }

            Lookup<int, (int, int, char)> tileGroups = (Lookup<int, (int, int, char)>)borders.ToLookup(k => k.Item2, v => v);

            var borderConnections = tileGroups.Where(x => x.Count() == 1);
            var innerConnections = tileGroups.Where(x => x.Count() == 2);
            var undetermined = tileGroups.Where(x => x.Count() > 2);

            //List<(int, int, int, int)> uniqueInnerConnections = new List<(int, int, int, int)>();
            List<(int, int, int, string, int, string)> uniqueInnerConnections = new List<(int, int, int, string, int, string)>();
            foreach (var connection in innerConnections)
            {
                (int id1, int border1, char side1) = connection.ElementAt(0);
                (int id2, int border2, char side2) = connection.ElementAt(1);

                var detected = uniqueInnerConnections.Where(x =>
                    (id1 == x.Item1 || id1 == x.Item2) &&
                    (id2 == x.Item1 || id2 == x.Item2)
                ).ToList();

                string sideName = side1.ToString() + side2.ToString();

                if (detected.Count() == 0)
                    uniqueInnerConnections.Add((id1, id2, connection.Key, sideName, 0, ""));
                else
                {
                    (int dId1, int dId2, int dBorder1, string dSide1, int dBorder2, string dSide2) = detected[0];
                    uniqueInnerConnections[uniqueInnerConnections.IndexOf(detected[0])] =
                        (dId1, dId2, dBorder1, dSide1, connection.Key, sideName);
                }
            }

            List<(int, List<(int, char)>)> uniqueBorderConnections = new List<(int, List<(int, char)>)>();
            foreach (var connection in borderConnections)
            {
                (int id1, int border1, char side) = connection.ElementAt(0);

                var detected = uniqueBorderConnections.Where(x => id1 == x.Item1).ToList();

                if (detected.Count() == 0)
                    uniqueBorderConnections.Add((id1, new List<(int, char)>() { (connection.Key, side) }));
                else
                {
                    (int dId1, List<(int, char)> dBorders) = detected[0];
                    int index = uniqueBorderConnections.IndexOf(detected[0]);
                    if (dBorders.Count >= 1 && dBorders.Count <= 4) dBorders.Add((connection.Key, side));
                    uniqueBorderConnections[index] = (dId1, dBorders);
                }
            }

            long product = 1;
            foreach (var connection in uniqueBorderConnections.Where(x => x.Item2.Count == 4))
                product *= connection.Item1;

            ArrangeImage(uniqueBorderConnections, uniqueInnerConnections);

            Render();

            return "Product of corner tile-IDs: " + product;
        }

        private void ArrangeImage(
            //id (border connection)
            List<(int, List<(int, char)>)> uniqueBorderConnections,
            //id id border direction border direction
            List<(int, int, int, string, int, string)> uniqueInnerConnections)
        {
            var newImg = new List<CameraImage>();

            while (newImg.Count < images.Count)
            {
                int upperBorderIndex = newImg.Count - dimension;
                int leftBorderIndex = newImg.Count - 1;
                CameraImage upperImage = null;
                CameraImage leftImage = null;
                if (upperBorderIndex >= 0) upperImage = newImg[upperBorderIndex];
                if ((leftBorderIndex + 1) % dimension != 0 && leftBorderIndex >= 0) leftImage = newImg[leftBorderIndex];

                int upperBorderValue = upperImage?.BottomBorderNr ?? invalidBorderValue;
                int leftBorderValue = leftImage?.RightBorderNr ?? invalidBorderValue;

                CameraImage selImg;
                List<(int, char)> connecting;
                List<(int, char)> borders;
                Func<CameraImage, List<(int, char)>, List<(int, char)>, int, bool, bool, bool, bool> checkImageOrientation;

                if (upperBorderValue == invalidBorderValue && leftBorderValue == invalidBorderValue)
                {
                    (int selId, List<(int, char)> borderValues) = uniqueBorderConnections.First(img => img.Item2.Count == 4);
                    selImg = images.Single(x => x.ID == selId);
                    connecting = new List<(int, char)>();
                    borders = borderValues;

                    foreach (var connection in uniqueInnerConnections.Where(x => x.Item1 == selId || x.Item2 == selId))
                    {
                        connecting.Add((connection.Item3, connection.Item4[connection.Item1 == selId ? 0 : 1]));
                        connecting.Add((connection.Item5, connection.Item6[connection.Item1 == selId ? 0 : 1]));
                    }

                    checkImageOrientation = (img, borderValues, connectingValues, rotations, isFlipped, _0, _1) =>
                    {
                        return (!borderValues.Any(x => x.Item1 == img.LeftBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'R' : 'L') : x.Item2 == (isFlipped ? 'U' : 'D'))) ||
                            !borderValues.Any(x => x.Item1 == img.TopBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'D' : 'U') : x.Item2 == (isFlipped ? 'L' : 'R'))) ||
                            !connectingValues.Any(x => x.Item1 == img.RightBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'L' : 'R') : x.Item2 == (isFlipped ? 'D' : 'U'))) ||
                            !connectingValues.Any(x => x.Item1 == img.BottomBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'U' : 'D') : x.Item2 == (isFlipped ? 'R' : 'L'))));
                    };
                }
                else if (upperBorderValue != invalidBorderValue && leftBorderValue == invalidBorderValue)
                {
                    (int id1, int id2, int border1, string side1, int border2, string side2) = uniqueInnerConnections.Single(
                        x => (x.Item1 == upperImage.ID || x.Item2 == upperImage.ID) && (x.Item3 == upperBorderValue || x.Item5 == upperBorderValue));
                    int selId = id1 == upperImage.ID ? id2 : id1;

                    selImg = images.Single(x => x.ID == selId);
                    borders = uniqueBorderConnections.Single(x => x.Item1 == selId).Item2;
                    connecting = new List<(int, char)>();
                    foreach (var connection in uniqueInnerConnections.Where(x => x.Item1 == selId || x.Item2 == selId))
                    {
                        connecting.Add((connection.Item3, connection.Item4[connection.Item1 == selId ? 0 : 1]));
                        connecting.Add((connection.Item5, connection.Item6[connection.Item1 == selId ? 0 : 1]));
                    }

                    checkImageOrientation = (img, borderValues, connectingValues, rotations, isFlipped, isBottom, isRight) =>
                    {
                        return (!borderValues.Any(x => x.Item1 == selImg.LeftBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'R' : 'L') : x.Item2 == (isFlipped ? 'U' : 'D'))) ||
                         selImg.TopBorderNr != upperBorderValue ||
                         !connecting.Any(x => x.Item1 == selImg.RightBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'L' : 'R') : x.Item2 == (isFlipped ? 'D' : 'U'))) ||
                         !isBottom && !connecting.Any(x => x.Item1 == selImg.BottomBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'U' : 'D') : x.Item2 == (isFlipped ? 'R' : 'L'))) ||
                         isBottom && !borderValues.Any(x => x.Item1 == selImg.BottomBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'U' : 'D') : x.Item2 == (isFlipped ? 'R' : 'L'))));
                    };
                }
                else if (upperBorderValue == invalidBorderValue && leftBorderValue != invalidBorderValue)
                {
                    (int id1, int id2, int border1, string side1, int border2, string side2) = uniqueInnerConnections.Single(
                        x => (x.Item1 == leftImage.ID || x.Item2 == leftImage.ID) && (x.Item3 == leftBorderValue || x.Item5 == leftBorderValue));
                    int selId = id1 == leftImage.ID ? id2 : id1;
                    selImg = images.Single(x => x.ID == selId);
                    borders = uniqueBorderConnections.Single(x => x.Item1 == selId).Item2;

                    connecting = new List<(int, char)>();
                    foreach (var connection in uniqueInnerConnections.Where(x => x.Item1 == selId || x.Item2 == selId))
                    {
                        connecting.Add((connection.Item3, connection.Item4[connection.Item1 == selId ? 0 : 1]));
                        connecting.Add((connection.Item5, connection.Item6[connection.Item1 == selId ? 0 : 1]));
                    }

                    checkImageOrientation = (img, borderValues, connectingValues, rotations, isFlipped, isBottom, isRight) =>
                    {
                        return (leftBorderValue != selImg.LeftBorderNr ||
                        !borderValues.Any(x => x.Item1 == selImg.TopBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'D' : 'U') : x.Item2 == (isFlipped ? 'L' : 'R'))) ||
                        !isRight && !connecting.Any(x => x.Item1 == selImg.RightBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'L' : 'R') : x.Item2 == (isFlipped ? 'D' : 'U'))) ||
                        isRight && !borderValues.Any(x => x.Item1 == selImg.RightBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'L' : 'R') : x.Item2 == (isFlipped ? 'D' : 'U'))) ||
                        !connecting.Any(x => x.Item1 == selImg.BottomBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'U' : 'D') : x.Item2 == (isFlipped ? 'R' : 'L'))));
                    };
                }
                else
                {
                    (int id1, int id2, int border1, string side1, int border2, string side2) = uniqueInnerConnections.Single(
                        x => (x.Item1 == leftImage.ID || x.Item2 == leftImage.ID) && (x.Item3 == leftBorderValue || x.Item5 == leftBorderValue));
                    int selId = id1 == leftImage.ID ? id2 : id1;
                    selImg = images.Single(x => x.ID == selId);

                    borders = uniqueBorderConnections.SingleOrDefault(x => x.Item1 == selId).Item2;
                    connecting = new List<(int, char)>();
                    foreach (var connection in uniqueInnerConnections.Where(x => x.Item1 == selId || x.Item2 == selId))
                    {
                        connecting.Add((connection.Item3, connection.Item4[connection.Item1 == selId ? 0 : 1]));
                        connecting.Add((connection.Item5, connection.Item6[connection.Item1 == selId ? 0 : 1]));
                    }

                    checkImageOrientation = (img, borderValues, connectingValues, rotations, isFlipped, isBottom, isRight) =>
                    {
                        return (leftBorderValue != selImg.LeftBorderNr || upperBorderValue != selImg.TopBorderNr ||
                        !isRight && !connecting.Any(x => x.Item1 == selImg.RightBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'L' : 'R') : x.Item2 == (isFlipped ? 'D' : 'U'))) ||
                        isRight && !borderValues.Any(x => x.Item1 == selImg.RightBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'L' : 'R') : x.Item2 == (isFlipped ? 'D' : 'U'))) ||
                        !isBottom && !connecting.Any(x => x.Item1 == selImg.BottomBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'U' : 'D') : x.Item2 == (isFlipped ? 'R' : 'L'))) ||
                        isBottom && !borderValues.Any(x => x.Item1 == selImg.BottomBorderNr && ((!isFlipped || rotations % 2 == 0) ? x.Item2 == (isFlipped ? 'U' : 'D') : x.Item2 == (isFlipped ? 'R' : 'L'))));
                    };
                }


                int rotationCount = 0;
                bool flipped = false;
                bool isBottom = (newImg.Count) / dimension == dimension - 1;
                bool isRight = (newImg.Count + 1) % dimension == 0;
                while (checkImageOrientation(selImg, borders, connecting, rotationCount, flipped, isBottom, isRight))
                {
                    selImg.RotateRight();
                    if (++rotationCount >= 4)
                    {
                        if (flipped)
                        {
                            if (rotationCount >= 5)
                                throw new InvalidOperationException("LOOP");
                            else continue;
                        }
                        selImg.Flip();
                        rotationCount = 0;
                        flipped = true;
                    }
                }
                newImg.Add(selImg);
            }

            images = newImg;
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
