using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day21 : DayBase
    {
        public override string Title => "Dirac Dice";

        int player1Pos = 0;
        int player2Pos = 0;
        int player1Score = 0;
        int player2Score = 0;
        int lastDieRoll = 0;
        int dieRollCount = 0;

        const int targetScore = 1_000;

        public override string Solve(string input, bool part2)
        {
            if (part2) return Part2UnavailableMessage;
            var matches = Regex.Matches(input, @"Player (1|2) starting position: (\d+)");
            LoadPlayerPosition(matches[0]);
            LoadPlayerPosition(matches[1]);

            Console.Clear();
            Console.WriteLine(string.Join(" ", Enumerable.Range(1, 10)));
            bool player1 = false;
            while (player1Score < targetScore && player2Score < targetScore)
            {
                player1 = !player1;
                MakePlayerMove(player1);
                Console.SetCursorPosition(0, 3);
                Console.WriteLine($"Player 1: {player1Score}");
                Console.WriteLine($"Player 2: {player2Score}");
                Task.Delay(6).Wait();
            }

            Console.WriteLine($"Player {(player1 ? 1 : 2)} wins");
            int loosingPlayerScore = player1Score >= targetScore ? player2Score : player1Score;

            return $"Game Hash value = {loosingPlayerScore * dieRollCount}";
        }

        private void LoadPlayerPosition(Match matches)
        {
            int position = int.Parse(matches.Groups[2].Value);
            if (matches.Groups[1].Value == "1")
                player1Pos = position;
            else
                player2Pos = position;
        }

        private int GetNextDiceRoll()
        {
            var result = ++lastDieRoll;
            if (lastDieRoll == 100)
                lastDieRoll = 0;
            ++dieRollCount;
            return result;
        }

        private void MakePlayerMove(bool player1)
        {
            int position = player1 ? player1Pos : player2Pos;
            int cursorPosition = (position - 1) * 2 - 1;
            if (cursorPosition < 0)
                cursorPosition += 20;
            Console.CursorLeft = cursorPosition;
            for (int i = 0; i < 3; ++i)
                position += GetNextDiceRoll();
            while (position > 10)
                position -= 10;


            Console.CursorTop = player1 ? 1 : 2;
            while (Console.CursorLeft + 1 != position * 2)
            {
                if (Console.CursorLeft > 0)
                    --Console.CursorLeft;
                Console.Write("  ");
                if (Console.CursorLeft >= 20)
                    Console.CursorLeft = 0;
                Console.Write("X");
                Task.Delay(2).Wait();
            }

            if (player1)
            {
                player1Pos = position;
                player1Score += position;
            }
            else
            {
                player2Pos = position;
                player2Score += position;
            }
        }
    }
}
