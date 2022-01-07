using AdventOfCode.Days.Tools.Day20;
using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.IO;
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
            foreach (var image in GetGroupedLines(input))
                images.Add(new CameraImage(GetLines(image)));
            dimension = (int)Math.Floor(Math.Sqrt(images.Count));

            Render(false);
            Console.WriteLine();

            List<(int, int, char)> borders = new List<(int, int, char)>();

            foreach (CameraImage image in images)
            {
                for (int i = 0; i < 2; ++i)
                {   //get all borders for every image. since Right and bottom basically read reversed, we don't need to flip and only do a 180.
                    //since the value for example for a border at the left would be the same for the same border at the top we can skip this rotation step
                    image.RotateRight();
                    image.RotateRight();
                    borders.Add((image.ID, image.TopBorderNr, 'U'));
                    borders.Add((image.ID, image.BottomBorderNr, 'D'));
                    borders.Add((image.ID, image.LeftBorderNr, 'L'));
                    borders.Add((image.ID, image.RightBorderNr, 'R'));
                }
            }

            //Group the images according to how they share a border
            Lookup<int, (int, int, char)> tileGroups = (Lookup<int, (int, int, char)>)borders.ToLookup(k => k.Item2, v => v);
            //Separate these groups for outer and inner connections
            //(outer connection being the image border that connects to no other image and is therefore part of the outer border of the final image)
            var borderConnections = tileGroups.Where(x => x.Count() == 1);
            var innerConnections = tileGroups.Where(x => x.Count() == 2);
            //all borders must be pairs. there can't be a border value, that matches more than 2 images. at least i would panic a bit in that case.
            var undetermined = tileGroups.Where(x => x.Count() > 2);
            if (undetermined.Count() > 0) throw new InvalidOperationException("Some borders couldn't be assigned !!");


            //reorganize the inner connections, so that we don't have duplicates (bidirectional) anymore
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

            //reorganize the outer borders, so that all possible border values are assigned to th same image
            Dictionary<int, List<(int, char)>> uniqueBorderConnections = new Dictionary<int, List<(int, char)>>();
            foreach (var connection in borderConnections)
            {
                (int id1, int border1, char side) = connection.ElementAt(0);

                if (!uniqueBorderConnections.ContainsKey(id1))
                    uniqueBorderConnections.Add(id1, new List<(int, char)>() { (connection.Key, side) });
                else
                {
                    List<(int, char)> dBorders = uniqueBorderConnections[id1];
                    if (dBorders.Count >= 1 && dBorders.Count <= 4) dBorders.Add((connection.Key, side));
                }
            }



            if (part2)
            {
                ArrangeImage(uniqueBorderConnections, uniqueInnerConnections);
                //Create an image from the rendered Map
                CameraImage map = new CameraImage(GetLines("Tile 2020:\r\n" + Render(true)));
                Console.WriteLine();
                //Create an image from the monster string
                CameraImage monster = new CameraImage(GetLines("Tile 2020:\r\n..................#.\r\n#....##....##....###\r\n.#..#..#..#..#..#..."));
                int monsterWidth = monster.TopBorder.Count;
                int monsterHeight = monster.Image.Count;

                //Set up trying to find the monsters
                int currMonsterCnt = 0;
                int rotationCount = 0;
                bool flipped = false;
                Console.SetCursorPosition(0, 0);
                while (currMonsterCnt == 0)
                {
                    currMonsterCnt = 0;
                    //Going over every line in the map except the last three, because our monster is 3 rows tall
                    for (int y = 0; y <= map.Image.Count - monsterHeight; ++y)
                    {   //Moving along this line leaving enough space for the width of our monster.
                        for (int x = 0; x <= map.Image[0].Count - monsterWidth; ++x)
                        {
                            //go over the lines of the monster
                            bool isMonster = true;
                            for (int monsterY = 0; monsterY < monsterHeight; ++monsterY)
                            {
                                //Get the area of the current line of the map and the monster as number (bitmask)
                                long mappedValue = Bitwise.GetValue<long>(map.Image[y + monsterY].GetRange(x, monsterWidth));
                                long monsterRow = Bitwise.GetValue<long>(monster.Image[monsterY]);
                                //if the map contains the monster, the and join should output a perfect replica of the monster
                                if ((mappedValue & monsterRow) != monsterRow)
                                {//otherwise we don't have a complete monster
                                    isMonster = false;
                                    break;
                                }
                            }
                            if (isMonster)
                            {   //When we got a full monster increase the counter and censor the area, the monster is located in
                                //I was to lazy, to work out the pixels, that are part of the monster so we just draw  in the area the monster takes up (monsterWidth x monsterHeight)
                                for (int monsterY = 0; monsterY < monsterHeight; ++monsterY)
                                {
                                    Console.SetCursorPosition(x, y + monsterY);                      
                                    //MArk the area with a letter to differentiate monsters. especially those that overlap
                                    Console.WriteLine("".PadLeft(monsterWidth, (char)(0x41 + currMonsterCnt)));
                                }
                                ++currMonsterCnt;
                            }
                        }
                    }


                    if (currMonsterCnt == 0)
                    {//if we didn't find a single monster, rotate the image by 90 degrees
                        map.RotateRight();
                        Render(true);
                        if (++rotationCount >= 4)
                        {//we did a full 360. Perhaps we need to flip
                            if (flipped)
                            {
                                if (rotationCount >= 5)//we flipped one more time to be sure. this image couldn't be properly aligned. that shouldn't happen :(
                                    throw new InvalidOperationException("LÖÖP");
                                else continue;
                            }
                            map.Flip();
                            rotationCount = 0;
                            flipped = true;
                        }
                    }
                }

                Console.CursorTop= map.Image.Count + 1;
                int roughness = map.ToString().Where(x => x == '#').Count();
                roughness -= monster.ToString().Where(x => x == '#').Count() * currMonsterCnt;
                return "Monsters Found: " + currMonsterCnt + "\r\nRoughness: " + roughness;
            }
            else
            {//part 1 only needs the product of the ids of the corner tiles
                long product = 1;
                foreach (var connection in uniqueBorderConnections.Where(x => x.Value.Count == 4))
                    product *= connection.Key;
                return "Product of corner tile-IDs: " + product;
            }
        }

        private void ArrangeImage(
            //id (border connection)
            Dictionary<int, List<(int, char)>> uniqueBorderConnections,
            //id id border direction border direction
            List<(int, int, int, string, int, string)> uniqueInnerConnections)
        {
            var newImg = new List<CameraImage>();

            //fill all the imgaes;
            while (newImg.Count < images.Count)
            {
                #region Variable initialization
                CameraImage upperImage = null;
                CameraImage leftImage = null;

                CameraImage selImg;
                List<(int, char)> borders;
                Func<CameraImage, List<(int, char)>, List<(int, char)>, int, bool, bool, bool, bool> checkImageOrientation;
                //callback for checking a single border for right orientation
                bool checkImageBorder(int border, int targetBorder, char orientation, string orientations, bool flipped, int rotations)
                {
                    bool borderNr = border == targetBorder;
                    bool borderOrientation;
                    if (rotations % 2 == 0)
                        borderOrientation = orientation == (flipped ? orientations[0] : orientations[1]);
                    else
                        borderOrientation = orientation == (flipped ? orientations[2] : orientations[3]);
                    return borderNr && borderOrientation;
                }
                #endregion

                //calcutlate our current position and get the image above and left to the current image
                int upperBorderIndex = newImg.Count - dimension;
                int leftBorderIndex = newImg.Count - 1;
                if (upperBorderIndex >= 0) upperImage = newImg[upperBorderIndex];
                if ((leftBorderIndex + 1) % dimension != 0 && leftBorderIndex >= 0) leftImage = newImg[leftBorderIndex];

                int upperBorderValue = upperImage?.BottomBorderNr ?? invalidBorderValue;
                int leftBorderValue = leftImage?.RightBorderNr ?? invalidBorderValue;

                if (upperBorderValue == invalidBorderValue && leftBorderValue == invalidBorderValue)
                {//the very first image has nothing above and left to it.
                 //Get the first image that belongs to a corner
                    int selId = uniqueBorderConnections.First(img => img.Value.Count == 4).Key;
                    selImg = images.Single(x => x.ID == selId);
                    borders = uniqueBorderConnections[selId];

                    //define how the image is checked to be correctly aligned
                    //it is when: all borders could be validated, when at least one couldn't, change it again.
                    checkImageOrientation = (img, borderValues, connectingValues, rotations, isFlipped, _0, _1) =>
                    {
                        return
                            !borderValues.Any(x => checkImageBorder(x.Item1, img.LeftBorderNr, x.Item2, "RLUD", isFlipped, rotations)) ||
                            !borderValues.Any(x => checkImageBorder(x.Item1, img.TopBorderNr, x.Item2, "DULR", isFlipped, rotations)) ||
                            !connectingValues.Any(x => checkImageBorder(x.Item1, img.RightBorderNr, x.Item2, "LRDU", isFlipped, rotations)) ||
                            !connectingValues.Any(x => checkImageBorder(x.Item1, img.BottomBorderNr, x.Item2, "UDRL", isFlipped, rotations));
                    };
                }
                else if (upperBorderValue != invalidBorderValue && leftBorderValue == invalidBorderValue)
                {//the image has something above it, but nothng to its left. meaning the very left images besides the very first.
                 //get the image, that matches somehow to a border of its left neighbour
                    (int id1, int id2, int border1, string side1, int border2, string side2) = uniqueInnerConnections.Single(
                        x => (x.Item1 == upperImage.ID || x.Item2 == upperImage.ID) && (x.Item3 == upperBorderValue || x.Item5 == upperBorderValue));
                    int selId = id1 == upperImage.ID ? id2 : id1;
                    selImg = images.Single(x => x.ID == selId);

                    borders = uniqueBorderConnections[selId];

                    //define how the image is checked to be correctly aligned
                    checkImageOrientation = (img, borderValues, connectingValues, rotations, isFlipped, isBottom, isRight) =>
                    {
                        return !borderValues.Any(x => checkImageBorder(x.Item1, img.LeftBorderNr, x.Item2, "RLUD", isFlipped, rotations)) ||
                         selImg.TopBorderNr != upperBorderValue ||
                         !connectingValues.Any(x => checkImageBorder(x.Item1, img.RightBorderNr, x.Item2, "LRDU", isFlipped, rotations)) ||
                         //if we reached the bottom, we need to check, that the images bottom border is part of the outer border
                         //otherwise we check, if it corecctly connects
                         !isBottom && !connectingValues.Any(x => checkImageBorder(x.Item1, img.BottomBorderNr, x.Item2, "UDRL", isFlipped, rotations)) ||
                         isBottom && !borderValues.Any(x => checkImageBorder(x.Item1, img.BottomBorderNr, x.Item2, "UDRL", isFlipped, rotations));
                    };
                }
                else if (upperBorderValue == invalidBorderValue && leftBorderValue != invalidBorderValue)
                {//The image has nothing above it, but it has an image to its left. This is true for the very first row except the very first image
                 //find the image, that should connect to the image above
                    (int id1, int id2, int border1, string side1, int border2, string side2) = uniqueInnerConnections.Single(
                        x => (x.Item1 == leftImage.ID || x.Item2 == leftImage.ID) && (x.Item3 == leftBorderValue || x.Item5 == leftBorderValue));
                    int selId = id1 == leftImage.ID ? id2 : id1;
                    selImg = images.Single(x => x.ID == selId);
                    borders = uniqueBorderConnections[selId];

                    //define how the image is checked to be correctly aligned
                    checkImageOrientation = (img, borderValues, connectingValues, rotations, isFlipped, isBottom, isRight) =>
                    {
                        return leftBorderValue != selImg.LeftBorderNr ||
                        !borderValues.Any(x => checkImageBorder(x.Item1, img.TopBorderNr, x.Item2, "DULR", isFlipped, rotations)) ||
                        //if we are at the very right, we need to check if the images right border is part of the outer border
                        //otherwise, we check if it correctly connects.
                        !isRight && !connectingValues.Any(x => checkImageBorder(x.Item1, img.RightBorderNr, x.Item2, "LRDU", isFlipped, rotations)) ||
                        isRight && !borderValues.Any(x => checkImageBorder(x.Item1, img.RightBorderNr, x.Item2, "LRDU", isFlipped, rotations)) ||
                        !connectingValues.Any(x => checkImageBorder(x.Item1, img.BottomBorderNr, x.Item2, "UDRL", isFlipped, rotations));
                    };
                }
                else
                {//The image has something above it and to its left. Meaning all other images
                 //Find the image, that connects to the image to the left. we don't check above, since it would be redundant.
                 //(i had this handy to be copied/pasted from above. don't judge me)
                    (int id1, int id2, int border1, string side1, int border2, string side2) = uniqueInnerConnections.Single(
                        x => (x.Item1 == leftImage.ID || x.Item2 == leftImage.ID) && (x.Item3 == leftBorderValue || x.Item5 == leftBorderValue));
                    int selId = id1 == leftImage.ID ? id2 : id1;
                    selImg = images.Single(x => x.ID == selId);

                    if (uniqueBorderConnections.ContainsKey(selId))
                        borders = uniqueBorderConnections[selId];
                    else
                        borders = null;

                    checkImageOrientation = (img, borderValues, connectingValues, rotations, isFlipped, isBottom, isRight) =>
                    {
                        return leftBorderValue != selImg.LeftBorderNr || upperBorderValue != selImg.TopBorderNr ||
                        //this time we need to check for the right and lower outer border.
                        !isRight && !connectingValues.Any(x => checkImageBorder(x.Item1, img.RightBorderNr, x.Item2, "LRDU", isFlipped, rotations)) ||
                        isRight && !borderValues.Any(x => checkImageBorder(x.Item1, img.RightBorderNr, x.Item2, "LRDU", isFlipped, rotations)) ||
                        !isBottom && !connectingValues.Any(x => checkImageBorder(x.Item1, img.BottomBorderNr, x.Item2, "UDRL", isFlipped, rotations)) ||
                        isBottom && !borderValues.Any(x => checkImageBorder(x.Item1, img.BottomBorderNr, x.Item2, "UDRL", isFlipped, rotations));
                    };
                }


                int rotationCount = 0;
                bool flipped = false;
                bool isBottom = (newImg.Count) / dimension == dimension - 1;
                bool isRight = (newImg.Count + 1) % dimension == 0;
                //get the borders, that connect to other images for this image
                List<(int, char)> connecting = new List<(int, char)>();
                var currUniqueInnerConnections = uniqueInnerConnections.Where(x => x.Item1 == selImg.ID || x.Item2 == selImg.ID);
                foreach (var connection in currUniqueInnerConnections)
                {
                    connecting.Add((connection.Item3, connection.Item4[connection.Item1 == selImg.ID ? 0 : 1]));
                    connecting.Add((connection.Item5, connection.Item6[connection.Item1 == selImg.ID ? 0 : 1]));
                }

                //Rotate the image until it is correctly aligned as per the function that checks the alignment
                while (checkImageOrientation(selImg, borders, connecting, rotationCount, flipped, isBottom, isRight))
                {
                    selImg.RotateRight();
                    if (++rotationCount >= 4)
                    {//we did a full 360. Perhaps we need to flip
                        if (flipped)
                        {
                            if (rotationCount >= 5)//we flipped one more time to be sure. this image couldn't be properly aligned. that shouldn't happen :(
                                throw new InvalidOperationException("LÖÖP");
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

        private void RenderGridded()
        {

            Console.Clear();
            var stdOut = Console.OpenStandardOutput();

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

                left = Console.CursorLeft + 1;
                if (++cellCnt >= dimension)
                {
                    Console.CursorLeft = 0;
                    cellCnt = 0;
                    Console.WriteLine();
                }
                else
                    Console.SetCursorPosition(left, top);
            }
            Console.WriteLine();
        }

        private string Render(bool stripped)
        {
            string outp = "";
            for (int row = 0; row < dimension; ++row)
            {
                bool hasLines = true;
                int lineNr = 0;
                while (hasLines)
                {
                    hasLines = false;
                    for (int img = 0; img < dimension; ++img)
                    {
                        var lines = GetLines(images[row * dimension + img].ToString(stripped));
                        if (lineNr < lines.Count)
                        {
                            hasLines = true;
                            outp += lines[lineNr];
                        }
                    }
                    if (hasLines) outp += "\r\n";
                    ++lineNr;
                }
            }
            Console.Clear();
            Console.WriteLine(outp);
            return outp;
        }
    }
}
