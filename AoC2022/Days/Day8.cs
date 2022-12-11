using AdventOfCode.Days.Tools.Day8;
using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day8 : DayBase
    {
        public override string Title => "Treetop Tree House";

        DynamicGrid<Tree> treeGrid = new DynamicGrid<Tree>();

        public override string Solve(string input, bool part2)
        {
            GetTreeGrid(input);
            RenderTreeGrid();
            if (!part2)
                return $"There are {treeGrid.Count(x => x.Value.IsVisible)} trees visible";
            CalculateScenicScores();
            return $"Highest Scenic Score possible is {treeGrid.Max(x => x.Value.ScenicScore)}";
        }

        private void CalculateScenicScores()
        {
            foreach (var tree in treeGrid)
                GetTreeScenicScore(tree);
        }

        private void GetTreeScenicScore(DynamicGridValue<Tree> tree)
        {
            var topDistance = 0;
            for (int y = tree.Y - 1; y >= 0; --y)
            {
                ++topDistance;
                var analyzedTree = treeGrid.GetRelative(tree.X, y);
                if (analyzedTree.Size >= tree.Value.Size)
                    break;
            }
            var bottomDistance = 0;
            for (int y = tree.Y + 1; y < treeGrid.YDim; ++y)
            {
                ++bottomDistance;
                var analyzedTree = treeGrid.GetRelative(tree.X, y);
                if (analyzedTree.Size >= tree.Value.Size)
                    break;
            }
            var leftDistance = 0;
            for (int x = tree.X - 1; x >= 0; --x)
            {
                ++leftDistance;
                var analyzedTree = treeGrid.GetRelative(x, tree.Y);
                if (analyzedTree.Size >= tree.Value.Size)
                    break;
            }
            var rightDistance = 0;
            for (int x = tree.X + 1; x < treeGrid.XDim; ++x)
            {
                ++rightDistance;
                var analyzedTree = treeGrid.GetRelative(x, tree.Y);
                if (analyzedTree.Size >= tree.Value.Size)
                    break;
            }

            tree.Value.ScenicScore = topDistance * bottomDistance * leftDistance * rightDistance;
        }

        private void RenderTreeGrid()
        {
            Console.Clear();
            Console.WriteLine(treeGrid.GetStringRepresentation((tree, x, y) => tree.IsVisible ? tree.Size.ToString() : " "));
        }

        private void GetTreeGrid(string input)
        {
            var lines = GetLines(input);
            List<int> maxHeightY = new List<int>();
            List<int> maxHeightX = new List<int>();
            for (int y = 0; y < lines.Count; ++y)
            {
                var line = lines[y];
                for (int x = 0; x < line.Length; ++x)
                {
                    var tree = new Tree(int.Parse(line[x].ToString()));
                    treeGrid.SetRelative(x, y, tree);
                    if (maxHeightY.Count <= x)
                        maxHeightY.Add(-1);
                    if (maxHeightX.Count <= y)
                        maxHeightX.Add(-1);
                    if (tree.Size > maxHeightY[x])
                    {
                        tree.IsVisible = true;
                        maxHeightY[x] = tree.Size;
                    }
                    if (tree.Size > maxHeightX[y])
                    {
                        tree.IsVisible = true;
                        maxHeightX[y] = tree.Size;
                    }
                }
            }


            maxHeightY = Enumerable.Repeat(-1, treeGrid.XDim).ToList();
            maxHeightX = Enumerable.Repeat(-1, treeGrid.YDim).ToList();
            for (int y = treeGrid.YDim - 1; y >= 0; --y)
            {
                for (int x = treeGrid.XDim - 1; x >= 0; --x)
                {
                    var tree = treeGrid.GetRelative(x, y);
                    if (tree.Size > maxHeightY[x])
                    {
                        tree.IsVisible = true;
                        maxHeightY[x] = tree.Size;
                    }
                    if (tree.Size > maxHeightX[y])
                    {
                        tree.IsVisible = true;
                        maxHeightX[y] = tree.Size;
                    }
                }
            }
        }
    }
}
