using AdventOfCode.Tools;
using System;
using System.Collections.Concurrent;
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

        bool part2;
        long lastDieRoll = 0;
        long dieRollCount = 0;
        long targetScore = 0;

        long p1Score;
        long p2Score;

        private long universeCount = 0;
        private readonly (long value, long amount)[] diceUniverses = new (long, long)[]{
            (3, 1),
            (4, 3),
            (5, 6),
            (6, 7),
            (7, 6),
            (8, 3),
            (9, 1)
            };

        public override string Solve(string input, bool part2)
        {
            this.part2 = part2;
            targetScore = part2 ? 21L : 1000L;
            var matches = Regex.Matches(input, @"Player (1|2) starting position: (\d+)");
            long p1Pos = long.Parse(matches[0].Groups[2].Value) - 1;
            long p2Pos = long.Parse(matches[1].Groups[2].Value) - 1;
            if (matches[0].Groups[1].Value == "2")
            {
                var positionBuffer = p2Pos;
                p2Pos = p1Pos;
                p1Pos = positionBuffer;
            }

            PlayGame(p1Pos, p1Score, p2Pos, p2Score, out long p1Wins, out long p2Wins, true);


            if (part2)
            {
                //(long wins1, long wins2) = PlayCopy(p1Pos, targetScore, p2Pos, targetScore);
                //return $"Winner is in {Math.Max(wins1, wins2)}";
                long winCount = p1Wins > p2Wins ? p1Wins : p2Wins;
                return $"Winner is Player {(p1Wins == winCount ? 1 : 2)} in {winCount} universes";
            }
            long gameHash = p1Score > p2Score ? p2Score : p1Score;
            gameHash *= dieRollCount;
            return $"Game Hash is {gameHash} (P1: {p1Score} P2: {p2Score} D: {dieRollCount})";
        }

        private void PlayGame(long p1Pos, long p1Score, long p2Pos, long p2Score, out long p1Wins, out long p2Wins, bool p1Turn)
        {
            p1Wins = 0;
            p2Wins = 0;
            this.p1Score = p1Score;
            this.p2Score = p2Score;
            if (p1Score >= targetScore)
            {
                ++p1Wins;
                return;
            }

            if (p2Score >= targetScore)
            {
                ++p2Wins;
                return;
            }

            long nP1Pos = p1Pos;
            long nP1Score = p1Score;
            long nP2Pos = p2Pos;
            long nP2Score = p2Score;

            foreach (var diceThrow in RollDice())
            {
                if (p1Turn)
                {
                    nP1Pos = MakeMoves(p1Pos, diceThrow.value);
                    nP1Score = p1Score + nP1Pos + 1;
                }
                else
                {
                    nP2Pos = MakeMoves(p2Pos, diceThrow.value);
                    nP2Score = p2Score + nP2Pos + 1;
                }
                PlayGame(nP1Pos, nP1Score, nP2Pos, nP2Score, out long uP1Wins, out long uP2Wins, !p1Turn);
                p1Wins += uP1Wins * diceThrow.amount;
                p2Wins += uP2Wins * diceThrow.amount;
            }
        }

        long MakeMoves(long currentPosition, long moves) => (currentPosition + moves) % 10;

        private IEnumerable<(long value, long amount)> RollDice()
        {
            if (part2)
            {
                foreach (var universe in diceUniverses)
                    yield return universe;
                yield break;
            }

            long accumulator = 0;
            for (long i = 0; i < 3; ++i)
                accumulator += GetNextDiceRoll();
            yield return (accumulator, 1);
        }

        private long GetNextDiceRoll()
        {
            var result = ++lastDieRoll;
            if (lastDieRoll == 100)
                lastDieRoll = 0;
            ++dieRollCount;
            return result;
        }
    }
}
