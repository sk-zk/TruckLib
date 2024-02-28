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
