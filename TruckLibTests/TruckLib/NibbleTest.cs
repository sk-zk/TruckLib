using TruckLib;

namespace TruckLibTests.TruckLib
{
    public class NibbleTest
    {
        [Fact]
        public void ConstructorAndEq()
        {
            var n = new Nibble(14);
            Assert.True(n == 14);
        }

        [Fact]
        public void Add()
        {
            Assert.True((Nibble)14 + (Nibble)3 == (Nibble)1);
        }

        [Fact]
        public void AddInt()
        {
            Assert.True((Nibble)14 + 3 == (Nibble)1);
        }

        [Fact]
        public void Increment()
        {
            var n = (Nibble)14;
            n++;
            Assert.True(n == (Nibble)15);
        }

        [Fact]
        public void Subtract()
        {
            Assert.True((Nibble)1 - (Nibble)3 == (Nibble)14);
        }

        [Fact]
        public void SubtractInt()
        {
            Assert.True((Nibble)1 - 3 == (Nibble)14);
        }

        [Fact]
        public void Decrement()
        {
            var n = (Nibble)14;
            n--;
            Assert.True(n == (Nibble)13);
        }

        [Fact]
        public void ToString_()
        {
            Assert.Equal("14", ((Nibble)14).ToString());
        }

        [Fact]
        public void ThrowsIfOutOfRange()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var n = (Nibble)(-3);
            });
        }
    }
}