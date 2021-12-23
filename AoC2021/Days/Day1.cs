using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day1 : DayBase
    {
        public override string Title => "Sonar Sweep";

        public override string Solve(string input, bool part2)
        {
            int[] depths = GetLines(input).Select(x => int.Parse(x)).ToArray();

            if (part2) depths = Smooth(depths);

            return GetFalloff(depths);
        }

        private int[] Smooth(int[] depths)
        {
            int[] result = new int[depths.Length - 2];
            for (int i = 0; i < depths.Length - 2; ++i)
                result[i] = depths[i] + depths[i + 1] + depths[i + 2];
            return result;
        }

        private string GetFalloff(int[] depths)
        {
            int minDepth = depths.Min();
            const int scale = 1;

            Bitmap bmp = new Bitmap(depths.Length * scale + 10, (depths.Max() - minDepth) * scale + 10);
            VisualFormHandler.Instance.Show((Bitmap)bmp.Clone());

            int previous = depths.First();
            int increase = 0;
            Console.CursorTop = depths[0] - minDepth;
            for (int i = 0; i < depths.Length; ++i)
                CheckDepth(depths[i], minDepth, scale, bmp, ref previous, ref increase, i);
            Console.WriteLine();
            return $"Depth Increasing Rate: {increase}";
        }

        private static void CheckDepth(int depth, int minDepth, int scale, Bitmap bmp, ref int previous, ref int increase, int left)
        {
            if (depth > previous)
            {
                increase++;
                ++Console.CursorTop;
                Console.Write("\\");
            }
            else if (depth < previous)
            {
                Console.Write("/");
                --Console.CursorTop;
            }
            else Console.Write("_");

            DrawVisualization(depth, minDepth, scale, bmp, previous, left);

            previous = depth;
        }

        private static void DrawVisualization(int depth, int minDepth, int scale, Bitmap bmp, int previous, int left)
        {
            Rectangle pixelArea = new Rectangle(left * scale + 5, (depth - minDepth) * scale + 5, scale, scale);
            var bmpPixels = bmp.LockBits(pixelArea, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int length = Math.Abs(bmpPixels.Stride) * bmpPixels.Height;
            byte[] pixelData = new byte[length];
            for (int channel = 0; channel < length; ++channel)
            {
                if (depth >= previous && channel % 3 == 2)
                    pixelData[channel] = 0xFF;
                if (depth <= previous && channel % 3 == 1)
                    pixelData[channel] = 0xFF;

            }
            System.Runtime.InteropServices.Marshal.Copy(pixelData, 0, bmpPixels.Scan0, length);
            bmp.UnlockBits(bmpPixels);
            VisualFormHandler.Instance.Update(bmp);
            VisualFormHandler.Instance.SetFocusTo(pixelArea.X, pixelArea.Y);
        }
    }
}
