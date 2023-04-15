using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib;

namespace TruckLibTests.TruckLib
{
    public class StringUtilsTest
    {
        [Theory]
        [InlineData("101", true)]
        [InlineData("-0.2", true)]
        [InlineData("2.34446041e-15", true)]
        [InlineData("&3a80bedf", true)]
        [InlineData("0x42", false)]
        public void IsNumerical(string input, bool expected)
        {
            Assert.Equal(expected, StringUtils.IsNumerical(input));
        }

        [Theory]
        [InlineData("&3a80bedf", true)]
        [InlineData("3a80", false)]
        public void IsHexNotationFloat(string input, bool expected)
        {
            Assert.Equal(expected, StringUtils.IsHexNotationFloat(input));
        }

        [Theory]
        [InlineData("DEADBEEF", true)]
        [InlineData("0a1b2C", true)]
        [InlineData("G", false)]
        public void IsHexadecimal(string input, bool expected)
        {
            Assert.Equal(expected, StringUtils.IsHexadecimal(input));
        }

        [Fact]
        public void CStringBytesToList()
        {
            var input = Encoding.ASCII.GetBytes("abc\0def\0ghi\0");
            var expected = new List<string> { "abc", "def", "ghi" };
            Assert.Equal(expected, StringUtils.CStringBytesToList(input));
        }

        [Fact]
        public void ListToCStringByteList()
        {
            var expected = new List<byte[]> {
                Encoding.ASCII.GetBytes("abc\0"),
                Encoding.ASCII.GetBytes("def\0"),
                Encoding.ASCII.GetBytes("ghi\0")
            };
            var input = new List<string> { "abc", "def", "ghi" };
            Assert.Equal(expected, StringUtils.ListToCStringByteList(input));
        }
    }
}
