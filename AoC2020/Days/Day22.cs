using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day22 : DayBase
    {
        public override string Title => "Crab Combat";

        private int gameCnt = 0;
        private bool recursion = false;

        public override string Solve(string input, bool part2)
        {
            recursion = part2;
            List<string> players = GetGroupedLines(input);


            Queue<int> player1 = new Queue<int>();
            Queue<int> player2 = new Queue<int>();
            LoadPlayer(player1, GetLines(players[0]));
            LoadPlayer(player2, GetLines(players[1]));

            var winner = DoGame(ref player1, ref player2);

            return $"The Winner is Player {winner.Item1} with a score of {winner.Item2}.";
        }

        private void LoadPlayer(Queue<int> player, List<string> playerDesc)
        {
            for (int i = 1; i < playerDesc.Count; ++i)
            {
                int card = int.Parse(playerDesc[i]);
                player.Enqueue(card);
            }
        }


        private (int, long) DoGame(ref Queue<int> play1, ref Queue<int> play2)
        {
            int gameNr = ++gameCnt;
            HashSet<string> p1Cards = new HashSet<string>();
            HashSet<string> p2Cards = new HashSet<string>();
            while (play1.Any() && play2.Any())
            {
                string deck1 = MakePlayerString(play1);
                string deck2 = MakePlayerString(play2);
                if (p1Cards.Contains(deck1) || p2Cards.Contains(deck2))
                {
                    return (1, 0);
                }
                p1Cards.Add(deck1);
                p2Cards.Add(deck2);
                DoTurn(play1, play2, gameNr);
            }
            if (play1.Count > 0)
            {
                return (1, GetScore(play1));
            }
            else
            {
                return (2, GetScore(play2));
            }
        }

        private void DoTurn(Queue<int> play1, Queue<int> play2, int gameNr)
        {
            int p1 = play1.Dequeue();
            int p2 = play2.Dequeue();

            bool recursed = false;
            bool player1Won = false;
            if (recursion && play1.Count >= p1 && play2.Count >= p2)
            {
                recursed = true;
                var clone1 = ClonePlayer(play1, p1);
                var clone2 = ClonePlayer(play2, p2);
                player1Won = DoGame(ref clone1, ref clone2).Item1 == 1;
            }

            if ((!recursed && p1 > p2) || (recursed && player1Won))
            {
                play1.Enqueue(p1);
                play1.Enqueue(p2);
            }
            else
            {
                play2.Enqueue(p2);
                play2.Enqueue(p1);
            }
        }

        private long GetScore(Queue<int> player)
        {
            long score = 0;
            while (player.Count > 0)
            {
                int points = player.Count;
                points *= player.Dequeue();
                score += points;
            }
            return score;
        }

        private string MakePlayerString(Queue<int> player)
        {
            string result = "";
            foreach (int card in player)
                result += card.ToString() + ", ";
            if(result.Length >= 2)
            result = result.Remove(result.Length - 2);
            return result;
        }

        private Queue<int> ClonePlayer(Queue<int> player, int amount)
        {
            Queue<int> newPlay = new Queue<int>();
            for (int i = 0; i < amount; ++i)
                newPlay.Enqueue(player.ElementAt(i));
            return newPlay;
        }
    }
}
