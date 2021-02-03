using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools
{
    public class DynamicGrid<T>
    {
        private List<List<T>> grid = new List<List<T>>();

        public int XDim { get; private set; }
        public int YDim { get; private set; }

        public DynamicGrid(int dimX = 1, int dimY = 1)
        {
            grid = new List<List<T>>();
            XDim = dimX;
            YDim = dimY;
        }

        public void IncreaseX(bool front)
        {
            for (int y = 0; y < YDim; y++)
                grid[y].Insert(front ? 0 : XDim, default);
            ++XDim;
        }

        public void IncreaseY(bool front)
        {
            List<T> row = new List<T>();
            for (int x = 0; x < XDim; x++)
            {
                row.Add(default);
            }
            grid.Insert(front ? 0 : YDim, row);
            ++YDim;
        }

        public void DecreaseX(bool front)
        {
            for (int y = 0; y < YDim; y++)
                grid[y].RemoveAt(front ? 0 : XDim);
            --XDim;
        }

        public void DecreaseY(bool front)
        {
            grid.RemoveAt(front ? 0 : YDim - 1);
            --YDim;
        }



        public T this[int x, int y]
        {
            get => grid[y][x];
            set
            {
                while (x > XDim) IncreaseX(false);
                while (x < 0)
                {
                    IncreaseX(true);
                    ++x;
                }
                while (y > XDim) IncreaseY(false);
                while (y < 0)
                {
                    IncreaseX(true);
                    ++y;
                }

                grid[y][x] = value;
            }
        }

        public string ToString(Func<T, int, int, string> stringConverter, Func<string, string> lineEndHandler = null)
        {
            StringBuilder result = new StringBuilder();
            for (int y = 0; y < YDim; ++y)
            {
                string line = "";
                for (int x = 0; x < XDim; ++x)
                    line += stringConverter(this[x, y], x, y);
                if (lineEndHandler != null)
                    line = lineEndHandler(line);
                result.AppendLine(line);
            }
            return result.ToString().Remove(result.Length - Environment.NewLine.Length);
        }
    }
}