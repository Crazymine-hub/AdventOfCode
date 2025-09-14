using AdventOfCode.Days.Tools.Day25;
using AdventOfCode.Tools.DynamicGrid;
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
    public class Day25 : DayBase
    {
        public override string Title => "Sea Cucumber";
        VisualFormHandler _visualForm = VisualFormHandler.GetInstance();
        private const int ImageScale = 5;

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            _visualForm.Show();

            var map = LoadMap(input);
            RenderMap(map);
            return $"Cucumbers can move {Simulate(map)} times";
        }

        private DynamicGrid<CucumberState> LoadMap(string input)
        {
            DynamicGrid<CucumberState> map = new DynamicGrid<CucumberState>();
            var lines = GetLines(input);
            for (int y = 0; y < lines.Count; ++y)
            {
                var line = lines[y];
                for(int x = 0; x < line.Count(); ++x)
                {
                    var state = CucumberState.Empty;
                    switch(line[x])
                    {
                        case '>': state = CucumberState.East;
                            break;
                        case 'v': state = CucumberState.South;
                            break;
                        case '.': state = CucumberState.Empty;
                            break;
                        default: throw new ArgumentException($"Unable to identify sea cucumber at {line[x]} ({x}|{y})");
                    }
                    map.SetRelative(x, y, state);
                }
            }
            return map;
        }

        private int Simulate(DynamicGrid<CucumberState> map)
        {
            int moveCounter = 0;
            bool hasMovement;
            Task delayTask = Task.Delay(100);
            do
            {
                DynamicGrid<CucumberState> newState = new DynamicGrid<CucumberState>();
                hasMovement = MoveEast(map, newState);
                hasMovement = hasMovement | MoveSouth(map, newState);
                map = newState;
                ++moveCounter;
                Console.WriteLine(moveCounter);
                RenderMap(newState);
                delayTask.Wait();
                delayTask = Task.Delay(50);
            } while (hasMovement);
            return moveCounter;
        }

        private bool MoveSouth(DynamicGrid<CucumberState> originGrid, DynamicGrid<CucumberState> targetGrid)
        {
            bool hasMoved = false;
            foreach(var cell in originGrid.Where(x => x.Value == CucumberState.South))
            {
                var targetY = 0;
                if (originGrid.InRange(cell.X, cell.Y + 1))
                    targetY = cell.Y + 1;
                targetGrid.MakeAvaliable(cell.X, targetY);
                if (originGrid.GetRelative(cell.X, targetY) != CucumberState.South &&
                    targetGrid.GetRelative(cell.X, targetY) != CucumberState.East)
                {
                    targetGrid.SetRelative(cell.X, targetY, cell.Value);
                    hasMoved = true;
                }
                else
                    targetGrid.SetRelative(cell.X, cell.Y, cell.Value);
            }
            return hasMoved;
        }

        private bool MoveEast(DynamicGrid<CucumberState> originGrid, DynamicGrid<CucumberState> targetGrid)
        {

            bool hasMoved = false;
            foreach (var cell in originGrid.Where(x => x.Value == CucumberState.East))
            {
                var targetX = 0;
                if (originGrid.InRange(cell.X + 1, cell.Y))
                    targetX = cell.X + 1;
                if (originGrid.GetRelative(targetX, cell.Y) == CucumberState.Empty)
                {
                    targetGrid.SetRelative(targetX, cell.Y, cell.Value);
                    hasMoved = true;
                }
                else
                    targetGrid.SetRelative(cell.X, cell.Y, cell.Value);
            }
            return hasMoved;
        }

        private void RenderMap(DynamicGrid<CucumberState> map)
        {
            var image = new Bitmap(map.XDim * ImageScale, map.YDim * ImageScale);
            foreach (var cell in map)
            {
                Color color = Color.Black;
                switch (cell.Value)
                {
                    case CucumberState.East: color = Color.Lime;
                        break;
                    case CucumberState.South: color = Color.Orange;
                        break;
                }
                image.FillRect(new Rectangle(cell.X * ImageScale, cell.Y * ImageScale, ImageScale , ImageScale), color);
            }
            Console.WriteLine(map.GetStringRepresentation((state, x,y) => { 
                switch (state)
                {
                    case CucumberState.East: return ">";
                    case CucumberState.South: return "v";
                    default: return ".";
                }
            }));
            Console.WriteLine(string.Empty.PadLeft(10, '='));
            _visualForm.Update(image, false);
        }
    }
}
