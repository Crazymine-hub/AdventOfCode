using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Tools;

namespace AdventOfCode.Days.Tools.Day20
{
    class CameraImage
    {
        //Y X
        //Image is assumed to be static and no pixels missing
        public List<List<bool>> Image { get; private set; }
        public int ID { get; }


        //Borders are Assembled Left to Right or Top to Bottom.
        public IReadOnlyList<bool> TopBorder => Image[0].ToList();
        public int TopBorderNr => Bitwise.GetValue<int>(TopBorder);
        public IReadOnlyList<bool> BottomBorder => Image.Last().ToList();
        public int BottomBorderNr => Bitwise.GetValue<int>(BottomBorder);

        public IReadOnlyList<bool> LeftBorder => GetColumnList(0);
        public int LeftBorderNr => Bitwise.GetValue<int>(LeftBorder);

        public IReadOnlyList<bool> RightBorder => GetColumnList(Image[0].Count - 1);
        public int RightBorderNr => Bitwise.GetValue<int>(RightBorder);


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

        //that's clockwise
        public void RotateRight()
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

        //Fun fact: we never rotate counterclockwise
        public void RotateLeft()
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

        //we only need to flip along one axis, since rotation takes care of the flip along the other axis
        public void Flip()
        {
            List<List<bool>> newImg = new List<List<bool>>();
            for (int y = 0; y < Image[0].Count; ++y)
            {
                newImg.Insert(0, Image[y]);
            }
            Image = newImg;
        }

        public IReadOnlyList<bool> GetColumnList(int column)
        {
            List<bool> border = new List<bool>();
            for (int i = 0; i < Image.Count; ++i)
                border.Add(Image[i][column]);
            return border;
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool stripBorder)
        {
            string outp = "";
            int offset = stripBorder ? 1 : 0;
            for (int y = offset; y < Image.Count - offset; ++y)
            {
                for (int x = offset; x < Image[0].Count - offset; ++x)
                    outp += Image[y][x] ? "#" : ".";
                outp += "\r\n";
            }
            return outp.Remove(outp.Length - 2);
        }
    }
}
