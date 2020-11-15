using AdventOfCode.Days.Classes.Day15;
using AdventOfCode.Tools.IntComputer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PathHelp = AdventOfCode.Tools.TraceChars;

namespace AdventOfCode.Days
{
    public class Day15 : DayBase
    {
        IntComputer computer = new IntComputer(true);
        private bool part2 = false;
        Point position = new Point();
        int direction = 0; //0 = North 1 = West 2 = South 3 = East

        List<MoveInfo> moves = new List<MoveInfo>();
        /* Bit  Set/Unset
         * 0 = Wall
         * 1 = Intersection (0 & 1 set = Path)
         * 2 = North
         * 3 = South
         * 4 = West
         * 5 = East
         */
        List<List<FieldInfo>> canvas = new List<List<FieldInfo>>();
        Point dimensions = new Point(1, 1);

        bool wallHit = false;
        int directionAdd = 0;
        private bool backtracking = false;
        private MoveInfo backtrackMove;
        private int distance;
        private int maxdist;
        private bool outletFound = false;

        public override string Solve(string input, bool part2)
        {
            this.part2 = part2;
            canvas.Add(new List<FieldInfo>() { new FieldInfo() { FieldType = FieldFlag.Path, IsStart = true } });
            computer.OnOutput += OnMoveFeedback;
            computer.InputRequested += Computer_InputRequested;
            computer.ReadMemory(input);
            computer.Run();
            if (part2)
                return "Maximum distance: " + maxdist;
            else
                return "Moves Used: " + moves.Count;
        }

        private long Computer_InputRequested()
        {
            if (wallHit)
            {
                directionAdd++;
                moves.Last().SetDirectionStatus(direction, true);
            }
            else
            {
                backtrackMove = null;
                directionAdd = 0;
                if (!backtracking)
                {
                    moves.Add(new MoveInfo(direction) { IsIntersection = true });
                    if (distance > maxdist)
                        maxdist = distance;
                    canvas[position.X][position.Y].Distance = distance++;
                }
            }
            if (directionAdd == 3 || (backtrackMove != null && backtrackMove.GetNextFreeDirection() == -1))
            {
                //Console.Beep(261, 100);
                //Console.Beep(261, 100);
                backtracking = true;
                directionAdd = -1;
                if (moves.Last().IsIntersection)
                    moves.Last().IsIntersection = false;
            }
            if (directionAdd >= 4)
                throw new InvalidOperationException("Path Not Found");

            if (backtracking)
            {
                canvas[position.X][position.Y].WasBacktracked = true;
                MoveInfo move = moves.Last();
                move.SetDirectionStatus(MoveInfo.InvertDirection(direction), true);
                bool hasDirectionsLeft = move.GetNextFreeDirection() != -1;
                move.SetDirectionStatus(MoveInfo.InvertDirection(direction), false);
                if (move.IsIntersection && hasDirectionsLeft)
                {
                    backtracking = false;
                    move.SetDirectionStatus(MoveInfo.InvertDirection(direction), true);
                    backtrackMove = move;
                    direction = backtrackMove.GetNextFreeDirection();
                    directionAdd = -1;
                }
                else
                {
                    if (moves.Count == 0)
                    {
                        backtracking = false;
                        moves.Add(new MoveInfo(0));
                        return 1;
                    }
                    moves.RemoveAt(moves.Count - 1);
                    distance--;
                    direction = MoveInfo.InvertDirection(move.Direction);
                }
            }
            else
            {
                if (backtrackMove != null)
                {
                    direction = backtrackMove.GetNextFreeDirection();
                    directionAdd = 0;
                }
                else
                    direction += directionAdd;
            }
            if (direction >= 4)
                direction -= 4;

            switch (direction)
            {
                case 1: return 3;
                case 2: return 2;
                case 3: return 4;
                default: return 1;
            }
        }

        private void OnMoveFeedback(long value)
        {
            wallHit = false;
            switch (value)
            {
                case 0:
                    wallHit = true;
                    break;
                case 2:
                    if (part2)
                    {
                        if (outletFound)
                        {
                            Console.SetCursorPosition(0, dimensions.Y + 2);
                            computer.Reset();
                            Imagine();
                            return;
                        }
                        Move();
                        distance = 0;
                        canvas = new List<List<FieldInfo>>();
                        canvas.Add(new List<FieldInfo>() { new FieldInfo() { FieldType = FieldFlag.Path, IsOxygen = true } });
                        wallHit = false;
                        directionAdd = 0;
                        backtracking = false;
                        moves = new List<MoveInfo>();
                        backtrackMove = null;
                        dimensions = new Point(1, 1);
                        position = new Point();
                        direction = 0;
                        outletFound = true;
                        return;
                    }
                    else
                    {
                        Move();
                        Console.Beep(440, 100);
                        Console.Beep(523, 100);
                        Console.Beep(440, 100);
                        Console.SetCursorPosition(0, dimensions.Y + 2);
                        computer.Reset();
                        return;
                    }
            }
            Move();
        }

