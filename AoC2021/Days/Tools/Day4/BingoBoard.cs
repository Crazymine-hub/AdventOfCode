using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day4
{
    internal class BingoBoard
    {
        private int[] board;

        public BingoBoard(string boardLayout)
        {
            board = new int[25];
            string[] numbers = boardLayout.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < numbers.Length; ++i)
                board[i] = int.Parse(numbers[i]);
        }

        public void PlayNumber(int number)
        {
            for (int i = 0; i < board.Length; ++i)
                if (board[i] == number)
                {
                    board[i] = -1;
                    return;
                }
        }

        public bool IsBoardComplete()
        {
            bool[] columnWin = new bool[5].Select(x => true).ToArray();
            bool rowWin = false;
            for (int i = 0; i < board.Length; ++i)
            {
                int colIndex = i % 5;
                columnWin[colIndex] &= board[i] < 0;
                rowWin &= board[i] < 0;
                if(colIndex == 4)
                {
                    if(rowWin) return true;
                    rowWin = false;
                }
            }
            return columnWin.Any(x => x);
        }

        public bool IsFieldFilled(int index)
        {
            return board[index] < 0;
        }
    }
}
