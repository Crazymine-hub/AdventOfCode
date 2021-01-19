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
        private List<List<List<bool>>> altUniverse = new List<List<List<bool>>>();
        private int xDim = 0;
        private int yDim = 0;
        private int zDim = 0;

        public override string Solve(string input, bool part2)
        {
            if (part2) return "Part 2 is unavailable!";
            Reset(input);
            int fields = 0;
            for (int i = 0; i < 6; i++)
            {
                fields = ConvayStep();
                Console.WriteLine("(" + i + ")Fields: " + fields);
            }
            return "Fields: " + fields;
        }

        private void Reset(string input)
        {
            universe = new List<List<List<bool>>>();
            altUniverse = new List<List<List<bool>>>();
            xDim = 0;
            yDim = 0;
            zDim = 0;
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
                {
                    universe[z][y].Insert(front ? 0 : xDim, false);
                    altUniverse[z][y].Insert(front ? 0 : xDim, false);
                }
            ++xDim;
        }

        private void IncreaseY(bool front)
        {
            for (int z = 0; z < zDim; z++)
            {
                List<bool> row = new List<bool>();
                List<bool> altRow = new List<bool>();
                for (int x = 0; x < xDim; x++)
                {
                    row.Add(false);
                    altRow.Add(false);
                }
                universe[z].Insert(front ? 0 : yDim, row);
                altUniverse[z].Insert(front ? 0 : yDim, altRow);
            }
            ++yDim;
        }

        private void IncreaseZ(bool front)
        {
            List<List<bool>> grid = new List<List<bool>>();
            List<List<bool>> altGrid = new List<List<bool>>();
            for (int y = 0; y < yDim; y++)
            {
                List<bool> row = new List<bool>();
                List<bool> altRow = new List<bool>();
                for (int x = 0; x < xDim; x++)
                {
                    row.Add(false);
                    altRow.Add(false);
                }
                grid.Add(row);
                altGrid.Add(altRow);
            }
            universe.Insert(front ? 0 : zDim, grid);
            altUniverse.Insert(front ? 0 : zDim, altGrid);
            ++zDim;
        }

        private void DecreaseX(bool front)
        {
            for (int z = 0; z < zDim; z++)
                for (int y = 0; y < yDim; y++)
                {
                    universe[z][y].RemoveAt(front ? 0 : xDim - 1);
                    altUniverse[z][y].RemoveAt(front ? 0 : xDim - 1);
                }
            --xDim;
        }

        private void DecreaseY(bool front)
        {
            for (int z = 0; z < zDim; z++)
            {
                universe[z].RemoveAt(front ? 0 : yDim - 1);
                altUniverse[z].RemoveAt(front ? 0 : yDim - 1);
            }
            --yDim;
        }

        private void DecreaseZ(bool front)
        {
            universe.RemoveAt(front ? 0 : zDim - 1);
            altUniverse.RemoveAt(front ? 0 : zDim - 1);
            --zDim;
        }

        private void PrintLayer(int zLayer, bool useAlt)
        {
            Console.WriteLine();
            List<List<bool>> grid;
            try
            {
                grid = useAlt ? altUniverse[zLayer] : universe[zLayer];
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
            foreach (var layer in universe)
            {
                int currMinY = layer.FindIndex(x => x.Contains(true));
                int currMaxY = layer.FindLastIndex(x => x.Contains(true));
                if (currMinY >= 0 && (currMinY < minY || minY == -1)) minY = currMinY;
                if (currMaxY > maxY || maxY == -1) maxY = currMaxY;
                foreach (var column in layer)
                {
                    int currMinX = column.FindIndex(x => x);
                    int currMaxX = column.FindLastIndex(x => x);
                    if (currMinX >= 0 && (currMinX < minX || minX == -1)) minX = currMinX;
                    if (currMaxX > maxX || maxX == -1) maxX = currMaxX;
                }
            }
            if (minZ == 0)
            {
                IncreaseZ(true);
                ++maxZ;
            }
            if (maxZ == zDim - 1)
                IncreaseZ(false);
            if(minZ == 2)
            {
                DecreaseZ(true);
                --maxZ;
            }
            if (maxZ == zDim - 3)
                DecreaseZ(false);

            if (minY == 0)
            {
                IncreaseY(true);
                ++maxY;
            }
            if (maxY == yDim - 1)
                IncreaseY(false);
            if (minY == 2)
            {
                DecreaseY(true);
                --maxY;
            }
            if (maxY == yDim - 3)
                DecreaseY(false);

            if (minX == 0)
            {
                IncreaseX(true);
                ++maxX;
            }
            if (maxX == xDim - 1)
                IncreaseX(false);
            if (minX == 2)
            {
                DecreaseX(true);
                --maxX;
            }
            if (maxX == xDim - 3)
                DecreaseX(false);
        }

        private int ConvayStep()
        {
            AdjustBorder();
            var action = new Func<object, int>(ConvayLayer);
            //List<Task<int>> tasks = new List<Task<int>>();
            int fields = 0;
            for (int i = 0; i < zDim; i++)
            {
                //var task = new Task<int>(action, i);
                //tasks.Add(task);
                //task.Start();
                fields += ConvayLayer(i);
                PrintLayer(i, true);
            }

            Console.WriteLine();
            Console.WriteLine(ConsoleAssist.Center("", xDim, '-'));
            Console.WriteLine();

            //Task.WaitAll(tasks.ToArray());
            //foreach (var task in tasks)
            //    fields += task.Result;

            var bufUniverse = universe;
            universe = altUniverse;
            altUniverse = bufUniverse;
            return fields;
        }

        private int ConvayLayer(object zLayer)
        {
            int fields = 0;
            int z = (int)zLayer;
            for (int y = 0; y < yDim; y++)
                for (int x = 0; x < xDim; x++)
                {
                    int neighbours = CountNeighbours(x, y, z);
                    altUniverse[z][y][x] = false;
                    if ((universe[z][y][x] && neighbours >= 2 && neighbours <= 3) || (!universe[z][y][x] && neighbours == 3))
                    {
                        altUniverse[z][y][x] = true;
                        ++fields;
                    }
                }
            return fields;
        }

        private int CountNeighbours(int xStart, int yStart, int zStart)
        {
            int neighbours = 0;
            for (int z = zStart - 1; z <= zStart + 1; z++)
                for (int y = yStart - 1; y <= yStart + 1; y++)
                    for (int x = xStart - 1; x <= xStart + 1; x++)
                    {
                        if (x == xStart && y == yStart && z == zStart) continue;
                        if (x < 0 || x >= xDim || y < 0 || y >= yDim || z < 0 || z >= zDim) continue;
                        if (universe[z][y][x]) ++neighbours;
                    }
            return neighbours;
        }
    }
}
