using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;
using System.Numerics;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    public class PolylineLengthTest
    {
        [Fact]
        public void Calculate()
        {
            var node1 = new Node()
            {
                Position = new Vector3(33.5859f, 0f, 15.3047f),
                Rotation = new Quaternion(-0.0400523f, 0.539937f, -0.0257339f, -0.840358f)
            };
            var node2 = new Node()
            {
                Position = new Vector3(60.5938f, 5f, 12.0234f),
                Rotation = new Quaternion(0.0107212f, 0.762572f, 0.0126424f, -0.646691f)
            };

            var actual = PolylineLength.Calculate(node1, node2);
            Assert.Equal(28.145f, actual, 1e-3);
        }
    }
}
