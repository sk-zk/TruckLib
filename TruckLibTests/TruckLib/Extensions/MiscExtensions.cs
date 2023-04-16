using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib;

namespace TruckLibTests.TruckLib.Extensions
{
    public class MiscExtensions
    {
        [Fact]
        public void Push()
        {
            int[] arr = { 1, 2, 3 };
            arr = arr.Push(4);
            Assert.Equal(arr, new int[] { 1, 2, 3, 4 });
        }

        [Fact]
        public void PushEmpty()
        {
            int[] arr = null;
            arr = arr.Push(42);
            Assert.Equal(arr, new int[] { 42 });
        }

        [Fact]
        public void QuaternionToEuler()
        {
            var q = Quaternion.CreateFromYawPitchRoll(3, 1, 2);
            var expected = new Vector3(1, 3, 2);
            var actual = q.ToEuler();
            Assert.Equal(expected.X, actual.X, 4u);
            Assert.Equal(expected.Y, actual.Y, 4u);
            Assert.Equal(expected.Z, actual.Z, 4u);
        }

        [Fact]
        public void BoolToByte()
        {
            // Quite possibly the most important unit test imaginable
            Assert.Equal((byte)0, false.ToByte());
            Assert.Equal((byte)1, true.ToByte());
        }
    }
}
