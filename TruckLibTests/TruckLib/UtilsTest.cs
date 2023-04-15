using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib;

namespace TruckLibTests.TruckLib
{
    public class UtilsTest
    {
        [Fact]
        public void SwitchXY()
        {
            var input = new int[2, 2]
            {
                { 1, 2 },
                { 3, 4 },
            };
            var expected = new int[2, 2]
            {
                { 1, 3 },
                { 2, 4 },
            };
            Assert.Equal(expected, Utils.SwitchXY(input));
        }

        [Fact]
        public void MirrorX()
        {
            var input = new int[2, 2]
            {
                { 1, 2 },
                { 3, 4 },
            };
            var expected = new int[2, 2]
            {
                { 3, 4 },
                { 1, 2 },
            };
            Assert.Equal(expected, Utils.MirrorX(input));
        }

        [Fact]
        public void SetIfInRange()
        {
            Assert.Equal(5, Utils.SetIfInRange(5, 0, 9));
            Assert.Equal(0.25f, Utils.SetIfInRange(0.25f, 0, 0.5f));
        }

        [Fact]
        public void SetIfInRangeThrows()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Utils.SetIfInRange(10, 0, 9);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                Utils.SetIfInRange(1f, 0f, 0.5f);
            });
        }
    }
}
