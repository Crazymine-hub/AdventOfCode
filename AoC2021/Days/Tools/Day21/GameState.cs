using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day21
{
    internal struct GameState
    {
        public ulong Player1Pos { get; }
        public ulong Player2Pos { get; }
        public ulong Player1Score { get; }
        public ulong Player2Score { get; }
        public bool Player1Turn { get; }

        public GameState(ulong p1Pos, ulong p2Pos, bool p1Turn = true, ulong p1Score = 0, ulong p2Score = 0)
        {
            Player1Pos = p1Pos;
            Player2Pos = p2Pos;
            Player1Score = p1Score;
            Player2Score = p2Score;
            Player1Turn = p1Turn;
        }

        public GameState ApplyMoves(ulong moves)
        {
            ulong position = Player1Turn ? Player1Pos : Player2Pos;
            position += moves;
            while (position > 10)
                position -= 10;

            if (Player1Turn)
                return new GameState(position, Player2Pos, false, Player1Score + position, Player2Score);
            else
                return new GameState(Player1Pos, position, true, Player1Score, Player2Score + position);
        }

        public int GetWinner(ulong targetScore)
        {
            if (Player1Score >= targetScore)
                return 1;
            if (Player2Score >= targetScore)
                return 2;
            return 0;
        }

        public override string ToString() => $"P1: {Player1Pos} @{Player1Score} P2: {Player2Pos} @{Player2Score} Turn: {(Player1Turn ? 1 : 2)}";
    }
}
