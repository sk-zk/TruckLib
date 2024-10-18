using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    public class MoverTest
    {
        [Fact]
        public void Add()
        {
            var points = new List<Vector3>() {
                new(13, 0, 13),
                new(38, 5, 5),
                new(60, 10, 16),
                new(44, 2, 31)
            };
            var expectedRotations = new List<Quaternion>()
            {
                new(-0.075905f, 0.586977f, -0.055407f, -0.804131f),
                new(-0.0714474f, 0.725294f, -0.0761533f, -0.680474f),
                new(0.00633117f, 0.992021f, 0.0555912f, -0.112979f),
                new(-0.0691682f, 0.903593f, 0.159656f, 0.391465f)
            };

            var map = new Map("foo");
            var mover = Mover.Add(map, points, "aaa", "bbb", "ccc");

            Assert.Equal(4, mover.Nodes.Count);
            Assert.Equal(3, mover.Lengths.Count);
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

        [Fact]
        public void MoveNode()
        {
            var points = new List<Vector3>() {
                new(13, 0, 13),
                new(38, 5, 5),
                new(60, 10, 16),
                new(44, 2, 31)
            };
            var map = new Map("foo");
            var mover = Mover.Add(map, points, "aaa", "bbb", "ccc");

            mover.Nodes[1].Move(new Vector3(24, 5, 24));

            Assert.Equal(new Vector3(24, 5, 24), mover.Nodes[1].Position);
            AssertEx.Equal(new Quaternion(-0.0592645f, 0.912733f, -0.143077f, -0.378067f), mover.Nodes[0].Rotation);
            AssertEx.Equal(new Quaternion(-0.0714474f, 0.725294f, -0.0761533f, -0.680474f), mover.Nodes[1].Rotation);
            AssertEx.Equal(new Quaternion(0.0406577f, 0.813567f, 0.0573061f, -0.57721f), mover.Nodes[2].Rotation);
        }
    }
}
