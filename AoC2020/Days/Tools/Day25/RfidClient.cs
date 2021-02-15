using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day25
{
    class RfidClient
    {
        public long PublicKey { get; }
        public int LoopCount { get; private set; }

        public RfidClient(long recievedKey)
        {
            PublicKey = recievedKey;
            LoopCount = 0;
            long key = 1;
            while(key != PublicKey)
            {
                key = DoStep(key, 7);
                ++LoopCount;
            }
        }

        private long DoStep(long value, long subject)
        {
            value *= subject;
            value %= 20201227;
            return value;
        }

        public long GetEncriptionKey(long subject)
        {
            long encKey = 1;
            for (int i = 0; i < LoopCount; ++i)
                encKey = DoStep(encKey, subject);
            return encKey;
        }
    }
}
