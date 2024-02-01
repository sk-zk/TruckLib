using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;
using System.Numerics;

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
    }
}
