using AdventOfCode.Days.Tools.Day16;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day16 : DayBase
    {
        public override string Title => "Packet Decoder";

        public override string Solve(string input, bool part2)
        {
            List<BitsPacket> packets = BitsPacket.ReadPackets(input, out List<BitsPacket> allPackets);


            int versionSum = allPackets
                .Select(x => x.Version)
                .Aggregate((int accumulated, int next) => accumulated + next);
            return $"Read {allPackets.Count} packet(s).{Environment.NewLine}" +
                $"Sum of all version numbers: {versionSum}{Environment.NewLine}" +
                $"Transmission Value: {packets.First().Value}";
        }
    }
}
