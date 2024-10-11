using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    /// <summary>
    /// A string type used in Prism3D which packs up to 12 characters 
    /// of a limited character set into 8 bytes.
    /// </summary>
    public struct Token : IBinarySerializable
    {
        /// <summary>
        /// The character set of this type.
        /// </summary>
        public static readonly char[] CharacterSet =
        { 
            '\0', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b',
            'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
            'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '_'
        };

        private static readonly int CharsetLength = CharacterSet.Length; // =38

        /// <summary>
        /// The maximum number of characters a token can contain.
        /// </summary>
        public static readonly int MaxLength = 12;

        /// <summary>
        /// Gets or sets the integer representation of the token.
        /// </summary>
        public ulong Value { get; set; }

        /// <summary>
        /// Gets or sets the string representation of the token.
        /// </summary>
        public string String
        {   
            readonly get => TokenToString(Value);
            set => Value = StringToToken(value);
        }

        /// <summary>
        /// Creates a new token.
        /// </summary>
        /// <param name="token">The integer representation of the token.</param>
        public Token(ulong token)
        {
            Value = token;
        }

        /// <summary>
        /// Creates a new token from a string.
        /// </summary>
        /// <param name="str">The string representation of the token.</param>
        public Token(string str)
        {
            Value = StringToToken(str);
        }

        /// <summary>
        /// Returns the string representation of the token.
        /// </summary>
        /// <returns>The string representation of the token.</returns>
        public override string ToString() => String;

        /// <summary>
        /// Converts a string to a token.
        /// </summary>
        /// <param name="input">The string representation of the token.</param>
        /// <returns>The integer representation of the token.</returns>
        public static ulong StringToToken(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            if (!IsValidToken(input))
                throw new ArgumentException($"Input is not a valid token.");

            var token = 0UL;
            input = input.ToLower();
            for (var i = 0; i < input.Length; i++)
            {
                token += (ulong)Math.Pow(CharsetLength, i) * 
                    (ulong)GetCharIndex(input[i]);
            }
            return token;
        }

        /// <summary>
        /// Converts a token to string.
        /// </summary>
        /// <param name="token">The integer representation of the token.</param>
        /// <returns>The string representation of the token.</returns>
        public static string TokenToString(ulong token)
        {
            // for empty tokens, return "" rather than "\0"
            if (token == 0)
                return "";

            // Determine length of string
            int length = 1;
            while (Math.Pow(CharsetLength, length) - 1 < token)
                length++;

            // reverse the token, from last to first character
            var input = new char[length];
            for (var i = length; i > 0; i--)
            {
                // find the last character of the input
                // by dividing by 38^len and ignoring the remainder
                var pow = (ulong)Math.Pow(CharsetLength, i - 1);
                var character = token / pow;
                input[i - 1] += CharacterSet[character];

                // subtract that part from the token
                token -= character * pow;
            }

            var inputStr = new string(input);
            return inputStr;
        }

        /// <summary>
        /// Returns the index of a character in the character set.
        /// </summary>
        /// <param name="letter">The character.</param>
        /// <returns>Its index in the character set.</returns>
        private static int GetCharIndex(char letter)
        {
            var index = Array.IndexOf(CharacterSet, letter);
            return index == -1 ? 0 : index;
        }

        /// <summary>
        /// Returns whether the given string contains a valid token.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>Whether it contains a valid token.</returns>
        public static bool IsValidToken(string str)
        {
            if (str.Length > MaxLength)
                return false;

            foreach (var c in str)
            {
                if (!CharacterSet.Contains(c))
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Value = r.ReadUInt64();
        }

        /// <inheritdoc/>
        public void Serialize(BinaryWriter w)
        {
            w.Write(Value);
        }

        public static implicit operator Token(string s)
        {
            if (!IsValidToken(s))
                throw new InvalidCastException($"Input is not a valid token.");

            return new Token(s);
        }

        public static implicit operator Token(ulong v) 
            => new(v);

        public static implicit operator Token(int v)
        {
            if (v < 0)
                throw new ArgumentOutOfRangeException(nameof(v), "Value can't be less than zero.");
            return new Token((ulong)v);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj switch
            {
                null =>         false,
                Token token2 => Value == token2.Value,
                ulong ul =>     Value == ul,
                long l =>       l >= 0 && Value == (ulong)l,
                uint ui =>      Value == ui,
                int i =>        i >= 0 && Value == (ulong)i,
                string str =>   String == str,
                _ => false,
            };
        }

        /// <inheritdoc/>
        public override int GetHashCode() =>
            Value.GetHashCode();

        public static bool operator ==(Token token, object obj) =>
            token.Equals(obj);

        public static bool operator !=(Token token, object obj) =>
            !token.Equals(obj);
    }
}
