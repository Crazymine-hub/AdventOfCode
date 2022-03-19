using AdventOfCode.Days.Tools.Day21;
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
        ulong lastDieRoll = 0;
        ulong dieRollCount = 0;
        ulong targetScore = 0;
        readonly ulong[] winCount = new ulong[2];

        private ulong universeCount = 0;

        public override string Solve(string input, bool part2)
        {
            this.part2 = part2;
            targetScore = part2 ? 21UL : 1000UL;
            var matches = Regex.Matches(input, @"Player (1|2) starting position: (\d+)");
            ulong position1 = ulong.Parse(matches[0].Groups[2].Value);
            ulong position2 = ulong.Parse(matches[1].Groups[2].Value);
            if (matches[0].Groups[1].Value == "2")
            {
                var positionBuffer = position2;
                position2 = position1;
                position1 = positionBuffer;
            }
            GameState state = new GameState(position1, position2);

            state = PlayGame(state);

            int winner = 0;
            if (part2)
            {
                winner = winCount[0] > winCount[1] ? 1 : 2;
                return $"Winner is Player {winner} in {winCount[winner - 1]} universes";
            }
            winner = state.GetWinner(targetScore);
            ulong gameHash = winner == 1 ? state.Player2Score : state.Player1Score;
            gameHash *= dieRollCount;
            return $"Game Hash is {gameHash} (P1: {state.Player1Score} P2: {state.Player2Score} D: {dieRollCount})";
        }

        private GameState PlayGame(GameState initialState, ulong depth = 0)
        {
            int winner = initialState.GetWinner(targetScore);
            if (winner != 0)
            {
                checked
                {
                    ++winCount[winner - 1];
                }
                return initialState;
            }

            GameState movedState = initialState;
            foreach (ulong diceValue in RollDice())
            {
                ++universeCount;
                movedState = initialState.ApplyMoves(diceValue);
                movedState = PlayGame(movedState, depth + 1);
            }
            return movedState;
        }

        private Queue<ulong> RollDice()
        {
            Queue<ulong> rolls = new Queue<ulong>();
            if (part2)
            {
                for (ulong i = 1; i <= 3; ++i)
                    for (ulong j = 1; j <= 3; ++j)
                        for (ulong k = 1; k <= 3; ++k)
                            rolls.Enqueue(i + j + k); //+3 = offset 1 each
                return rolls;
            }


            for (ulong i = 0; i < 3; ++i)
                rolls.Enqueue(GetNextDiceRoll());
            rolls.Enqueue(rolls.Aggregate((ulong accumulator, ulong next) => accumulator + next));
            while (rolls.Count > 1)
                rolls.Dequeue();
            return rolls;
        }

        private ulong GetNextDiceRoll()
        {
            var result = ++lastDieRoll;
            if (lastDieRoll == 100)
                lastDieRoll = 0;
            ++dieRollCount;
            return result;
        }
    }
}
