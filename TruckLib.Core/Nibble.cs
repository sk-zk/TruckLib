using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib
{
    /// <summary>
    /// Represents an unsigned half-byte integer.
    /// </summary>
    public struct Nibble
    {
        /// <summary>
        /// Represents the smallest possible value of a nibble.
        /// </summary>
        public const byte MinValue = 0;
        /// <summary>
        /// Represents the largest possible value of a nibble.
        /// </summary>
        public const byte MaxValue = 15;

        private byte value;
        /// <summary>
        /// Gets or sets the underlying value.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private byte Value
        {
            get => value;
            set
            {
                this.value = SetIfInRange(value, MinValue, MaxValue);
            }
        }

        /// <summary>
        /// Instantiates a nibble.
        /// </summary>
        /// <param name="value">The initial value.</param>
        public Nibble(byte value)
        {
            this.value = 0;
            Value = value;
        }

        private static Nibble Add(int a, int b)
        {
            var result = (byte)(a + b);
            result &= 0x0F; // simulate overflow
            return (Nibble)result;
        }

        public static Nibble operator +(Nibble a, int b) => Add(a.Value, b);
        public static Nibble operator +(int a, Nibble b) => Add(a, b.Value);
        public static Nibble operator +(Nibble a, Nibble b) => Add(a.Value, b.Value);
        public static Nibble operator ++(Nibble a) => Add(a.Value, 1);

        private static Nibble Subtract(int a, int b)
        {
            var result = (byte)(a - b);
            result &= 0x0F; // simulate overflow
            return (Nibble)result;
        }

        public static Nibble operator -(Nibble a, Nibble b) => Subtract(a.Value, b.Value);
        public static Nibble operator -(Nibble a, int b) => Subtract(a.Value, b);
        public static Nibble operator -(int a, Nibble b) => Subtract(a, b.Value);
        public static Nibble operator --(Nibble a) => Subtract(a.Value, 1);

        public static bool operator >(Nibble a, Nibble b) => a.Value > b.Value;
        public static bool operator >(Nibble a, int b) => a.Value > b;
        public static bool operator >(int a, Nibble b) => a > b.Value;

        public static bool operator <(Nibble a, Nibble b) => a.Value < b.Value;
        public static bool operator <(Nibble a, int b) => a.Value < b;
        public static bool operator <(int a, Nibble b) => a < b.Value;

        public static bool operator ==(Nibble a, Nibble b) => a.Value == b.Value;
        public static bool operator ==(Nibble a, int b) => a.Value == b;
        public static bool operator ==(int a, Nibble b) => a == b.Value;

        public static bool operator !=(Nibble a, Nibble b) => a.Value != b.Value;
        public static bool operator !=(Nibble a, int b) => a.Value != b;
        public static bool operator !=(int a, Nibble b) => a != b.Value;

        public static explicit operator Nibble(byte b) => new(b);
        public static explicit operator Nibble(short i) => new((byte)i);
        public static explicit operator Nibble(ushort i) => new((byte)i);
        public static explicit operator Nibble(int i) => new((byte)i);
        public static explicit operator Nibble(uint i) => new((byte)i);
        public static explicit operator byte(Nibble n) => n.Value;

        /// <inheritdoc/>
        public override bool Equals(object obj) => base.Equals(obj);

        /// <inheritdoc/>
        public override int GetHashCode() => value.GetHashCode();

        /// <summary>
        /// Converts the nibble to string.
        /// </summary>
        /// <returns>The nibble as string.</returns>
        public override string ToString() => value.ToString();

        /// <summary>
        /// Checks if a number is within the specified range. 
        /// Returns the number if it is; throws ArgumentException if it isn't.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <param name="min">The lower limit.</param>
        /// <param name="max">The upper limit.</param>
        /// <returns>The number, if it is within range.</returns>
        /// <exception cref="ArgumentException"></exception>
        private static byte SetIfInRange(byte value, byte min, byte max)
        {
            if (value < min || value > max)
            {
                throw new ArgumentException($"Value must be between {min} and {max}.");
            }
            return value;
        }
    }
}
