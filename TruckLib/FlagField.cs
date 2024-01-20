using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib
{
    /// <summary>
    /// Represents a 32-bit flag field.
    /// </summary>
    public struct FlagField
    {
        private uint bits;
        /// <summary>
        /// Gets or sets the underlying value.
        /// </summary>
        public uint Bits
        {
            get => bits;
            set => bits = value;
        }

        private const int byteSize = 8;
        private const uint byteMask = 0xFFU;

        /// <summary>
        /// Initializes a new flag field with the given value.
        /// </summary>
        /// <param name="bits">The initial value of the flag field.</param>
        public FlagField(uint bits)
        {
            this.bits = bits;
        }

        /// <summary>
        /// Gets or sets one flag of the flag field as Boolean.
        /// </summary>
        /// <param name="index">The index of the flag, where 0 is the LSB.</param>
        public bool this[int index]
        {
            get
            {
                AssertInRange(index, 0, 31);
                var mask = 1U << index;
                return (bits & mask) == mask;
            }
            set
            {
                AssertInRange(index, 0, 31);
                var mask = 1U << index;
                if (value)
                    bits |= mask;
                else
                    bits &= ~mask;
            }
        }

        /// <summary>
        /// Returns a byte of the flag field.
        /// </summary>
        /// <param name="index">The index of the byte, where 0 is the LSB.</param>
        public byte GetByte(int index)
        {
            AssertInRange(index, 0, 3);
            var mask = byteMask << index * byteSize;
            return (byte)((bits & mask) >> index * byteSize);
        }

        /// <summary>
        /// Sets a byte of the flag field.
        /// </summary>
        /// <param name="index">The index of the byte, where 0 is the LSB.</param>
        /// <param name="value">The value to set.</param>
        public void SetByte(int index, byte value)
        {
            AssertInRange(index, 0, 3);
            var mask = byteMask << index * byteSize;
            bits &= ~mask; // clear
            bits |= (uint)value << index * byteSize; // set
        }

        /// <summary>
        /// Converts the flag field to a bool array.
        /// </summary>
        /// <returns>The flag field as bool array.</returns>
        public bool[] ToBoolArray()
        {
            var arr = new bool[32];
            var mask = 1U;
            for (int i = 0; i < 32; i++)
            {
                arr[i] = (bits & mask) == mask;
                mask <<= 1;
            }
            return arr;
        }

        /// <summary>
        /// Returns a sub-bitstring of the flag field.
        /// </summary>
        /// <param name="start">The index of the first bit, where 0 is the LSB.</param>
        /// <param name="length">The length in bits.</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public uint GetBitString(int start, int length)
        {
            if (length == 0)
                return 0;

            AssertInRange(start, 0, 31);
            AssertInRange(length, 0, 31);

            if ((start + length) > 32)
                throw new IndexOutOfRangeException();

            var mask = (uint)((1UL << length) - 1) << start;
            return (bits & mask) >> start;
        }

        /// <summary>
        /// Sets a sub-bitstring of the flag field.
        /// </summary>
        /// <param name="start">The index of the first bit, where 0 is the LSB.</param>
        /// <param name="length">The length in bits.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void SetBitString(int start, int length, uint value)
        {
            if (length == 0)
                return;

            AssertInRange(start, 0, 31);
            AssertInRange(length, 0, 31);

            if ((start + length) > 32)
                throw new IndexOutOfRangeException();

            var mask = (uint)((1UL << length) - 1);

            // trim value first
            value &= mask;

            bits &= ~(mask << start); // clear
            bits |= value << start;
        }

        /// <summary>
        /// Returns the content of the flag field as a binary string.
        /// </summary>
        /// <returns>The content of the flag field as a binary string.</returns>
        public override string ToString() =>
            Convert.ToString(bits, 2).PadLeft(32, '0');

        /// <inheritdoc/>
        public override int GetHashCode() =>
            bits.GetHashCode();

        private void AssertInRange(int i, int min, int max)
        {
            if (i > max || i < min)
                throw new IndexOutOfRangeException();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            obj is FlagField flagField && flagField.Bits == this.Bits;

        public static bool operator ==(FlagField left, FlagField right) => 
            left.Equals(right);

        public static bool operator !=(FlagField left, FlagField right) => 
            !(left == right);
    }
}
