using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Day24 : DayBase
    {
        public override string Title => "Lobby Layout";

        DynamicGrid<bool> lobby = new DynamicGrid<bool>();

        public override string Solve(string input, bool part2)
        {
            foreach (string tile in GetLines(input.ToLower()))
                ToggleTile(tile);

            if (part2)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < 100; ++i)
                {
                    CancellationToken.ThrowIfCancellationRequested();
                    Console.WriteLine(lobby.ToString(GetTileString, CorrectLine));
                    Console.WriteLine($"{i}: Found {lobby.Count(x => x)} black tiles. Elapsed: {stopwatch.Elapsed} Remaining: {new TimeSpan(stopwatch.ElapsedTicks / (i+1) * (100 - i))}");
                    Conway();
                    Console.WriteLine();
                }
                stopwatch.Stop();
            }
            Console.WriteLine(lobby.ToString(GetTileString, CorrectLine));


            return $"Found {lobby.Count(x => x)} black tiles";
        }

        private string CorrectLine(string line, int y)
        {
            //pad the lines and add the outer left tile border.
            //since we need to shift the quadratic display to the right further and further.
            string prefix = "".PadLeft(lobby.YDim - y);
            prefix += lobby[0, y] ? "▐" : " ";
            return prefix + line;
        }

        private string GetTileString(bool value, int x, int y)
        {
            // a hexagonal tile is hard to render in the terminal (ascii art gets large quickly)
            // therefore a tile is always three chars wide, whereby the outers change depending on the neighbour
            //char for the current tile is full or empty (black or white.)
            string result = value ? "█" : " ";
            if (x + 1 < lobby.XDim && lobby[x + 1, y])
                //the right neighbour is active. fill, if we are active, else only the right half
                result += value ? "█" : "▐";
            else
                //the right neighbour is inactive. fill left half if we are active, leave empty if we are also inactive
                result += value ? "▌" : " ";
            //we don't need to consider letf, since it's filled as the previous ones right already.
            return result;
        }

        private void ToggleTile(string tile)
        {
            int x = 0, y = 0;

            /* The Hexagonal grid is projected onto a normal (quadratic) grid.
             * Straight up in quadratic is bottom left, to top right. Hence the following offsets from the center tile
             *  / \
             * | X |                One row in quadratic    Quadratic projection:
             * | Y |                is projected as         +-+-+-+
             *  \ /                 shown below             |-|0| |
             *                                              |+|+| |
             *   / \ / \                                    +-+-+-+
             *  | - | 0 |                                   |-|0|+|
             *  | + | + |             ____Top               |0|0|0|
             * / \ / \ / \               /|                 +-+-+-+
             *| - | 0 | + |             / |                 | |0|+|
             *| 0 | 0 | 0 |            /  |                 | |-|-|
             * \ / \ / \ /            /                     +-+-+-+
             *  | 0 | + |            /              
             *  | - | - |           /               
             *   \ / \ /            Bottom         
             *    
             */

            for (int i = 0; i < tile.Length; ++i)
            {//Determine the path of the tiles to find the tile to toggle
                switch (tile[i])
                {
                    case 'e':
                        ++x;
                        break;
                    case 'w':
                        --x;
                        break;
                    case 'n':
                        --y;
                        if (tile[++i] == 'w')
                            --x;
                        break;
                    case 's':
                        ++y;
                        if (tile[++i] == 'e')
                            ++x;
                        break;
                }
            }

            //Toggle the target tile. If it isn't in the defined area, we use the default (white) to toggle.
            bool lobbyValue = false;
            try
            {
                lobbyValue = lobby.GetRelative(x, y);
            }
            catch { /*The grid isn't large enough (yet) we use the default and insert it, since that will resize the grid*/ }
            lobby.SetRelative(x, y, !lobbyValue);
        }

        private void Conway()
        {
            //increase the border to easily allow detection of border tiles (if the borders were to be realigned, mid conway, it would break)
            lobby.IncreaseX(true);
            lobby.IncreaseX(false);
            lobby.IncreaseY(true);
            lobby.IncreaseY(false);
            //create a copy of our lobby with the lobbys dimensions
            DynamicGrid<bool> newLobby = new DynamicGrid<bool>(lobby.XDim, lobby.YDim);
            //classic conway. determine the new state based on the counted neighbours
            for (int y = 0; y < lobby.YDim; ++y)
                for (int x = 0; x < lobby.XDim; ++x)
                {
                    int neighbours = GetTileNeighbours(x, y);
                    bool tileState = false;
                    if (lobby[x, y])
                        tileState = neighbours >= 1 && neighbours <= 2;
                    else
                        tileState = neighbours == 2;
                    newLobby[x, y] = tileState;
                }
            //remove excess borders, to avoid excessive growth (it's pretty large as it is.)
            newLobby.CutDown();
            lobby = newLobby;
        }

        private int GetTileNeighbours(int xStart, int yStart)
        {
            int neighbours = 0;
            for (int y = -1; y <= 1; ++y)
                for (int xOff = -1; xOff <= 0; ++xOff)
                {
                    //on the mid we need to shift the second to get the right neighbor and not ourself.
                    //on the bottom row, the neighbours are generally shifted in memory.
                    int x = xOff;
                    if (y == 0 && xOff == 0 || y == 1) ++x;

                    try
                    {
                        if (lobby[xStart + x, yStart + y]) ++neighbours;
                    }
                    catch {/*We are outside of the available space, just don't count this neighbour*/}
                }
            return neighbours;
        }
    }
}
