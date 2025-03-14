using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    public class PolylineTest
    {
        [Fact]
        public void InterpolateCurveDist()
        {
            var map = new Map("foo");
            var r1 = Road.Add(map, new(19, 0, 19.5f), new(65, 0, 23), "ger1");
            var r2 = r1.Append(new(98, 0, 43.5f));
            var r3 = r2.Append(new(146.5f, 0, 25));

            var actual = r2.InterpolateCurveDist(4.2f);

            Assert.NotNull(actual);
            Assert.Equal(new(68.90782f, 0, 24.699467f), actual?.Position);
            Assert.Equal(new(0, -0.85898876f, 0, 0.5119944f), actual?.Rotation);
        }

        [Fact]
        public void InterpolateCurveDistNullIfTooFar()
        {
            var map = new Map("foo");
            var r1 = Road.Add(map, new(19, 0, 19.5f), new(65, 0, 23), "ger1");
            var r2 = r1.Append(new(98, 0, 43.5f));

            var actual = r2.InterpolateCurveDist(999f);
            Assert.Null(actual);
        }
    }
}
