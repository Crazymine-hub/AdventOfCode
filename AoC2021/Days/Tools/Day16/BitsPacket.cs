using AdventOfCode.Tools;
using AdventOfCode.Tools.SpecificBitwise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day16
{
    internal class BitsPacket
    {
        public int Version { get; }
        public int TypeId { get; }
        public ulong Value { get; }
        public List<BitsPacket> SubPackets { get; }

        public BitsPacket(int version, int typeId, ulong value, List<BitsPacket> subPackets)
        {
            Version = version;
            TypeId = typeId;
            Value = value;
            SubPackets = subPackets;
        }

        public static List<BitsPacket> ReadPackets(string message, out List<BitsPacket> linearList)
        {
            Queue<bool> binaryMessage = new Queue<bool>();
            linearList = new List<BitsPacket>();
            int position = 0;
            while (position < message.Length)
            {
                byte value = byte.Parse(message[position++].ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                for (int i = 3; i >= 0; i--)
                    binaryMessage.Enqueue(IntBitwise.IsBitSet(value, i));
            }

            return ReadPackets(binaryMessage, true, linearList);
        }

        private static List<BitsPacket> ReadPackets(Queue<bool> message, bool continueReading, List<BitsPacket> linearPackets)
        {
            List<BitsPacket> packets = new List<BitsPacket>();
            while (message.Count > 0 && !message.All(x => x == false))
            {
                int version = 0;
                for (int i = 2; i >= 0; --i)
                    version = IntBitwise.SetBit(version, i, message.Dequeue());
                int typeId = 0;
                for (int i = 2; i >= 0; --i)
                    typeId = IntBitwise.SetBit(typeId, i, message.Dequeue());

                ulong value = 0;
                List<BitsPacket> subPackets = null;

                switch (typeId)
                {
                    case 4:
                        value = ReadLiteral(message);
                        break;
                    default:
                        subPackets = ReadOperatorPackage(message, linearPackets);
                        break;
                }

                BitsPacket packet = new BitsPacket(version, typeId, value, subPackets);
                packets.Add(packet);
                linearPackets.Add(packet);

                if (!continueReading) return packets;
            }
            return packets;
        }


        private static ulong ReadLiteral(Queue<bool> message)
        {
            bool hasMoreSegments = false;
            List<bool> bits = new List<bool>();
            do
            {
                hasMoreSegments = message.Dequeue();
                for (int i = 0; i < 4; ++i)
                    bits.Add(message.Dequeue());
            }
            while (hasMoreSegments);
            bits.Reverse();
            return ULongBitwise.GetValue(bits);
        }
        private static List<BitsPacket> ReadOperatorPackage(Queue<bool> message, List<BitsPacket> linearPackets)
        {
            bool hasPacketNumber = message.Dequeue();
            List<bool> bits = new List<bool>();
            for (int i = 0; i < (hasPacketNumber ? 11 : 15); ++i)
                bits.Add(message.Dequeue());
            bits.Reverse();
            int length = IntBitwise.GetValue(bits);
            List<BitsPacket> subPackets = new List<BitsPacket>();
            if (hasPacketNumber)
            {
                for (int i = 0; i < length; ++i)
                    subPackets.AddRange(ReadPackets(message, false, linearPackets));
            }
            else
            {
                Queue<bool> packetContent = new Queue<bool>();
                for (int i = 0; i < length; ++i)
                    packetContent.Enqueue(message.Dequeue());
                subPackets = ReadPackets(packetContent, true, linearPackets);
            }
            return subPackets;
        }
    }
}
