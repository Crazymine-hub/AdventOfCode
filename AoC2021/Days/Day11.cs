using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day11 : DayBase, IDisposable
    {
        public override string Title => "Dumbo Octopus";
        int[][] raveGrid;
        Bitmap rave;

        const int scale = 10;
        private const int squidFlashEnergy = 10;
        private bool wasDisposed;

        public override string Solve(string input, bool part2)
        {
            raveGrid = GetLines(input).Select(x => x.Select(y => int.Parse(y.ToString())).ToArray()).ToArray();
            rave = new Bitmap(raveGrid[0].Length * scale, raveGrid.Length * scale);
            VisualFormHandler.Instance.Show((Image)rave.Clone());

            int flashCount = 0;
            int i = 0;
            for (i = 0; part2 || (i < 100); ++i)
            {
                Console.Clear();
                Console.WriteLine(i);
                int stepFlashes = DoStep();
                flashCount += stepFlashes;
                if (part2 && stepFlashes == RaveGridForEach().Count()) break;
                Task.Delay(10).Wait();
            }

            Console.WriteLine();

            if (part2)
                return "All flashing in step " + (i + 1);
            return "Total flashes: " + flashCount;
        }

        private int DoStep()
        {
            foreach (var squid in RaveGridForEach())
                raveGrid[squid.Y][squid.X] += 1;

            while (RaveGridForEach().Any(x => raveGrid[x.Y][x.X] >= squidFlashEnergy))
            {
                foreach (var squid in RaveGridForEach())
                {
                    if (raveGrid[squid.Y][squid.X] < squidFlashEnergy) continue;
                    raveGrid[squid.Y][squid.X] = -1;
                    foreach (var adjacant in GetAdjacent(squid.X, squid.Y))
                    {
                        if (adjacant.X < 0 ||
                            adjacant.Y < 0 ||
                            adjacant.Y >= raveGrid.Length ||
                            adjacant.X >= raveGrid[0].Length) continue;
                        if (raveGrid[adjacant.Y][adjacant.X] < 0) continue;

                        ++raveGrid[adjacant.Y][adjacant.X];
                    }
                }
            }
            int prevY = -1;
            int flashCount = 0;
            foreach (var squid in RaveGridForEach())
            {
                if (squid.Y != prevY)
                    Console.WriteLine();
                prevY = squid.Y;

                if (raveGrid[squid.Y][squid.X] == -1)
                {
                    raveGrid[squid.Y][squid.X] = 0;
                    Console.Write('█');
                    ++flashCount;
                }
                else
                    Console.Write(raveGrid[squid.Y][squid.X]);

                double value = raveGrid[squid.Y][squid.X] / 9.0;
                int brightness = Convert.ToInt32(value * 0xFF);
                rave.FillRect(new Rectangle(squid.X * scale, squid.Y * scale, scale, scale), Color.FromArgb(brightness, brightness, brightness));
            }
            VisualFormHandler.Instance.Update(rave);
            return flashCount;
        }

        private IEnumerable<Point> RaveGridForEach()
        {
            for (int y = 0; y < raveGrid.Length; ++y)
                for (int x = 0; x < raveGrid.Length; ++x)
                    yield return new Point(x, y);
        }

        private IEnumerable<Point> GetAdjacent(int x, int y)
        {
            yield return new Point(x - 1, y - 1);   //top left
            yield return new Point(x, y - 1);     //top mid
            yield return new Point(x + 1, y - 1);   //top right

            yield return new Point(x - 1, y);     //left
            yield return new Point(x + 1, y);     //right

            yield return new Point(x - 1, y + 1);   //bottom left
            yield return new Point(x, y + 1);     //bottom mid
            yield return new Point(x + 1, y + 1);   //bottom right
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!wasDisposed)
            {
                if (disposing)
                {
                    rave.Dispose();
                }
                wasDisposed = true;
            }
        }

        // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // ~Day11()
        // {
        //     // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
