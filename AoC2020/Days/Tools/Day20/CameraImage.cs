﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day20
{
    class CameraImage
    {
        //Y X
        //Image is assumed to be static and no pixels missing
        public List<List<bool>> Image { get; private set; }
        public int ID { get; }


        //Borders are Assembled Left to Right or Top to Bottom.
        public IReadOnlyList<bool> TopBorder
        {
            get
            {
                return Image[0].ToList();
            }
        }
        public IReadOnlyList<bool> BottomBorder
        {
            get
            {
                return Image.Last().ToList();
            }
        }
        public IReadOnlyList<bool> LeftBorder
        {
            get
            {
                List<bool> border = new List<bool>();
                for (int i = 0; i < Image.Count; ++i)
                    border.Add(Image[i][0]);
                return border;
            }
        }
        public IReadOnlyList<bool> RightBorder
        {
            get
            {
                List<bool> border = new List<bool>();
                for (int i = 0; i < Image.Count; ++i)
                    border.Add(Image[i].Last());
                return border;
            }
        }

        public CameraImage(List<string> imageInfo)
        {
            ID = int.Parse(imageInfo[0].Substring(5, imageInfo[0].Length - 6));
            Image = new List<List<bool>>();

            for (int line = 1; line < imageInfo.Count; ++line)
            {
                List<bool> row = new List<bool>();
                foreach (char pixel in imageInfo[line])
                    row.Add(pixel == '#');
                Image.Add(row);
            }
        }

        public void RotateLeft()
        {
            List<List<bool>> newImg = new List<List<bool>>();
            for(int x = 0; x < Image[0].Count; ++x)
            {
                List<bool> row = new List<bool>();
                for (int y = 0; y < Image.Count; ++y)
                    row.Insert(0, Image[y][x]);
                newImg.Add(row);
            }
            Image = newImg;
        }

        public void RotateRight()
        {
            List<List<bool>> newImg = new List<List<bool>>();
            for (int x = 0; x < Image[0].Count; ++x)
            {
                List<bool> row = new List<bool>();
                for (int y = 0; y < Image.Count; ++y)
                    row.Add(Image[y][x]);
                newImg.Insert(0, row);
            }
            Image = newImg;
        }

        public override string ToString()
        {
            string outp = "";
            for (int y = 0; y < Image[0].Count; ++y)
            {
                for (int x = 0; x < Image.Count; ++x)
                    outp += Image[y][x] ? "█" : " ";
                outp += "\r\n";
            }
            return outp.Remove(outp.Length - 2);
        }
    }
}
