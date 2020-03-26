using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib
{
    public struct FlagField
    {
        private uint bits;
        public uint Bits
        {
            get => bits;
            set => bits = value;
        }

        const int byteSize = 8;
        const uint byteMask = 0xFFU;

        public FlagField(uint bits)
        {
            this.bits = bits;
        }

        public bool this[int index]
        {
            get
            {
                ErrorIfNotInRange(index, 0, 31);
                var mask = 1U << index;
                return (bits & mask) == mask;
            }
            set
            {
                ErrorIfNotInRange(index, 0, 31);
                var mask = 1U << index;
                if (value)
                {
                    bits |= mask;
                }
                else
                {
                    bits &= ~mask;
                }
            }
        }

        public byte GetByte(int index)
        {
            ErrorIfNotInRange(index, 0, 3);
            var mask = byteMask << index * byteSize;
            return (byte)((bits & mask) >> index * byteSize);
        }

        public void SetByte(int index, byte value)
        {
            ErrorIfNotInRange(index, 0, 3);
            var mask = byteMask << index * byteSize;
            bits &= ~mask; // clear
            bits |= (uint)value << index * byteSize; // set
        }

        public bool[] ToBoolArray()
        {
            var arr = new bool[32];
            var mask = 1U;
            for(int i = 0; i < 32; i++)
            {
                arr[i] = (bits & mask) == mask;
                mask <<= 1;
            }
            return arr;
        }

        public uint GetBitString(int start, int length)
        {
            if (length == 0) return 0;
            ErrorIfNotInRange(start, 0, 31);
            ErrorIfNotInRange(length, 0, 31);

            if ((start + length) > 32)
                throw new IndexOutOfRangeException();

            var mask = (uint)((1UL << length) - 1) << start;
            return (bits & mask) >> start;
        }

        public void SetBitString(int start, int length, uint value)
        {
            if (length == 0) return;
            ErrorIfNotInRange(start, 0, 31);
            ErrorIfNotInRange(length, 0, 31);

            if ((start + length) > 32)
                throw new IndexOutOfRangeException();

            var mask = (uint)((1UL << length) - 1);

            // trim value first
            value &= mask;

            bits &= ~(mask << start); // clear
            bits |= value << start;
        }

        public override string ToString()
        {
            return Convert.ToString(bits, 2).PadLeft(32, '0');
        }

        public override int GetHashCode()
        {
            return bits.GetHashCode();
        }

        private void ErrorIfNotInRange(int i, int min, int max)
        {
            if (i > max || i < min)
                throw new IndexOutOfRangeException();
        }
    }
}
