using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.Visualization
{
    public static class BitmapExtension
    {
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
    }
}
