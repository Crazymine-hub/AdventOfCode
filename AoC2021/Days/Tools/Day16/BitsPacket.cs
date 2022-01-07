using AdventOfCode.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day16
{
    internal class BitsPacket
    {
        public byte Version { get; }
        public byte TypeId { get; }
        public ulong Value { get; }
        public List<BitsPacket> SubPackets { get; }

        public BitsPacket(byte version, byte typeId, ulong value, List<BitsPacket> subPackets)
        {
            Version = version;
            TypeId = typeId;
            Value = value;
            SubPackets = subPackets;
        }

        public static List<BitsPacket> ReadPackets(string message)
        {
            Queue<bool> binaryMessage = new Queue<bool>();
            int position = 0;
            while (position < message.Length) {
                byte value = byte.Parse(message[position++].ToString() , System.Globalization.NumberStyles.AllowHexSpecifier);
                for (int i = 3; i >= 0; i--)
                    binaryMessage.Enqueue(Bitwise.IsBitSet((int)value, i));
            }

            return ReadPackets(binaryMessage, true);
        }

        private static List<BitsPacket> ReadPackets(Queue<bool> message, bool continueReading)
        {
            List<BitsPacket> packets = new List<BitsPacket>();
            while (message.Count > 0)
            {
                int version = 0;
                for (int i = 2; i >= 0; --i)
                    version = Bitwise.SetBit(version, i, message.Dequeue());
                int typeId = 0;
                for (int i = 2; i >= 0; --i)
                    typeId = Bitwise.SetBit(typeId, i, message.Dequeue());

                ulong value = 0;
                List<BitsPacket> subPackets = null;

                switch (typeId)
                {
                    case 4:
                        value = ReadLiteral(message);
                        break;
                    default:
                        subPackets = ReadOperatorPackage(message);
                        break;
                }

                if(!continueReading) return packets;
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
            ulong value = 0;
            for(int i = 0; i < bits.Count; ++i)
                value = Bitwise.SetBit(value, i, bits[i]);
            return value;
        }
        private static List<BitsPacket> ReadOperatorPackage(Queue<bool> message)
        {
            throw new NotImplementedException();
        }
    }
}
