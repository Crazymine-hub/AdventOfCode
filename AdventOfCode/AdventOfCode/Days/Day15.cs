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

namespace AdventOfCode.Days
{
    class Day15 : IDay
    {
        IntComputer computer = new IntComputer(true);
        Point position = new Point();
        int direction = 0; //0 = North 1 = West 2 = South 3 = East
                           //0    1    2    3    4    5    6    7    8    9    10   11   12   13   14   15
        readonly char[] paths = new char[] { ' ', '╵', '╷', '│', '╴', '┘', '┐', '┤', '╶', '└', '┌', '├', '─', '┴', '┬', '┼' };

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
        private bool isIntersection;
        private bool backtracking = false;
        private MoveInfo backtrackMove;

        public string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";
            canvas.Add(new List<FieldInfo>() { new FieldInfo() { FieldType = FieldFlag.Path } });
            computer.OnOutput += OnMoveFeedback;
            computer.InputRequested += Computer_InputRequested;
            computer.ReadMemory(input);
            computer.Run();
            return "Moves Used: " + moves.Count;
        }

        private long Computer_InputRequested()
        {
            if (wallHit)
            {
                directionAdd++;
                moves.Last().BlockDirection(direction);
            }
            else
            {
                backtrackMove = null;
                directionAdd = 0;
                if (!backtracking)
                {
                    moves.Add(new MoveInfo(direction));
                    if (isIntersection)
                    {
                        moves[moves.Count - 2].IsIntersection = true;
                    }
                }
            }
            if (directionAdd == 3 || (backtrackMove != null && backtrackMove.GetNextFreeDirection() == -1))
            {
                Console.Beep(261, 100);
                Console.Beep(261, 100);
                backtracking = true;
                directionAdd = -1;
                if (moves.Last() == backtrackMove)
                    backtrackMove.IsIntersection = false;
            }
            if (directionAdd >= 4)
                throw new InvalidOperationException("Path not found");

            if (backtracking)
            {
                MoveInfo move = moves.Last();
                if (move.IsIntersection)
                {
                    backtracking = false;
                    move.BlockDirection(MoveInfo.InvertDirection(direction));
                    direction = move.Direction;
                    directionAdd = -1;
                    backtrackMove = move;
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
                    Move();
                    Console.Beep(440, 100);
                    Console.Beep(523, 100);
                    Console.Beep(440, 100);
                    Console.SetCursorPosition(0, dimensions.Y + 2);
                    computer.Reset();
                    return;
            }
            Move();
        }

        private void Move()
        {
            Console.SetCursorPosition(position.X, position.Y);
            FieldInfo currField = canvas[position.X][position.Y];
            isIntersection = false;

            if (currField.FieldType == FieldFlag.Path && !wallHit)
            {
                currField.FieldType = FieldFlag.Intersection;
                isIntersection = true;
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
            switch (canvas[x][y].FieldType)
            {
                case FieldFlag.Wall:
                    Console.Write('█');
                    break;
                case FieldFlag.Unknown:
                    Console.Write('░');
                    break;
                default:
                    Console.Write(paths[(int)canvas[x][y].GetPathFlag()]);
                    break;
            }
        }

        private void DrawBot()
        {
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write("8");
        }
    }
}
