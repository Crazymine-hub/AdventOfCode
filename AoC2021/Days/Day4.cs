using AdventOfCode.Days.Tools.Day4;
using AdventOfCode.Tools.Visualization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    internal class Day4 : DayBase
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

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            List<string> game = GetGroupedLines(input);
            boards = new List<BingoBoard>();
            foreach (string boardDefinition in game.Skip(1))
                boards.Add(new BingoBoard(boardDefinition));
            int[] drawnNumbers = game[0].Split(',').Select(x => int.Parse(x)).ToArray();

            gridWidth = Convert.ToInt32(Math.Sqrt(boards.Count));
            gridHeight = boards.Count / gridWidth;

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

            PlayGame(drawnNumbers);
            return "";
        }

        private void DrawBoards()
        {
            for (int y = 0; y < gridHeight; ++y)
                for (int x = 0; x < gridWidth; ++x)
                    for (int gridY = 0; gridY < 5; ++gridY)
                        for (int gridX = 0; gridX < 5; ++gridX)
                        {
                            var block = new Rectangle(
                                x * scaling + BoardMargin + DividerStrength + gridX * (FieldSpacing + FieldSize),
                                y * scaling + BoardMargin + DividerStrength + gridY * (FieldSpacing + FieldSize),
                                FieldSize,
                                FieldSize);
                            gameBoard.FillRect(block, boards[y * gridWidth + x].GetFieldValue(gridY * 5 + gridX) < 0 ? Color.Green : Color.Red);
                        }
            VisualFormHandler.Instance.Update((Bitmap)gameBoard.Clone());
        }

        private BingoBoard PlayGame(int[] drawnNumbers)
        {
            BingoBoard winner = null;
            foreach (int number in drawnNumbers)
            {
                Console.WriteLine("Drawn: " + number);
                foreach (BingoBoard board in boards)
                {
                    board.PlayNumber(number);
                    DisplayBoard(board);
                    if (board.IsBoardComplete())
                    {
                        winner = board;
                        break;
                    }
                }
                DrawBoards();
                if (winner != null) break;
            }
            throw new Exception("No winning board found");
        }

        private void DisplayBoard(BingoBoard board)
        {
            for (int i = 0; i < 25; ++i)
            {
                int number = board.GetFieldValue(i);
                Console.Write(number < 0 ? 'X' : number);
                if (i > 0 && i % 5 == 0)
                    Console.WriteLine();
            }
        }
    }
}
