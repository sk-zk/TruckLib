using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap.Collections
{
    public class PathNodeListTest
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
            var map = new Map();
            var mover = Mover.Add(map, points, "aaa", "bbb", "ccc");

            mover.Nodes.Add(new Vector3(56, 0, 45));

            Assert.True(map.Nodes.ContainsKey(mover.Nodes[4].Uid));
            Assert.Equal(new Vector3(56, 0, 45), mover.Nodes[4].Position);
            Assert.False(mover.Nodes[4].IsRed);
            Assert.Equal(4, mover.Lengths.Count);
            AssertEx.Equal(new Quaternion(0.00633117f, 0.992021f, 0.0555912f, -0.112979f), mover.Nodes[2].Rotation);
            AssertEx.Equal(new Quaternion(-0.0112201f, 0.984171f, 0.163456f, 0.0675541f), mover.Nodes[3].Rotation);
            AssertEx.Equal(new Quaternion(0.0187338f, 0.936517f, 0.0506411f, -0.34644f), mover.Nodes[4].Rotation);
        }

        [Fact]
        public void Insert()
        {
            var points = new List<Vector3>() {
                new(13, 0, 13),
                new(38, 5, 5),
                new(60, 10, 16),
                new(44, 2, 31)
            };
            var map = new Map();
            var mover = Mover.Add(map, points, "aaa", "bbb", "ccc");

            mover.Nodes.Insert(2, new Vector3(44, 0, 15));

            Assert.True(map.Nodes.ContainsKey(mover.Nodes[2].Uid));
            Assert.Equal(new Vector3(44, 0, 15), mover.Nodes[2].Position);
            Assert.False(mover.Nodes[2].IsRed);
            Assert.Equal(4, mover.Lengths.Count);
            AssertEx.Equal(new Quaternion(0f, 0.729514f, 0f, -0.683966f), mover.Nodes[1].Rotation, 0.01f);
            AssertEx.Equal(new Quaternion(-0.0526284f, 0.846378f, -0.0851546f, -0.52309f), mover.Nodes[2].Rotation, 0.01f);
            AssertEx.Equal(new Quaternion(-0.000166893f, 0.998064f, -0.0621363f, -0.00268044f), mover.Nodes[3].Rotation, 0.01f);
        }

        [Fact]
        public void Remove()
        {
            var points = new List<Vector3>() {
                new(13, 0, 13),
                new(38, 5, 5),
                new(60, 10, 16),
                new(44, 2, 31)
            };
            var map = new Map();
            var mover = Mover.Add(map, points, "aaa", "bbb", "ccc");

            var nodeToRemove = mover.Nodes[3];
            mover.Nodes.RemoveAt(3);

            Assert.False(map.Nodes.ContainsKey(nodeToRemove.Uid));
            Assert.Equal(-1, mover.Nodes.IndexOf(nodeToRemove));
            Assert.Equal(2, mover.Lengths.Count);
            AssertEx.Equal(new Quaternion(-0.075905f, 0.586977f, -0.055407f, -0.804131f), mover.Nodes[0].Rotation);
            AssertEx.Equal(new Quaternion(-0.0714474f, 0.725294f, -0.0761533f, -0.680474f), mover.Nodes[1].Rotation);
            AssertEx.Equal(new Quaternion(-0.0526284f, 0.846378f, -0.0851546f, -0.52309f), mover.Nodes[2].Rotation);
        }
    }
}
