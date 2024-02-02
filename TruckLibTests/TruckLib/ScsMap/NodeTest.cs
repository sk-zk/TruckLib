using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;
using System.Numerics;
using Newtonsoft.Json.Bson;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class NodeTest
    {
        [Fact]
        public void MergeRoadsViaFwNodeOfFirst()
        {
            var map = new Map("foo");
            var road1 = Road.Add(map, new(10, 0, 10), new(30, 0, 10), "ger1");
            var road2 = Road.Add(map, new(32, 0, 12), new(50, 0, 30), "ger1");

            var nodeToKeep = road1.ForwardNode;
            var nodeToMerge = road2.Node;
            nodeToKeep.Merge(nodeToMerge);

            Assert.Equal(nodeToKeep, road2.Node);
            Assert.Equal(new(30, 0, 10), road2.Node.Position);
            Assert.True(nodeToKeep.IsRed);
            Assert.False(map.Nodes.ContainsKey(nodeToMerge.Uid));
        }

        [Fact]
        public void MergeRoadsViaBwNodeOfSecond()
        {
            var map = new Map("foo");
            var road1 = Road.Add(map, new(10, 0, 10), new(30, 0, 10), "ger1");
            var road2 = Road.Add(map, new(32, 0, 12), new(50, 0, 30), "ger1");

            var nodeToKeep = road2.Node;
            var nodeToMerge = road1.ForwardNode;
            nodeToKeep.Merge(nodeToMerge);

            Assert.Equal(nodeToKeep, road1.ForwardNode);
            Assert.Equal(new(32, 0, 12), road2.Node.Position);
            Assert.True(nodeToKeep.IsRed);
            Assert.False(map.Nodes.ContainsKey(nodeToMerge.Uid));
        }

        [Fact]
        public void MergeThrowsIfTryingToMergeTwoRedNodes()
        {
            var map = new Map("foo");
            var road1 = Road.Add(map, new(10, 0, 10), new(30, 0, 10), "ger1");
            var road2 = Road.Add(map, new(32, 0, 12), new(50, 0, 30), "ger1");

            var nodeToKeep = road1.Node;
            var nodeToMerge = road2.Node;
            Assert.Throws<InvalidOperationException>(() => nodeToKeep.Merge(nodeToMerge));
        }

        [Fact]
        public void MergeThrowsIfTryingToMergeTwoGreenNodes()
        {
            var map = new Map("foo");
            var road1 = Road.Add(map, new(10, 0, 10), new(30, 0, 10), "ger1");
            var road2 = Road.Add(map, new(32, 0, 12), new(50, 0, 30), "ger1");

            var nodeToKeep = road1.ForwardNode;
            var nodeToMerge = road2.ForwardNode;
            Assert.Throws<InvalidOperationException>(() => nodeToKeep.Merge(nodeToMerge));
        }

        [Fact]
        public void MergeIntoCurveLocatorNode()
        {
            var map = new Map("foo");
            var curve = Curve.Add(map, new(10, 0, 10), new(30, 0, 10), "bar");

            curve.Locators.Add(new(10.29f, 0, 7.97f), Quaternion.Identity);
            curve.Locators.Add(new(29.71f, 0, 7.97f), Quaternion.Identity);
            var node2 = curve.Locators[1];

            var curve2 = Curve.Add(map, node2.Position, new(34, 0, 4), "baz");
            node2.Merge(curve2.Node);

            Assert.Equal(node2, curve2.Node);
            Assert.False(node2.IsRed);
            Assert.True(node2.IsCurveLocator);
        }
    }
}
