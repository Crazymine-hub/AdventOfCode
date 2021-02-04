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
            string prefix = "".PadLeft(lobby.YDim - y);
            prefix += lobby[0, y] ? "▐" : " ";
            return prefix + line;
        }

        private string GetTileString(bool value, int x, int y)
        {
            string result = value ? "█" : " ";
            if (x + 1 < lobby.XDim && lobby[x + 1, y])
                result += value ? "█" : "▐";
            else
                result += value ? "▌" : " ";
            return result;
        }

        private void ToggleTile(string tile)
        {
            int x = 0, y = 0;
            for (int i = 0; i < tile.Length; ++i)
            {
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
            lobby.IncreaseX(true);
            lobby.IncreaseX(false);
            lobby.IncreaseY(true);
            lobby.IncreaseY(false);
            DynamicGrid<bool> newLobby = new DynamicGrid<bool>(lobby.XDim, lobby.YDim);
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
            newLobby.CutDown();
            lobby = newLobby;
        }

        private int GetTileNeighbours(int xStart, int yStart)
        {
            int neighbours = 0;
            for (int y = -1; y <= 1; ++y)
                for (int xOff = -1; xOff <= 0; ++xOff)
                {
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
