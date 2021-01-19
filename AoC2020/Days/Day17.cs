using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    //this could maybe have been realized recursively. imagine: we could just easily add a fifth dimension...
    //though terminal space is limited and 3D is hard enough to keep track of. Don't get me started on 4D.
    class Day17 : DayBase
    {
        public override string Title => "Conway Cubes";

        // W Z Y X
        //The new multiverse state gets written to another multiverse to prevent interference with the state analysis
        private List<List<List<List<bool>>>> multiverse = new List<List<List<List<bool>>>>();
        private List<List<List<List<bool>>>> altMultiverse = new List<List<List<List<bool>>>>();
        private int xDim = 0;
        private int yDim = 0;
        private int zDim = 0;
        private int wDim = 0;
        private bool moreDimensions = false;

        public override string Solve(string input, bool part2)
        {
            moreDimensions = part2;
            Reset(input);
            int fields = 0;
            for (int i = 0; i < 6; i++)
            {// Game of Life
                fields = ConwayStep();
                Console.WriteLine("(" + i + ")Fields: " + fields);
            }
            return "Fields: " + fields;
        }

        private void Reset(string input)
        {   //set all values as they should
            multiverse = new List<List<List<List<bool>>>>();
            altMultiverse = new List<List<List<List<bool>>>>();
            xDim = 0;
            yDim = 0;
            zDim = 0;
            wDim = 0;
            //prepare the multiverse array to contain it's first element (2D Plane in a single universe)
            IncreaseW(false);
            IncreaseZ(false);
            var layout = GetLines(input);
            for (int y = 0; y < layout.Count; y++)
            {
                IncreaseY(false);
                for (int x = 0; x < layout[y].Length; x++)
                {//Fill as stated in the input
                    if (x >= xDim) IncreaseX(false);
                    multiverse[0][0][y][x] = layout[y][x] == '#';
                }
            }
        }

        private void IncreaseX(bool front)
        {//in all universes add one x space
            for (int w = 0; w < wDim; w++)
                for (int z = 0; z < zDim; z++)
                    for (int y = 0; y < yDim; y++)
                    {
                        multiverse[w][z][y].Insert(front ? 0 : xDim, false);
                        altMultiverse[w][z][y].Insert(front ? 0 : xDim, false);
                    }
            ++xDim;
        }

        private void IncreaseY(bool front)
        {//in all universes, append one y space
            for (int w = 0; w < wDim; w++)
                for (int z = 0; z < zDim; z++)
                {   //fill the y space to contain enough x spaces
                    List<bool> row = new List<bool>();
                    List<bool> altRow = new List<bool>();
                    for (int x = 0; x < xDim; x++)
                    {
                        row.Add(false);
                        altRow.Add(false);
                    }
                    multiverse[w][z].Insert(front ? 0 : yDim, row);
                    altMultiverse[w][z].Insert(front ? 0 : yDim, altRow);
                }
            ++yDim;
        }

        private void IncreaseZ(bool front)
        {//in all universes, append one z space
            for (int w = 0; w < wDim; w++)
            {//fill the z space, to contain enough y spaces
                List<List<bool>> grid = new List<List<bool>>();
                List<List<bool>> altGrid = new List<List<bool>>();
                for (int y = 0; y < yDim; y++)
                {//fill the y space to contain enough x spaces
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
                multiverse[w].Insert(front ? 0 : zDim, grid);
                altMultiverse[w].Insert(front ? 0 : zDim, altGrid);
            }
            ++zDim;
        }

        private void IncreaseW(bool front)
        {//Add a new universe
            List<List<List<bool>>> universe = new List<List<List<bool>>>();
            List<List<List<bool>>> altUniverse = new List<List<List<bool>>>();
            for (int w = 0; w < wDim; w++)
            {//fill the universe to contain enough z spaces
                List<List<bool>> grid = new List<List<bool>>();
                List<List<bool>> altGrid = new List<List<bool>>();
                for (int y = 0; y < yDim; y++)
                {//fill the z space to contain enough y spaces
                    List<bool> row = new List<bool>();
                    List<bool> altRow = new List<bool>();
                    for (int x = 0; x < xDim; x++)
                    {//fill the y space to contain enough x spaces
                        row.Add(false);
                        altRow.Add(false);
                    }
                    grid.Add(row);
                    altGrid.Add(altRow);
                }
                universe.Add(grid);
                altUniverse.Add(altGrid);
            }
            multiverse.Insert(front ? 0 : wDim, universe);
            altMultiverse.Insert(front ? 0 : wDim, altUniverse);
            ++wDim;
        }

        private void DecreaseX(bool front)
        {//remove one x space from all y spaces from all z spaces from all universes
            for (int w = 0; w < wDim; w++)
                for (int z = 0; z < zDim; z++)
                    for (int y = 0; y < yDim; y++)
                    {
                        multiverse[w][z][y].RemoveAt(front ? 0 : xDim - 1);
                        altMultiverse[w][z][y].RemoveAt(front ? 0 : xDim - 1);
                    }
            --xDim;
        }

        private void DecreaseY(bool front)
        {//remove one y space from all z spaces in all universes
            for (int w = 0; w < wDim; w++)
                for (int z = 0; z < zDim; z++)
                {
                    multiverse[w][z].RemoveAt(front ? 0 : yDim - 1);
                    altMultiverse[w][z].RemoveAt(front ? 0 : yDim - 1);
                }
            --yDim;
        }

        private void DecreaseZ(bool front)
        {//remove one z space from each universe
            for (int w = 0; w < wDim; w++)
            {
                multiverse[w].RemoveAt(front ? 0 : zDim - 1);
                altMultiverse[w].RemoveAt(front ? 0 : zDim - 1);
            }
            --zDim;
        }

        private void DecreaseW(bool front)
        {//remove one universe
            multiverse.RemoveAt(front ? 0 : wDim - 1);
            altMultiverse.RemoveAt(front ? 0 : wDim - 1);
            --wDim;
        }

        private void PrintUniverse(int wLayer, bool useAlt)
        {
            Console.WriteLine();
            try
            {
                //get the given universe and iterate over it's space to print it's state
                var universe = useAlt ? altMultiverse[wLayer] : multiverse[wLayer];
                for (int z = 0; z < zDim; z++)
                {
                    for (int y = 0; y < yDim; y++)
                    {
                        for (int x = 0; x < xDim; x++)
                            Console.Write(universe[z][y][x] ? '#' : '.');
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                    Console.WriteLine("Z=" + z);
                    Console.WriteLine();
                }
            }
            catch
            {//universe doesn't exist
                for (int y = 0; y < yDim; y++)
                    Console.WriteLine(ConsoleAssist.Center("<EMPTY>", xDim, '-'));
            }
        }

        private void AdjustBorder()
        {//Make sure all universes are expanded enough. or unexpand parts, that are too much. (we don't want any black holes* appearing)
            //get the first universe to contain a cube
            int minW = multiverse.FindIndex(
                w => w.FirstOrDefault(
                    z => z.FirstOrDefault(
                        y => y.Contains(true)
                    ) != default(List<bool>)
                ) != default(List<List<bool>>));
            //get the last niverse to contain a cube
            int maxW = multiverse.FindLastIndex(
                w => w.LastOrDefault(
                    z => z.LastOrDefault(
                        y => y.Contains(true)
                    ) != default(List<bool>)
                ) != default(List<List<bool>>));

            int minZ = -1;
            int maxZ = -1;
            int minY = -1;
            int maxY = -1;
            int minX = -1;
            int maxX = -1;
            foreach (var universe in multiverse)
            {//check each universe for which z space contains the first and last cube
                int currMinZ = universe.FindIndex(z => z.FirstOrDefault(y => y.Contains(true)) != default(List<bool>));
                int currMaxZ = universe.FindLastIndex(z => z.LastOrDefault(y => y.Contains(true)) != default(List<bool>));
                if (currMinZ >= 0 && (currMinZ < minZ || minZ == -1)) minZ = currMinZ;
                if (currMaxZ > maxZ || maxZ == -1) maxZ = currMaxZ;
                foreach (var layer in universe)
                {//check each z space for wich y space contains the first and last cube
                    int currMinY = layer.FindIndex(y => y.Contains(true));
                    int currMaxY = layer.FindLastIndex(y => y.Contains(true));
                    if (currMinY >= 0 && (currMinY < minY || minY == -1)) minY = currMinY;
                    if (currMaxY > maxY || maxY == -1) maxY = currMaxY;
                    foreach (var column in layer)
                    {//check each y space for wich x space contains the first and the last cube
                        int currMinX = column.FindIndex(x => x);
                        int currMaxX = column.FindLastIndex(x => x);
                        if (currMinX >= 0 && (currMinX < minX || minX == -1)) minX = currMinX;
                        if (currMaxX > maxX || maxX == -1) maxX = currMaxX;
                    }
                }
            }

            //-----------------
            // create a border of 1 in each direction. delete spaces over this and add spaces when there is no border
            //-----------------
            if (moreDimensions)
            {//only adjust the universe number if required (part 2)
                if (minW == 0)
                {
                    IncreaseW(true);
                    ++maxW;
                }
                if (maxW == wDim - 1)
                    IncreaseW(false);
                if (minW == 2)
                {
                    DecreaseW(true);
                    --maxW;
                }
                if (maxW == wDim - 3)
                    DecreaseW(false);
            }

            //adjust z space
            if (minZ == 0)
            {
                IncreaseZ(true);
                ++maxZ;
            }
            if (maxZ == zDim - 1)
                IncreaseZ(false);
            if (minZ == 2)
            {
                DecreaseZ(true);
                --maxZ;
            }
            if (maxZ == zDim - 3)
                DecreaseZ(false);


            //adjust y space
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


            //adjust x space
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

        private int ConwayStep()
        {
            AdjustBorder();
            int fields = 0;
            //Do conway for each universe
            for (int w = 0; w < wDim; w++)
            {
                fields += ConwayLayer(w);
                PrintUniverse(w, true);
            }

            Console.WriteLine();
            Console.WriteLine(ConsoleAssist.Center("", xDim, '-'));
            Console.WriteLine();

            //swap out the active multiverse with the alternative
            //we don't regenerate the alternative multiverse. Why thjrow away, what's perfectly fine for recycling? (Also Runtime stuff idk)
            var bufUniverse = multiverse;
            multiverse = altMultiverse;
            altMultiverse = bufUniverse;
            return fields;
        }

        //originally this part was supposed to be multitasked, hence the seperate function
        private int ConwayLayer(object wLayer)
        {
            int cubes = 0;
            int w = (int)wLayer;
            //go over every cube in this universe
            for (int z = 0; z < zDim; z++)
                for (int y = 0; y < yDim; y++)
                    for (int x = 0; x < xDim; x++)
                    {
                        //count the neighbours for the current cube
                        int neighbours = CountNeighbours(x, y, z, w);
                        //the multiverse we write to got recycled and may still contain a cube. So we remove it
                        altMultiverse[w][z][y][x] = false;
                        //check if we need to set a new cube according to the rules. and count the cubes
                        if ((multiverse[w][z][y][x] && neighbours >= 2 && neighbours <= 3) || (!multiverse[w][z][y][x] && neighbours == 3))
                        {
                            altMultiverse[w][z][y][x] = true;
                            ++cubes;
                        }
                    }
            return cubes;
        }

        private int CountNeighbours(int xStart, int yStart, int zStart, int wStart)
        {
            //neighbours are offset by 1 universe, z y or x space
            int neighbours = 0;
            for (int w = wStart - 1; w <= wStart + 1; w++)
                for (int z = zStart - 1; z <= zStart + 1; z++)
                    for (int y = yStart - 1; y <= yStart + 1; y++)
                        for (int x = xStart - 1; x <= xStart + 1; x++)
                        {
                            if (x == xStart && y == yStart && z == zStart && w == wStart) continue; // skip the requested cube
                            // we may get out of range at the border (universe expands as requested)
                            if (x < 0 || x >= xDim || y < 0 || y >= yDim || z < 0 || z >= zDim || w < 0 || w >= wDim) continue; 
                            //count if there is a cube
                            if (multiverse[w][z][y][x]) ++neighbours;
                        }
            return neighbours;
        }
    }
}
// *by black holes i mean Memory overusage, which can negatively impact performance. and memory space... so it somewhat is like a black hole i guess