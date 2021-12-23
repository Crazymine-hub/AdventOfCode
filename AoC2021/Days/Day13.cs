﻿using AdventOfCode.Tools.DynamicGrid;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    internal class Day13 : DayBase, IDisposable
    {
        public override string Title => "Transparent Origami";
        private readonly DynamicGrid<bool> paper = new DynamicGrid<bool>();
        private bool disposedValue;

        Bitmap paperPunches;

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            List<string> instructions = GetGroupedLines(input);
            foreach (string dot in GetLines(instructions[0]))
            {
                string[] coords = dot.Split(',');
                paper.SetRelative(int.Parse(coords[0]), int.Parse(coords[1]), true);
            }
            paperPunches = new Bitmap(paper.XDim, paper.YDim);
            VisualFormHandler.Instance.Show(paperPunches);
            
            foreach (string foldInstruction in GetLines(instructions[1]))
            {
                Fold(foldInstruction);
                if (!part2) break;
            }
            return "Holes: " + paper.Count(x => x);
        }

        private void Fold(string foldInstruction)
        {
            Match instruction = Regex.Match(foldInstruction, @"^fold along (x|y)=(\d+)$");
            if (!instruction.Success) throw new ArgumentException(nameof(foldInstruction));
            bool isHorizontal = instruction.Groups[1].Value == "y";
            int value = int.Parse(instruction.Groups[2].Value);
            for (int i = 0; i < value; ++i)
            {
                for (int j = 0; j < (isHorizontal ? paper.XDim : paper.YDim); ++j)
                {
                    (int x, int y, int oppositeX, int oppositeY) = isHorizontal ? (j, i, j, paper.YDim - i - 1) : (i, j, paper.XDim - i - 1, j);
                    paper.SetRelative(x, y, paper.GetRelative(x, y) || paper.GetRelative(oppositeX, oppositeY));
                }
            }

            while ((isHorizontal ? paper.YDim : paper.XDim) > value)
                if (isHorizontal)
                    paper.DecreaseY(false);
                else
                    paper.DecreaseX(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    paperPunches.Dispose();
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                disposedValue = true;
            }
        }

        // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // ~Day13()
        // {
        //     // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
