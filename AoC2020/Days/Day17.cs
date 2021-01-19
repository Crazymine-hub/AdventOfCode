using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day17 : DayBase
    {
        public override string Title => "Conway Cubes";


        // Z Y X
        private List<List<List<bool>>> universe = new List<List<List<bool>>>();
        private int xDim = 0;
        private int yDim = 0;
        private int zDim = 0;

        public override string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable!";
            Reset(input);
            PrintLayer(1);
            PrintLayer(0);
            AdjustBorder();
            return "";
        }

        private void Reset(string input)
        {
            universe = new List<List<List<bool>>>();
            IncreaseZ(false);
            var layout = GetLines(input);
            for (int y = 0; y < layout.Count; y++)
            {
                IncreaseY(false);
                for (int x = 0; x < layout[y].Length; x++)
                {
                    if (x >= xDim) IncreaseX(false);
                    universe[0][y][x] = layout[y][x] == '#';
                }
            }
        }

        private void IncreaseX(bool front)
        {
            for (int z = 0; z < zDim; z++)
                for (int y = 0; y < yDim; y++)
                    universe[z][y].Insert(front ? 0 : xDim, false);
            xDim++;
        }

        private void IncreaseY(bool front)
        {
            for (int z = 0; z < zDim; z++)
            {
                List<bool> row = new List<bool>();
                for (int x = 0; x < xDim; x++)
                    row.Add(false);
                universe[z].Insert(front ? 0 : yDim, row);
            }
            yDim++;
        }

        private void IncreaseZ(bool front)
        {
            List<List<bool>> grid = new List<List<bool>>();
            for (int y = 0; y < yDim; y++)
            {
                List<bool> row = new List<bool>();
                for (int x = 0; x < xDim; x++)
                    row.Add(false);
                grid.Add(row);
            }
            universe.Insert(front ? 0 : zDim, grid);
            zDim++;
        }

        private void PrintLayer(int zLayer)
        {
            Console.WriteLine();
            List<List<bool>> grid;
            try
            {
                grid = universe[zLayer];
                for (int y = 0; y < yDim; y++)
                {
                    for (int x = 0; x < xDim; x++)
                        Console.Write(grid[y][x] ? '#' : '.');
                    Console.WriteLine();
                }
            }
            catch
            {
                for (int y = 0; y < yDim; y++)
                    Console.WriteLine(ConsoleAssist.Center("<EMPTY>", xDim, '-'));
            }
        }

        private void AdjustBorder()
        {
            int minZ = universe.FindIndex(x => x.FirstOrDefault(y => y.Contains(true)) != default(List<bool>));
            int maxZ = universe.FindLastIndex(x => x.FirstOrDefault(y => y.Contains(true)) != default(List<bool>));
            int minY = -1;
            int maxY = -1;
            int minX = -1;
            int maxX = -1;
            foreach(var layer in universe)
            {
                int currMinY = layer.FindIndex(x => x.Contains(true));
                int currMaxY = layer.FindLastIndex(x => x.Contains(true));
                if (currMinY < minY || minY == -1) minY = currMinY;
                if (currMaxY > maxY || maxY == -1) maxY = currMaxY;
                foreach(var column in layer)
                {
                    int currMinX = column.FindIndex(x => x);
                    int currMaxX = column.FindLastIndex(x => x);
                    if (currMinX < minX || minX == -1) minX = currMinY;
                    if (currMaxX > maxX || maxX == -1) maxX = currMaxY;
                }
            }
            if(minZ == 0)
            {
                IncreaseZ(true);
                ++maxZ;
            }
            if(maxZ == zDim - 1)
                IncreaseZ(false);

            if(minY == 0)
            {
                IncreaseY(true);
                ++maxY;
            }
            if(maxY == yDim - 1)
                IncreaseY(false);

            if(minX == 0)
            {
                IncreaseX(true);
                ++maxX;
            }
            if(maxX == xDim - 1)
                IncreaseX(false);
        }
    }
}
