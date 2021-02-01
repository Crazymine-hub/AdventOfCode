using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day11 : DayBase
    {
        public override string Title => "Seating System";

        private bool part2;
        private byte[][] seats;
        public override string Solve(string input, bool part2)
        {
            //Read the Seat Layout
            //0=floor 1=seat 2=occupied seat
            this.part2 = part2;
            List<byte[]> seatList = new List<byte[]>();
            foreach (string row in GetLines(input))
            {
                List<byte> rowSeats = new List<byte>();
                foreach (char seat in row)
                {
                    switch (seat)
                    {
                        case 'L':
                            rowSeats.Add(1);
                            break;
                        case '#':
                            rowSeats.Add(2);
                            break;
                        default:
                            rowSeats.Add(0);
                            break;
                    }
                }
                seatList.Add(rowSeats.ToArray());
            }
            seats = seatList.ToArray();
            return "Occupied: " + GameOfLife();
        }

        private int GameOfLife()
        {
            bool wasChanged = false;
            int occupiedCount = 0;
            //regenerate seat usage until it doesn't change
            do
            {
                Console.Clear();
                wasChanged = false;
                occupiedCount = 0;
                List<byte[]> newSeats = new List<byte[]>();
                //iterate over the layout
                for (int row = 0; row < seats.Length; ++row)
                {
                    List<byte> seatRow = new List<byte>();
                    for (int seat = 0; seat < seats[row].Length; ++seat)
                    {
                        if (!IsValidSeat(row, seat))
                        {//Invalid seat in this range must be floor
                            seatRow.Add(0);
                            Console.Write(" ");
                            continue;
                        }
                        //determine current seat status and occupied neighbours
                        bool isOccupied = IsSeatOccupied(row, seat);
                        int neighbours = CountOccupiedAdjacent(row, seat);
                        bool newOccupied = isOccupied;

                        //Determine if the seat state changes and set this state.
                        if (!isOccupied && neighbours == 0) newOccupied = true;
                        if (isOccupied && neighbours >= (part2 ? 5 : 4)) newOccupied = false;
                        //check if the state has changed and another iteration is needed
                        if (wasChanged == false && isOccupied != newOccupied) wasChanged = true;
                        Console.Write(newOccupied ? "█" : "░");
                        if (newOccupied) occupiedCount++;
                        seatRow.Add((byte)(newOccupied ? 2 : 1));
                    }
                    newSeats.Add(seatRow.ToArray());
                    Console.WriteLine();
                }
                seats = newSeats.ToArray();
                System.Threading.Thread.Sleep(100);
            } while (wasChanged);
            return occupiedCount;
        }

        private bool IsValidSeat(int row, int seat)
        {
            return IsValidSeat(row, seat, out bool inRange);
        }
        private bool IsValidSeat(int row, int seat, out bool inRange)
        {//Check whether the given positions are in array range and the selected seat exists (isn't floor)
            inRange = row >= 0 && row < seats.Length && seat >= 0 && seat < seats[row].Length;
            return inRange && seats[row][seat] > 0;
        }

        private bool IsSeatOccupied(int row, int seat)
        {//check whether the seat is occupied, invalid seats count as free
            if (!IsValidSeat(row, seat)) return false;
            return seats[row][seat] == 2;
        }

        private int CountOccupiedAdjacent(int row, int seat)
        {
            int[] occupied = new int[9];
            //0=undetermined 1=free 2=occupied
            int outOfRange = 0;
            int distance = 1;
            //go forther away from the seat as needed
            do
            {
                outOfRange = 0;
                for (int rowOff = -1; rowOff <= 1; ++rowOff)
                    for (int seatOff = -1; seatOff <= 1; ++seatOff)
                    {
                        //keep track of the state of the closest seat in a direction
                        int seatIndex = (rowOff + 1) * 3 + seatOff + 1;
                        //directions that already have a seat and the seat itself don't need to be counted
                        if (rowOff == 0 && seatOff == 0 || occupied[seatIndex] != 0)
                        {
                            //count these as out of range to prevent overflowing
                            outOfRange++;
                            continue;
                        }
                        //valid seats can be set in this direction.
                        if (IsValidSeat(row + (rowOff * distance), seat + (seatOff * distance), out bool inRange))
                        { 
                            occupied[seatIndex] = 1;
                            if (IsSeatOccupied(row + (rowOff * distance), seat + (seatOff * distance))) occupied[seatIndex] = 2;
                        }
                        //we are out of range
                        if (!inRange) outOfRange++;
                    }
                distance++;
                //stop moving away if all fields are out of range or all seats have been found (only move in part 2)
            } while (part2 && outOfRange < 9 && occupied.Where(x => x == 0).Count() > 1);

            return occupied.Where(x => x == 2).Count();
        }
    }
}
