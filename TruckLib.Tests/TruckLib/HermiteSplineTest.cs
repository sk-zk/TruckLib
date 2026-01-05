using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib
{
    public class HermiteSplineTest
    {
        [Fact]
        public void GetEquidistantPoints()
        {
            var node1 = new Node()
            {
                Position = new(65, 0, 23),
                Rotation = new(0, -0.8095299f, 0, 0.5870785f)
            };
            var node2 = new Node()
            {
                Position = new(98, 0, 43.5f),
                Rotation = new(0, -0.7401302f, 0, 0.67246354f)
            };

            var actual = HermiteSpline.GetEquidistantPoints(node1, node2, 8);
            var expected = new OrientedPoint[]
            {
                new(new(64.99936f, 0, 22.999792f), new(0, -0.8095205f, 0, 0.5870916f)),
                new(new(72.18538f, 0, 26.741638f), new(0, -0.8870569f, 0, 0.4616601f)),
                new(new(78.50672f, 0, 31.848032f), new(0, -0.9109966f, 0, 0.41241378f)),
                new(new(84.65635f, 0, 37.146854f), new(0, -0.90160865f, 0, 0.43255267f)),
                new(new(91.41531f, 0, 41.587543f), new(0, -0.8472321f, 0, 0.5312229f)),
            };

            Assert.Equal(expected.Length, actual.Count);
            for (int i = 0; i < expected.Length; i++)
            {
                AssertEx.Equal(expected[i].Position, actual[i].Position, 0.001f);
                AssertEx.Equal(expected[i].Rotation, actual[i].Rotation, 0.001f);
            }
        }
    }
}
