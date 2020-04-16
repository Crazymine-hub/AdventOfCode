using AdventOfCode.Tools.IntComputer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day11 : IDay
    {
        IntComputer computer = new IntComputer(true);
        Point position = new Point();
        int direction = 0; //0 = North 1 = West 2 = South 3 = East
        List<List<long>> canvas = new List<List<long>>(); //byte 0 = color byte 1 = wasColoredOnce
        Point dimensions = new Point(1, 1);
        bool rotateOutput = false;

        readonly string directionArrows = "<^>v";

        public string Solve(string input, bool part2)
        {
            canvas.Add(new List<long>() { 0 });

            computer.OnOutput += OnMoveInstruction;
            computer.ReadMemory(input);
            computer.AddInput(part2 ? 1 : 0);
            computer.Run();

            int drawn = Redraw();
            Console.SetCursorPosition(0, dimensions.Y + 2);
            return string.Format("Dimensions: {0}/{1}\r\nTiles drawn at least once: {2}\r\nPart 2 Active: {3}", dimensions.X, dimensions.Y, drawn, part2.ToString());
        }

        private void OnMoveInstruction(long value)
        {
            if (rotateOutput)
            {
                Rotate(value == 0);
                Move();
                computer.AddInput(canvas[position.X][position.Y] & 1);
            }
            else
                canvas[position.X][position.Y] = 2 + (value & 1);
            rotateOutput = !rotateOutput;
        }

        private void Move()
        {

            Console.SetCursorPosition(position.X, position.Y);
            Console.Write((canvas[position.X][position.Y] & 1) == 1 ? "#" : " ");
            int offset = direction - 2 < 0 ? -1 : 1;
            if(direction % 2 == 0)
            {//Y-Movement
                position.Y += offset;
                if(position.Y < 0 || position.Y >=  dimensions.Y)
                    ResizeCanvas(true, offset < 0);
            }
            else
            {//X-Movement
                position.X += offset;
                if (position.X < 0 || position.X >= dimensions.X)
                    ResizeCanvas(false, offset < 0);
            }
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write(directionArrows[direction]);
            Thread.Sleep(10);
        }

        private void Rotate(bool left)
        {
            if (left)
                direction++;
            else
                direction--;
            if (direction > 3) direction = 0;
            if (direction < 0) direction = 3;
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

        private int Redraw()
        {
            Console.Clear();

            int drawnOnce = 0;

            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                {
                    Console.Write((canvas[x][y] & 1) == 1 ? "#" : " ");
                    if ((canvas[x][y] & 2) == 2)
                        drawnOnce++;
                }
                Console.WriteLine();
            }
            DrawBot();
            return drawnOnce;
        }

        private void DrawBot()
        {
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write(directionArrows[direction]);
        }
    }
}
