using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class MoverTest
    {
        [Fact]
        public void Add()
        {
            var points = new List<Vector3>() {
                new(14.45f, 0, 12.32f),
                new(28.68f, 0, 11.37f),
                new(28.90f, 0, 25.28f),
                new(14.98f, 0, 22.84f)
            };
            var expectedRotations = new List<Quaternion>()
            {
                new(0, 0.682978f, 0, -0.730439f),
                new(0, 0.913131f, 0, -0.407665f),
                new(0, 0.906075f, 0, 0.423116f),
                new(0, 0.643127f, 0, 0.76576f)
            };

            var map = new Map("foo");
            var mover = Mover.Add(map, points, "aaa", "bbb", "ccc");

            Assert.Equal(4, mover.Nodes.Count);
            for (int i = 0; i < mover.Nodes.Count; i++)
            {
                Assert.Equal(points[i], mover.Nodes[i].Position);
                Assert.True(map.Nodes.ContainsKey(mover.Nodes[i].Uid));
                Assert.Equal(mover, mover.Nodes[i].ForwardItem);
                Assert.Null(mover.Nodes[i].BackwardItem);
                AssertEx.Equal(expectedRotations[i], mover.Nodes[i].Rotation, 0.001f);
            }

            Assert.True(mover.Nodes[0].IsRed);
            for (int i = 1; i < mover.Nodes.Count; i++)
            {
                Assert.False(mover.Nodes[i].IsRed);
            }
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var mover = Mover.Add(map, new List<Vector3>() {
                new(-30, 0, -30),
                new(-10, 0, -10),
            }, "aaa", "bbb", "ccc");

            mover.Move(new Vector3(20, 0, 20));

            Assert.Equal(new Vector3(20, 0, 20), mover.Nodes[0].Position);
            Assert.Equal(new Vector3(40, 0, 40), mover.Nodes[1].Position);
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var mover = Mover.Add(map, new List<Vector3>() {
                new(-30, 0, -30),
                new(-10, 0, -10),
            }, "aaa", "bbb", "ccc");

            mover.Translate(new Vector3(50, 0, 50));

            Assert.Equal(new Vector3(20, 0, 20), mover.Nodes[0].Position);
            Assert.Equal(new Vector3(40, 0, 40), mover.Nodes[1].Position);
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var mover = Mover.Add(map, new List<Vector3>() {
                new(-13.11f, 0, 14.21f),
                new(-4.35f, 0, 3.08f),
                new(6.25f, 0, 3.16f),
                new(12.89f, 0, 14.29f)
            }, "aaa", "bbb", "ccc");

            map.Delete(mover);

            Assert.Empty(map.MapItems);
            Assert.Empty(map.Nodes);
        }
    }
}
