using AdventOfCode.Tools.TopologicalOrder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day23
{
    [DebuggerDisplay("BoardState: {StateString}")]
    internal sealed class BoardState : IEquatable<string>, IEquatable<BoardState>
    {
        public string StateString { get; }
        public bool IsCompleted { get; }
        public HashSet<(BoardState state, long changeCost)> FurtherStates { get; } = new HashSet<(BoardState state, long changeCost)>();

        public long CheapestMoveCost { get; private set; } = -2;

        public BoardState(string stateString, bool isCompleted)
        {
            StateString = stateString;
            IsCompleted = isCompleted;
        }

        public bool Equals(string other) => StateString == other;

        public bool Equals(BoardState other) => Equals(other.StateString);

        public void StartCheapestMoveCalculation()
        {
            long Calculation()
            {
                if (FurtherStates.Any(x => x.state.CheapestMoveCost == -2)) throw new InvalidOperationException("Some dependencies have not been calculated!");
                if (!FurtherStates.Any()) return IsCompleted ? 0L : -1L;


                var pathCosts = FurtherStates.Where(x => x.state.CheapestMoveCost >= 0)
                .Select(x => x.state.CheapestMoveCost + x.changeCost);
                if (!pathCosts.Any()) return -1;
                return pathCosts.Min();
            }

            CheapestMoveCost = Calculation();
        }

        public bool CanCalculateCost() => FurtherStates.All(x => x.state.CheapestMoveCost > -2);
    }
}
