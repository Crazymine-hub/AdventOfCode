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

        public int XOrigin { get; private set; }
        public int YOrigin { get; private set; }

        public DynamicGrid(int dimX = 1, int dimY = 1)
        {
            grid = new List<List<T>>();
            while (XDim < dimX) IncreaseX(false);
            while (YDim < dimY) IncreaseY(false);
        }

        public void IncreaseX(bool front)
        {
            for (int y = 0; y < YDim; y++)
                grid[y].Insert(front ? 0 : XDim, default);
            ++XDim;
            if (front)
                ++XOrigin;
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
            if (front)
                ++YOrigin;
        }

        public void DecreaseX(bool front)
        {
            for (int y = 0; y < YDim; y++)
                grid[y].RemoveAt(front ? 0 : XDim - 1);
            --XDim;
            if (front)
                --XOrigin;
        }

        public void DecreaseY(bool front)
        {
            grid.RemoveAt(front ? 0 : YDim - 1);
            --YDim;
            if (front)
                --YOrigin;
        }

        public T GetRelative(int x, int y)
        {
            return this[XOrigin + x, YOrigin + y];
        }

        public void SetRelative(int x, int y, T value)
        {
            this[XOrigin + x, YOrigin + y] = value;
        }

        public void CutDown(Predicate<T> isEmpty = null)
        {
            if (isEmpty == null) isEmpty = new Predicate<T>(x => x.Equals(default(T)));

            while (grid[0].All(x => isEmpty(x))) DecreaseY(true);
            while (grid.Last().All(x => isEmpty(x))) DecreaseY(false);
            while (grid.All(x => isEmpty(x[0]))) DecreaseX(true);
            while (grid.All(x => isEmpty(x.Last()))) DecreaseX(false);
        }

        public T this[int x, int y]
        {
            get => grid[y][x];
            set
            {
                while (x >= XDim) IncreaseX(false);
                while (x < 0)
                {
                    IncreaseX(true);
                    ++x;
                }
                while (y >= YDim) IncreaseY(false);
                while (y < 0)
                {
                    IncreaseY(true);
                    ++y;
                }

                grid[y][x] = value;
            }
        }

        public string ToString(Func<T, int, int, string> stringConverter, Func<string, int, string> lineEndHandler = null)
        {
            StringBuilder result = new StringBuilder();
            for (int y = 0; y < YDim; ++y)
            {
                string line = "";
                for (int x = 0; x < XDim; ++x)
                    line += stringConverter(this[x, y], x, y);
                if (lineEndHandler != null)
                    line = lineEndHandler(line, y);
                result.AppendLine(line);
            }
            return result.ToString().Remove(result.Length - Environment.NewLine.Length);
        }

        public long Count(Predicate<T> checker = null)
        {
            long result = 0;
            for (int y = 0; y < YDim; ++y)
                for (int x = 0; x < XDim; ++x)
                    if (checker == null || checker(this[x, y])) ++result;
            return result;
        }
    }
}