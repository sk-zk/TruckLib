using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib;

namespace TruckLibTests.TruckLib
{
    public class FlagFieldTest
    {
        [Fact]
        public void Get() 
        {
            var ff = new FlagField(0b100);
            Assert.True(ff[2]);
            Assert.False(ff[1]);
        }

        [Fact]
        public void GetThrowsOutOfRangeException()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var ff = new FlagField(0b100);
                _ = ff[32];
            });
        }

        [Fact]
        public void Set()
        {
            var ff = new FlagField(0b100);
            ff[3] = true;
            Assert.Equal(0b1100u, ff.Bits);
        }

        [Fact]
        public void SetThrowsOutOfRangeException()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var ff = new FlagField();
                ff[32] = true;
            });
        }

        [Fact]
        public void GetByte()
        {
            var ff = new FlagField(0xdeadbeef);
            Assert.Equal((byte)0xad, ff.GetByte(2));
        }

        [Fact]
        public void GetByteThrowsOutOfRangeException()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var ff = new FlagField(0b100);
                _ = ff.GetByte(4);
            });
        }

        [Fact]
        public void SetByte()
        {
            var ff = new FlagField(0x11220044);
            ff.SetByte(1, 0x33);
            Assert.Equal(0x11223344u, ff.Bits);
        }

        [Fact]
        public void SetByteThrowsOutOfRangeException()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var ff = new FlagField();
                ff.SetByte(4, 0);
            });
        }

        [Fact]
        public void ToBoolArray()
        {
            var ff = new FlagField(0b100);
            var arr = ff.ToBoolArray();
            Assert.Equal(32, arr.Length);
            Assert.False(arr[1]);
            Assert.True(arr[2]);
        }

        [Fact]
        public void GetBitSring()
        {
            var ff = new FlagField(0b110110);
            Assert.Equal((uint)0b11011, ff.GetBitString(1, 5));
        }

        [Fact]
        public void GetBitSringThrowsOutOfRangeException()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var ff = new FlagField();
                ff.GetBitString(0, 32);
            });
        }

        [Fact]
        public void SetBitString()
        {
            var ff = new FlagField();
            ff.SetBitString(1, 5, 0b11011);
            Assert.Equal((uint)0b110110, ff.Bits);
        }

        [Fact]
        public void SetBitSringThrowsOutOfRangeException()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var ff = new FlagField();
                ff.SetBitString(24, 24, 0);
            });
        }
    }
}
