using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day5 : DayBase
    {
        public override string Title => "Binary Boarding";
        const int rowCount = 128;
        const int colCount = 8;
        Dictionary<string, int> boardSeat = new Dictionary<string, int>();

        public override string Solve(string input, bool part2)
        {
            //Make Sure, our Console is wide enough
            if (Console.BufferWidth < rowCount)
                Console.BufferWidth = rowCount;
            foreach (string pass in GetLines(input))
            {//Generate the SeatID and print the position to the console
                int seatID = GetSeatID(pass);
                boardSeat.Add(pass, seatID);
                Console.SetCursorPosition(seatID >> 3, seatID & 7);
                //Leave a horizontal gap as a path
                if (Console.CursorTop > 3)
                    Console.CursorTop++;
                Console.Write("#");
                System.Threading.Thread.Sleep(50);
            }

            Console.SetCursorPosition(0, 9);
            if (part2)
            {
                for (int i = 1; i <= boardSeat.Count; i++)
                {//Find the first unboarded seat. Thats mine
                    int currID = boardSeat.ElementAt(i).Value;
                    if (!boardSeat.Values.Contains(currID - 1))
                        return "Your Seat ID is: " + (currID - 1);
                }
                return "ERROR";
            }

            return "Max Seat ID: " + boardSeat.Max(x => x.Value);
        }

        private int GetSeatID(string pass)
        {
            int lowRow = 0;
            int upRow = rowCount;
            int lowCol = 0;
            int upCol = colCount;
            pass = pass.ToUpper();

            for (int i = 0; i <= 9; i++)
            {
                //calculate the adjustment for this step
                int range = i <= 6 ? upRow - lowRow : upCol - lowCol;
                range /= 2;

                //Apply adjustment to right range value, according to letter. Check position.
                switch (pass[i])
                {
                    case 'F':
                        if (i <= 6)
                            upRow -= range;
                        else throw new ArgumentException("Direction at wrong position");
                        break;
                    case 'B':
                        if (i <= 6)
                            lowRow += range;
                        else throw new ArgumentException("Direction at wrong position");
                        break;
                    case 'L':
                        if (i > 6)
                            upCol -= range;
                        else throw new ArgumentException("Direction at wrong position");
                        break;
                    case 'R':
                        if (i > 6)
                            lowCol += range;
                        else throw new ArgumentException("Direction at wrong position");
                        break;
                    default: throw new ArgumentException("Invalid Direction: " + pass[i]);
                }
            }
            //We use 0-Index, hence decrease the value by one
            upRow--;
            upCol--;
            //generate the seat ID;
            return upRow * 8 + upCol;
        }
    }
}
