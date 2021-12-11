using AdventOfCode.Days.Tools.Day4;
using AdventOfCode.Tools.Extensions;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    internal class Day4 : DayBase, IDisposable
    {
        public override string Title => "GIant Squid";

        private const int FieldSize = 4;
        private const int FieldSpacing = 2;
        private const int FieldLineLength = 5;
        private const int BoardMargin = 2;
        private const int DividerStrength = 1;

        private List<BingoBoard> boards;
        private Bitmap gameBoard;
        private int gridWidth;
        private int gridHeight;
        private int scaling;
        private bool isDisposed;

        public override string Solve(string input, bool part2)
        {
            List<string> game = GetGroupedLines(input);
            boards = new List<BingoBoard>();
            foreach (string boardDefinition in game.Skip(1))
                boards.Add(new BingoBoard(boardDefinition));
            int[] drawnNumbers = game[0].Split(',').Select(x => int.Parse(x)).ToArray();

            gridWidth = Convert.ToInt32(Math.Sqrt(boards.Count));
            gridHeight = boards.Count / gridWidth;
            if (boards.Count % gridWidth > 0) ++gridHeight;

            scaling = FieldLineLength * FieldSize + (FieldLineLength - 1) * FieldSpacing + (2 * BoardMargin) + DividerStrength;


            gameBoard = new Bitmap(gridWidth * scaling + DividerStrength, gridHeight * scaling + DividerStrength);

            gameBoard.FillRect(new Rectangle(0, 0, gameBoard.Width, gameBoard.Height), Color.DarkSlateGray);

            for (int i = 0; i <= gridWidth; ++i)
            {
                var line = new Rectangle(scaling * i, 0, 1, gameBoard.Height);
                gameBoard.FillRect(line, Color.White);
            }

            for (int i = 0; i <= gridHeight; ++i)
            {
                var line = new Rectangle(0, scaling * i, gameBoard.Width, 1);
                gameBoard.FillRect(line, Color.White);
            }

            VisualFormHandler.Instance.Show((Bitmap)gameBoard.Clone());
            DrawBoards();

            BingoBoard winner = PlayGame(drawnNumbers, part2, out int drawnNumber);
            DrawBoards();

            int score = winner.GetUnmarkedFieldSum() * drawnNumber;

            return "Winner board score is " + score;
        }

        private void DrawBoards()
        {
            for (int y = 0; y < gridHeight; ++y)
                for (int x = 0; x < gridWidth; ++x)
                {
                    if (y * gridWidth + x >= boards.Count) break;
                    BingoBoard board = boards[y * gridWidth + x];
                    bool boardWinning = board.IsBoardComplete();
                    for (int gridY = 0; gridY < 5; ++gridY)
                        for (int gridX = 0; gridX < 5; ++gridX)
                        {
                            var block = new Rectangle(
                                x * scaling + BoardMargin + DividerStrength + gridX * (FieldSpacing + FieldSize),
                                y * scaling + BoardMargin + DividerStrength + gridY * (FieldSpacing + FieldSize),
                                FieldSize,
                                FieldSize);
                            int boardFieldValue = board.GetFieldValue(gridY * 5 + gridX);
                            gameBoard.FillRect(block, boardFieldValue < 0 ? (boardWinning ? Color.Green : Color.Yellow) : Color.Red);
                        }
                }
            VisualFormHandler.Instance.Update((Bitmap)gameBoard.Clone());
        }

        private BingoBoard PlayGame(int[] drawnNumbers, bool getLast, out int lastDrawn)
        {
            BingoBoard lastWinner = null;
            lastDrawn = -1;
            foreach (int number in drawnNumbers)
            {
                lastDrawn = number;
                Console.WriteLine("Drawn: " + number);
                foreach (BingoBoard board in boards)
                {
                    if (board.IsBoardComplete())
                    {
                        Console.WriteLine("Board already won. Skipping");
                        continue;
                    }
                    board.PlayNumber(number);
                    DisplayBoard(board);
                    if (board.IsBoardComplete())
                    {
                        if (!getLast) return board;
                        lastWinner = board;
                    }
                }
                DrawBoards();
                if (getLast && boards.All(x => x.IsBoardComplete())) break;
            }
            if(getLast) return lastWinner;
            throw new Exception("No winning board found");
        }

        private void DisplayBoard(BingoBoard board)
        {
            for (int i = 0; i < 25; ++i)
            {
                int number = board.GetFieldValue(i);
                Console.Write((number < 0 ? "X" : number.ToString()).PadRight(3));
                if (i > 0 && i % 5 == 4)
                    Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    gameBoard.Dispose();
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                isDisposed = true;
            }
        }

        // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // ~Day4()
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
