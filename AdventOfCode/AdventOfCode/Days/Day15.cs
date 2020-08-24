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
        List<int> moves = new List<int>();
        /* Bit  Set/Unset
         * 0 = Wall
         * 1 = Intersection (0 & 1 set = Path)
         * 2 = North
         * 3 = South
         * 4 = West
         * 5 = East
         */
        List<List<long>> canvas = new List<List<long>>();
        Point dimensions = new Point(1, 1);

        bool wallHit = false;
        int directionAdd = 0;

        public string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";
            canvas.Add(new List<long>() { 64 });
            computer.OnOutput += OnMoveFeedback;
            computer.InputRequested += Computer_InputRequested;
            computer.ReadMemory(input);
            computer.Run();
            return "";
        }

        private long Computer_InputRequested()
        {
            if (wallHit)
                directionAdd++;
            else
            {
                directionAdd = 0;
                moves.Add(direction);
            }
            if (directionAdd >= 4)
                throw new InvalidOperationException("Path not found");

            if ((canvas[position.X][position.Y] & 3) == 2 && (canvas[position.X][position.Y] & ~511) > 256 * 5 && directionAdd == 0)
            {
                Console.Beep(261, 250);
                directionAdd = 1;
            }

            direction += directionAdd;
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
                    canvas[position.X][position.Y] = canvas[position.X][position.Y] | 128;
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
            if ((canvas[position.X][position.Y] & 3) == 0)
                canvas[position.X][position.Y] = canvas[position.X][position.Y] | (long)(wallHit ? 2 : 3);
            if ((canvas[position.X][position.Y] & 3) == 2 && !wallHit)
            {
                canvas[position.X][position.Y] = canvas[position.X][position.Y] | (canvas[position.X][position.Y] + 256);
            }

            int offset = direction - 2 < 0 ? -1 : 1;
            if (direction % 2 == 0)
            {//Y-Movement
                if (!wallHit)
                    canvas[position.X][position.Y] = canvas[position.X][position.Y] | (long)(offset == -1 ? 4 : 8);
                DrawPixel(position.X, position.Y);
                position.Y += offset;
                if (position.Y < 0 || position.Y >= dimensions.Y)
                    ResizeCanvas(true, offset < 0);
                if (!wallHit)
                    canvas[position.X][position.Y] = canvas[position.X][position.Y] | (long)(offset == 1 ? 4 : 8);
            }
            else
            {//X-Movement
                if (!wallHit)
                    canvas[position.X][position.Y] = canvas[position.X][position.Y] | (long)(offset == -1 ? 16 : 32);
                DrawPixel(position.X, position.Y);
                position.X += offset;
                if (position.X < 0 || position.X >= dimensions.X)
                    ResizeCanvas(false, offset < 0);
                if (!wallHit)
                    canvas[position.X][position.Y] = canvas[position.X][position.Y] | (long)(offset == 1 ? 16 : 32);
            }

            Console.SetCursorPosition(position.X, position.Y);

            if (wallHit)
            {
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
                foreach (List<long> column in canvas)
                    column.Insert(pos, 0);
            }
            else
            {
                dimensions.X++;
                if (!isBefore)
                    pos = dimensions.X - 1;
                else
                    position.X++;
                List<long> newRow = (new long[dimensions.Y]).ToList();
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
            switch (canvas[x][y] & 3)
            {
                case 1:
                    Console.Write('█');
                    break;
                case 0:
                    Console.Write('░');
                    break;
                default:
                    Console.Write(paths[(canvas[x][y] & 0x3C) >> 2]);
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
