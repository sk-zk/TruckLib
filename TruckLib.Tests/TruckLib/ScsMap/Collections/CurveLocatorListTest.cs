using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap.Collections
{
    public class CurveLocatorListTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map();
            var curve = Curve.Add(map, new(10, 0, 10), new(30, 0, 10), "bar");

            curve.Locators.Add(new(10.29f, 0, 7.97f), Quaternion.Identity);
            curve.Locators.Add(new(29.71f, 0, 7.97f), Quaternion.Identity);

            Assert.Equal(2, curve.Locators.Count);

            Assert.Equal(new(10.29f, 0, 7.97f), curve.Locators[0].Position);
            Assert.Equal(Quaternion.Identity, curve.Locators[0].Rotation);
            Assert.Equal(new(29.71f, 0, 7.97f), curve.Locators[1].Position);
            Assert.Equal(Quaternion.Identity, curve.Locators[1].Rotation);

            Assert.True(map.Nodes.ContainsKey(curve.Locators[0].Uid));
            Assert.True(map.Nodes.ContainsKey(curve.Locators[1].Uid));
            Assert.True(curve.Locators[0].IsCurveLocator);
            Assert.True(curve.Locators[1].IsCurveLocator);
            Assert.Equal(curve, curve.Locators[0].BackwardItem);
            Assert.Null(curve.Locators[0].ForwardItem);
        }

        [Fact]
        public void RemoveAt()
        {
            var map = new Map();
            var curve = Curve.Add(map, new(10, 0, 10), new(30, 0, 10), "bar");

            var node = curve.Locators.Add(new(10.29f, 0, 7.97f), Quaternion.Identity);
            curve.Locators.RemoveAt(0);

            Assert.Empty(curve.Locators);
            Assert.False(map.Nodes.ContainsKey(node.Uid));
        }

        [Fact]
        public void Remove()
        {
            var map = new Map();
            var curve = Curve.Add(map, new(10, 0, 10), new(30, 0, 10), "bar");

            var node = curve.Locators.Add(new(10.29f, 0, 7.97f), Quaternion.Identity);
            var success = curve.Locators.Remove(node);

            Assert.True(success);
            Assert.Empty(curve.Locators);
            Assert.False(map.Nodes.ContainsKey(node.Uid));
        }

        [Fact]
        public void Clear()
        {
            var map = new Map();
            var curve = Curve.Add(map, new(10, 0, 10), new(30, 0, 10), "bar");

            var node1 = curve.Locators.Add(new(10.29f, 0, 7.97f), Quaternion.Identity);
            var node2 = curve.Locators.Add(new(29.71f, 0, 7.97f), Quaternion.Identity);
            curve.Locators.Clear();

            Assert.Empty(curve.Locators);
            Assert.False(map.Nodes.ContainsKey(node1.Uid));
            Assert.False(map.Nodes.ContainsKey(node2.Uid));
        }

        [Fact]
        public void AddThrowsIfFull()
        {
            var map = new Map();
            var curve = Curve.Add(map, new(10, 0, 10), new(30, 0, 10), "bar");

            curve.Locators.Add(new(10.29f, 0, 7.97f), Quaternion.Identity);
            curve.Locators.Add(new(29.71f, 0, 7.97f), Quaternion.Identity);
            Assert.Throws<IndexOutOfRangeException>(() =>
                curve.Locators.Add(new(29.71f, 0, 7.97f), Quaternion.Identity));
        }
    }
}
