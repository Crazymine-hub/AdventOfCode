using AdventOfCode.Tools.IntComputer;
using System;
using System.Collections.Generic;
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
        List<List<long>> canvas = new List<List<long>>(); //byte 0 = color byte 1 = wasColoredOnce
        Point dimensions = new Point(1, 1);

        bool wallHit = false;

        public string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable";
            canvas.Add(new List<long>() { 0 });
            computer.OnOutput += OnMoveFeedback;
            computer.InputRequested += Computer_InputRequested;
            computer.ReadMemory(input);
            computer.Run();
            return "";
        }

        private long Computer_InputRequested()
        {
            //TODO: Pathfinding here
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
                    Console.Beep();
                    computer.Reset();
                    break;
            }
            Move();
        }

        private void Move()
        {
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write(" ");
            int offset = direction - 2 < 0 ? -1 : 1;
            if (direction % 2 == 0)
            {//Y-Movement
                position.Y += offset;
                if (position.Y < 0 || position.Y >= dimensions.Y)
                    ResizeCanvas(true, offset < 0);
            }
            else
            {//X-Movement
                position.X += offset;
                if (position.X < 0 || position.X >= dimensions.X)
                    ResizeCanvas(false, offset < 0);
            }
            canvas[position.X][position.Y] = wallHit ? 1 : 0;
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write((canvas[position.X][position.Y] & 1) == 1 ? "#" : " ");
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
                {
                    Console.Write((canvas[x][y] & 1) == 1 ? "#" : " ");
                }
                Console.WriteLine();
            }
        }

        private void DrawBot()
        {
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write("8");
        }
    }
}
