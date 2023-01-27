using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Extensions
{
    public static class BitmapExtension
    {
        public static void FillRect(this Bitmap bmp, int x, int y, int width, int height, Color color) =>
            FillRect(bmp, new Rectangle(x, y, width, height), color);
        public static void FillRect(this Bitmap bmp, Rectangle rect, Color color)
        {
            var lockInfo = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var channelArray = new byte[Math.Abs(lockInfo.Stride) * lockInfo.Height];
            for (int top = 0; top < lockInfo.Height; ++top)
                for (int left = 0; left < lockInfo.Width; ++left)
                    for (int channel = 0; channel < 3; ++channel)
                    {
                        int pos = top * lockInfo.Stride + left * 3 + channel;
                        switch (channel % 3)
                        {
                            case 0: channelArray[pos] = color.B; break;
                            case 1: channelArray[pos] = color.G; break;
                            case 2: channelArray[pos] = color.R; break;
                        }
                    }
            System.Runtime.InteropServices.Marshal.Copy(channelArray, 0, lockInfo.Scan0, channelArray.Length);
            bmp.UnlockBits(lockInfo);
        }

        public static void DrawLine(this Bitmap bmp, Point start, Point end, Color color, int size = 1)
        {
            DrawPoints(bmp, VectorAssist.GetPointsFromLine(start, end), color, size);
        }

        public static void DrawPoints(this Bitmap bmp, IEnumerable<Point> points, Color color, int size = 1)
        {
            if (size < 1) throw new ArgumentOutOfRangeException(nameof(size), "Size must be at least 1");
            int offset = (size - 1) / 2;

            foreach (Point point in points)
                FillRect(bmp, new Rectangle(point.X - offset, point.Y - offset, size, size), color);
        }
    }
}
