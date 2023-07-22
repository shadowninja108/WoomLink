using System;

namespace WoomLink.sead
{
    public struct SeadRandom
    {
        private uint State0;
        private uint State1;
        private uint State2;
        private uint State3;

        public void Init()
        {
            Init((uint)Environment.TickCount);
        }

        public void Init(uint seed)
        {
            State0 = 1812433253 * (seed ^ (seed >> 30)) + 1;
            State1 = 1812433253 * (State0 ^ (State0 >> 30)) + 2;
            State2 = 1812433253 * (State1 ^ (State1 >> 30)) + 3;
            State3 = 1812433253 * (State2 ^ (State2 >> 30)) + 4;
        }

        public void Init(uint seedOne, uint seedTwo, uint seedThree, uint seedFour)
        {
            State0 = seedOne; 
            State1 = seedTwo; 
            State2 = seedThree; 
            State3 = seedFour;
        }

        public uint GetUInt32()
        {
            uint v1;
            uint v2;
            uint v3;

            v1 = State0 ^ (State0 << 11);
            State0 = State1;
            v2 = State3;
            v3 = v1 ^ (v1 >> 8) ^ v2 ^ (v2 >> 19);
            State1 = State2;
            State2 = v2;
            State3 = v3;

            return v3;
        }

        public float GetSingle()
        {
            var one = BitConverter.SingleToUInt32Bits(1);

            var bits = (GetUInt32() >> 9) | one;
            var f = BitConverter.UInt32BitsToSingle(bits) + -1;

            return f + -1;
        }
    }
}
