using AdventOfCode.Tools;
using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.SpecificBitwise;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day20 : DayBase, IDisposable
    {
        public override string Title => "Trench Map";
        private readonly List<bool> algorithm = new List<bool>();
        private DynamicGrid<bool> image = new DynamicGrid<bool>();
        private readonly VisualFormHandler form = VisualFormHandler.GetInstance();
        const int scale = 10;
        Bitmap renderedImage;
        private bool wasDisposed;
        private bool emptySpaceValue = false;

        public override string Solve(string input, bool part2)
        {
            List<string> maps = GetGroupedLines(input);
            foreach (char bit in maps[0].Replace(Environment.NewLine, ""))
                algorithm.Add(bit == '#');

            maps = GetLines(maps[1]);
            for (int y = 0; y < maps.Count; ++y)
            {
                string line = maps[y];
                for (int x = 0; x < line.Length; ++x)
                    image.SetRelative(x, y, line[x] == '#');
            }


            form.Show();
            image.AddMargin();
            RenderImage();

            int enhanceCount = part2 ? 50 : 2;
            for (int i = 0; i < enhanceCount; ++i)
            {
                Task.Delay(10).Wait();
                image = Enhance(image);
                emptySpaceValue = algorithm[IntBitwise.GetValue(Enumerable.Repeat(emptySpaceValue, 9))];
                image.AddMargin();
                Console.Clear();
                Console.WriteLine($"Run {i + 1}/{enhanceCount} ({(i + 1) / 50.0 * 100}%)");
                RenderImage();
            }

            return $"{image.Count(x => x)} pixels are lit.";
        }

        private void RenderImage()
        {
            renderedImage = new Bitmap(image.XDim * scale, image.YDim * scale);
            for (int y = 0; y < image.YDim; ++y)
            {
                for (int x = 0; x < image.XDim; ++x)
                {
                    Console.Write(image[x, y] ? '#' : ' ');
                    renderedImage.FillRect(new Rectangle(x * scale, y * scale, scale, scale), image[x, y] ? Color.White : Color.Black);
                }
                Console.WriteLine();
            }
            form.Update(renderedImage);
        }

        private DynamicGrid<bool> Enhance(DynamicGrid<bool> image)
        {
            var newImage = new DynamicGrid<bool>();
            newImage.GetDefault += Image_GetDefault;
            foreach (var pixel in image)
            {
                var neighbours = image.GetNeighbours(pixel.X, pixel.Y);
                int index = IntBitwise.GetValue(neighbours.Select(x => x.Value).Reverse());
                bool enabled = algorithm[index];
                newImage.SetRelative(pixel.X, pixel.Y, enabled);
            }

            return newImage;
        }

        private bool Image_GetDefault() => emptySpaceValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!wasDisposed)
            {
                if (disposing)
                {
                    renderedImage.Dispose();
                }
                wasDisposed = true;
            }
        }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