        private void Move()
        {
            Console.SetCursorPosition(position.X, position.Y);
            FieldInfo currField = canvas[position.X][position.Y];

            if (currField.FieldType == FieldFlag.Path && !wallHit)
            {
                currField.FieldType = FieldFlag.Intersection;
            }
            else
                currField.FieldType = FieldFlag.Path;

            int offset = direction - 2 < 0 ? -1 : 1;
            if (direction % 2 == 0)
            {//Y-Movement
                if (!wallHit)
                {
                    if (offset == -1)
                        currField.ConnectsNorth = true;
                    else
                        currField.ConnectsSouth = true;
                }
                DrawPixel(position.X, position.Y);
                position.Y += offset;
                if (position.Y < 0 || position.Y >= dimensions.Y)
                    ResizeCanvas(true, offset < 0);

                currField = canvas[position.X][position.Y];
                if (!wallHit)
                {
                    if (offset == 1)
                        currField.ConnectsNorth = true;
                    else
                        currField.ConnectsSouth = true;
                }
            }
            else
            {//X-Movement
                if (!wallHit)
                {
                    if (offset == -1)
                        currField.ConnectsWest = true;
                    else
                        currField.ConnectsEast = true;
                }
                DrawPixel(position.X, position.Y);
                position.X += offset;
                if (position.X < 0 || position.X >= dimensions.X)
                    ResizeCanvas(false, offset < 0);
                currField = canvas[position.X][position.Y];
                if (!wallHit)
                {
                    if (offset == 1)
                        currField.ConnectsWest = true;
                    else
                        currField.ConnectsEast = true;
                }
            }

            Console.SetCursorPosition(position.X, position.Y);

            if (wallHit)
            {
                currField.FieldType = FieldFlag.Wall;
                DrawPixel(position.X, position.Y);
                if (direction % 2 == 0)
                    position.Y -= offset;
                else
                    position.X -= offset;
            }
            DrawBot();
            Thread.Sleep(10);
        }

        private void ResizeCanvas(bool isRow, bool isBefore)
        {
            int pos = 0;
            if (isRow)
            {
                dimensions.Y++;
                if (!isBefore)
                    pos = dimensions.Y - 1;
                else
                    position.Y++;
                foreach (List<FieldInfo> column in canvas)
                    column.Insert(pos, new FieldInfo());
            }
            else
            {
                dimensions.X++;
                if (!isBefore)
                    pos = dimensions.X - 1;
                else
                    position.X++;
                List<FieldInfo> newRow = (new FieldInfo[dimensions.Y]).ToList();
                for (int i = 0; i < newRow.Count; i++)
                    newRow[i] = new FieldInfo();
                canvas.Insert(pos, newRow);
            }
            Redraw();
        }

        private void Redraw()
        {
            Console.Clear();

            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                    DrawPixel(x, y);
                Console.WriteLine();
            }
        }

        private void DrawPixel(int x, int y)
        {
            if (canvas[x][y].IsStart)
            {
                Console.Write('#');
                return;
            }
            if (canvas[x][y].IsOxygen)
            {
                Console.Write('O');
                return;
            }
            switch (canvas[x][y].FieldType)
            {
                case FieldFlag.Wall:
                    Console.Write('█');
                    break;
                case FieldFlag.Unknown:
                    Console.Write('░');
                    break;
                default:
                    Console.Write(PathHelp.paths[(int)canvas[x][y].GetPathFlag()]);
                    break;
            }
        }

        private void DrawBot()
        {
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write("8");
        }

        private void Imagine()
        {
            Bitmap map = new Bitmap(dimensions.X, dimensions.Y);

            for (int x = 0; x < canvas.Count; x++)
                for (int y = 0; y < canvas[x].Count; y++)
                {
                    FieldInfo field = canvas[x][y];
                    int value = Convert.ToInt32(field.Distance / (double)maxdist * 0xFF);
                    //value |= -16777216; // 0xFF000000
                    Color col = Color.Black;
                    if (field.FieldType != FieldFlag.Unknown && field.FieldType != FieldFlag.Wall)
                        //col = Color.FromArgb(field.Distance | -16777216);
                        col = Color.FromArgb(value, 0, 255);
                    map.SetPixel(x, y, col);
                }
            //map.Save("Day15_map.bmp");
        }
    }
}
