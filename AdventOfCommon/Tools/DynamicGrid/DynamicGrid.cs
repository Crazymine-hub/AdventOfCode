using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools.DynamicGrid
{
    public sealed class DynamicGrid<T> : IEnumerable<DynamicGridValue<T>>
    {
        private readonly List<List<List<T>>> grid;
        public const int DefaultLayer = 0;

        public int XDim { get; private set; }
        public int YDim { get; private set; }
        public int ZDim { get; private set; }

        public int XOrigin { get; private set; }
        public int YOrigin { get; private set; }
        public int ZOrigin { get; private set; }

        public event Func<T> GetDefault;

        public DynamicGrid(int dimX = 1, int dimY = 1, int dimZ = 1, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        {
            grid = new List<List<List<T>>>();
            while (XDim < dimX) IncreaseX(false);
            while (YDim < dimY) IncreaseY(false);
            while (ZDim < dimZ) IncreaseZ(false);
            XOrigin = -xOffset;
            YOrigin = -yOffset;
            ZOrigin = -zOffset;
        }

        private T GetDefaultValue()
        {
            if (GetDefault == null)
                return default(T);
            return GetDefault();
        }

        public void IncreaseX(bool front)
        {
            for (int z = 0; z < ZDim; z++)
                for (int y = 0; y < YDim; y++)
                    grid[z][y].Insert(front ? 0 : XDim, GetDefaultValue());
            ++XDim;
            if (front)
                ++XOrigin;
        }

        public void IncreaseY(bool front)
        {
            for (int z = 0; z < ZDim; z++)
            {
                List<T> row = new List<T>();
                for (int x = 0; x < XDim; x++)
                {
                    row.Add(GetDefaultValue());
                }
                grid[z].Insert(front ? 0 : YDim, row);
            }
            ++YDim;
            if (front)
                ++YOrigin;
        }

        public void IncreaseZ(bool front)
        {
            List<List<T>> newGrid = new List<List<T>>();
            for (int y = 0; y < YDim; y++)
            {
                List<T> row = new List<T>();
                for (int x = 0; x < XDim; x++)
                {
                    row.Add(GetDefaultValue());
                }
                newGrid.Add(row);
            }
            grid.Insert(front ? 0 : ZDim, newGrid);
            ++ZDim;
            if (front)
                ++ZOrigin;
        }

        public void DecreaseX(bool front)
        {
            for (int z = 0; z < ZDim; z++)
                for (int y = 0; y < YDim; y++)
                    grid[z][y].RemoveAt(front ? 0 : XDim - 1);
            --XDim;
            if (front)
                --XOrigin;
        }

        public void DecreaseY(bool front)
        {
            for (int z = 0; z < ZDim; z++)
                grid[z].RemoveAt(front ? 0 : YDim - 1);
            --YDim;
            if (front)
                --YOrigin;
        }

        public void DecreaseZ(bool front)
        {
            grid.RemoveAt(front ? 0 : ZDim - 1);
            --ZDim;
            if (front)
                --ZOrigin;
        }

        public bool InRange(int x, int y, int z = DefaultLayer) => x >= 0 && x < XDim && y >= 0 && y < YDim && z >= 0 && z < ZDim;

        public T GetRelative(int x, int y, int z = DefaultLayer)
        {
            int newX = XOrigin + x;
            int newY = YOrigin + y;
            int newZ = ZOrigin + z;
            try
            {
                return this[newX, newY, newZ];
            }
            catch (ArgumentOutOfRangeException)
            {
                return GetDefaultValue();
            }
        }

        public void SetRelative(int x, int y, T value)
        {
            SetRelative(x, y, 0, value);
        }

        public void SetRelative(int x, int y, int z, T value)
        {
            MakeAvaliable(XOrigin + x, YOrigin + y, ZOrigin + z);
            this[XOrigin + x, YOrigin + y, ZOrigin + z] = value;
        }

        public void CutDown(Predicate<T> isEmpty = null)
        {
            if (isEmpty == null) isEmpty = new Predicate<T>(x => x.Equals(GetDefaultValue()));

            while (grid.First().All(y => y.All(x => isEmpty(x)))) DecreaseZ(true);
            while (grid.Last().All(y => y.All(x => isEmpty(x)))) DecreaseZ(false);

            while (grid.All(z => z.First().All(x => isEmpty(x)))) DecreaseY(true);
            while (grid.All(z => z.Last().All(x => isEmpty(x)))) DecreaseY(false);

            while (grid.All(z => z.All(y => isEmpty(y.First())))) DecreaseX(true);
            while (grid.All(z => z.All(y => isEmpty(y.Last())))) DecreaseX(false);
        }

        public void AddMargin(int width = 1, bool marginZ = false)
        {
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width), "Value cannot be less than 0.");
            for (int i = 0; i < width; ++i)
            {
                IncreaseX(true);
                IncreaseX(false);
                IncreaseY(true);
                IncreaseY(false);
                if (marginZ)
                {
                    IncreaseZ(true);
                    IncreaseZ(false);
                }
            }
        }

        public void MakeAvaliable(int x, int y, int z = DefaultLayer)
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
            while (z >= ZDim) IncreaseZ(false);
            while (z < 0)
            {
                IncreaseZ(true);
                ++z;
            }
        }

        public IEnumerable<DynamicGridValue<T>> GetNeighbours(int x, int y, int z, bool diagonal = true, bool relative = false)
        {
            if (relative)
            {
                x += XOrigin;
                y += YOrigin;
                z += ZOrigin;
            }
            for (int layer = z - 1; layer <= z + 1; ++layer)
            {
                for (int row = y - 1; row <= y + 1; ++row)
                {
                    for (int column = x - 1; column <= x + 1; ++column)
                    {
                        if (!diagonal && !(layer == z || row == y || column == x)) continue;
                        if (!InRange(column, row, layer))
                        {
                            yield return new DynamicGridValue<T>(column, row, layer, GetDefaultValue());
                            continue;
                        }
                        var cellValue = new DynamicGridValue<T>(column, row, layer, this[column, row, layer]);
                        yield return cellValue;
                    }
                }
            }
        }

        public IEnumerable<DynamicGridValue<T>> GetNeighbours(int x, int y, bool diagonal = true, bool relative = false)
        {
            if (relative)
            {
                x += XOrigin;
                y += YOrigin;
            }
            for (int row = y - 1; row <= y + 1; ++row)
            {
                for (int column = x - 1; column <= x + 1; ++column)
                {
                    if (!diagonal && !(row == y || column == x)) continue;
                    if (!InRange(column, row, DefaultLayer))
                    {
                        yield return new DynamicGridValue<T>(column, row, DefaultLayer, GetDefaultValue());
                        continue;
                    }
                    var cellValue = new DynamicGridValue<T>(column, row, DefaultLayer, this[column, row, DefaultLayer]);
                    yield return cellValue;
                }
            }
        }

        public T this[int x, int y, int z = 0]
        {
            get => grid[z][y][x];
            set
            {
                MakeAvaliable(x, y, z);
                grid[z][y][x] = value;
            }
        }

        public string GetStringRepresentation(Func<T, int, int, string> stringConverter, Func<string, int, string> lineEndHandler = null)
        {
            StringBuilder result = new StringBuilder();
            for (int z = 0; z < ZDim; ++z)
            {
                for (int y = 0; y < YDim; ++y)
                {
                    StringBuilder line = new StringBuilder();
                    for (int x = 0; x < XDim; ++x)
                        line.Append(stringConverter(this[x, y], x, y));
                    if (lineEndHandler != null)
                        line.Append(lineEndHandler(line.ToString(), y));
                    result.AppendLine(line.ToString());
                }
                result.AppendLine();
            }
            return result.ToString().Remove(result.Length - Environment.NewLine.Length);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<DynamicGridValue<T>> GetEnumerator()
        {
            for (int z = 0; z < ZDim; ++z)
                for (int y = 0; y < YDim; ++y)
                    for (int x = 0; x < XDim; ++x)
                    {
                        var cellValue = new DynamicGridValue<T>(x, y, z, this[x, y, z]);
                        yield return cellValue;
                    }
        }
    }
}