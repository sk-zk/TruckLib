using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.Models;

namespace TruckLibTests.TruckLib
{
    public class StringUtilsTest
    {     
        [Fact]
        public void CStringBytesToList()
        {
            byte[] input = Encoding.ASCII.GetBytes("abc\0def\0ghi\0");
            List<string> expected = ["abc", "def", "ghi"];
            Assert.Equal(expected, StringUtils.CStringBytesToList(input));
        }

        [Fact]
        public void ListToCStringByteList()
        {
            List<byte[]> expected = [
                Encoding.ASCII.GetBytes("abc\0"),
                Encoding.ASCII.GetBytes("def\0"),
                Encoding.ASCII.GetBytes("ghi\0")
            ];
            List<string> input = ["abc", "def", "ghi"];
            Assert.Equal(expected, StringUtils.ListToCStringByteList(input));
        }        
    }
}
