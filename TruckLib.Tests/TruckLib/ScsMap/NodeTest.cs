using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;
using System.Numerics;

namespace TruckLib.Tests.TruckLib.ScsMap 
{
    [Collection("Prefab collection")]
    public class NodeTest
    {
        PrefabFixture fixture;
        public NodeTest(PrefabFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Construct()
        {
            var node = new Node();
            Assert.NotEqual(0UL, node.Uid);
            Assert.Equal(Quaternion.Identity, node.Rotation);
        }

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

        [Fact]
        public void MergeRoadBwNodeIntoPrefab()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);
            var road = Road.Add(map, new Vector3(30, 0, 30), new Vector3(10, 0, 30), "blkw1");

            prefab.Nodes[1].Merge(road.Node);

            Assert.Equal(road.Node, prefab.Nodes[1]);
            Assert.Equal(prefab, road.Node.BackwardItem);
            Assert.Equal(road, road.Node.ForwardItem);
            AssertEx.Equal(new Quaternion(0, -0.707107f, 0, -0.707107f), road.Node.Rotation, 0.01f);
            Assert.True(road.Node.IsRed);
        }

        [Fact]
        public void MergeRoadFwNodeIntoPrefabOrigin()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);
            var road = Road.Add(map, new Vector3(50, 0, 70), new Vector3(55, 0, 55), "blkw1");

            prefab.Nodes[0].Merge(road.ForwardNode);

            Assert.Equal(road.ForwardNode, prefab.Nodes[0]);
            Assert.Equal(prefab, road.ForwardNode.ForwardItem);
            Assert.Equal(road, road.ForwardNode.BackwardItem);
            AssertEx.Equal(Quaternion.Identity, road.ForwardNode.Rotation, 0.01f);
            Assert.True(road.Node.IsRed);
        }

        [Fact]
        public void MergeRoadFwNodeIntoPrefab()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);
            var road = Road.Add(map, new Vector3(80, 0, 32), new Vector3(70, 0, 35), "blkw1");

            prefab.Nodes[3].Merge(road.ForwardNode);

            Assert.Equal(road.ForwardNode, prefab.Nodes[3]);
            Assert.Equal(prefab, road.ForwardNode.ForwardItem);
            Assert.Equal(road, road.ForwardNode.BackwardItem);
            AssertEx.Equal(new Quaternion(0, 0.707107f, 0, 0.707107f), road.ForwardNode.Rotation, 0.01f);
            Assert.True(road.Node.IsRed);
        }

        [Fact]
        public void MergeRoadBwNodeIntoPrefabOriginThrows()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);
            var road = Road.Add(map, new Vector3(55, 0, 55), new Vector3(50, 0, 70), "blkw1");

            Assert.Throws<InvalidOperationException>(() => prefab.Nodes[0].Merge(road.Node));
        }

        [Fact]
        public void SplitRoads()
        {
            var map = new Map("foo");
            var road1 = Road.Add(map, new Vector3(10, 0, 10), new Vector3(30, 0, 30), "blkw1");
            var road2 = road1.Append(new Vector3(50, 0, 50));

            var newNode = road2.Node.Split();

            Assert.NotEqual(road1.ForwardNode, road2.Node);
            Assert.Equal(newNode, road2.Node);
            Assert.Null(road1.ForwardNode.ForwardItem);
            Assert.Equal(road2, newNode.ForwardItem);
            Assert.Null(newNode.BackwardItem);
            Assert.True(newNode.IsRed);
            Assert.False(road1.ForwardNode.IsRed);
            Assert.Equal(road1.ForwardNode.Position, newNode.Position);
        }

        [Fact]
        public void SplitReturnsNull()
        {
            var map = new Map("foo");
            var model = Model.Add(map, new Vector3(1, 2, 3), "aaa", "bbb", "ccc");

            Assert.Null(model.Node.Split());
        }

        [Fact]
        public void SplitRoadAndPrefab()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);
            var expectedPfNodeRot = prefab.Nodes[3].Rotation;
            var road = prefab.AppendRoad(3, new Vector3(100, 0, 20), "ger1");

            var newNode = road.Node.Split();

            Assert.NotEqual(prefab.Nodes[3], road.Node);
            Assert.Equal(newNode, road.Node);
            Assert.Null(road.Node.BackwardItem);
            Assert.Equal(road, newNode.ForwardItem);
            Assert.Null(prefab.Nodes[3].BackwardItem);
            Assert.Equal(prefab, prefab.Nodes[3].ForwardItem);
            Assert.True(newNode.IsRed);
            Assert.False(prefab.Nodes[3].IsRed);
            Assert.Equal(prefab.Nodes[3].Position, newNode.Position);
            AssertEx.Equal(expectedPfNodeRot, prefab.Nodes[3].Rotation, 0.001f);
        }

        [Fact]
        public void SplitRoadAndPrefabAtOrigin()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);
            prefab.ChangeOrigin(3);
            var expectedPfNodeRot = prefab.Nodes[0].Rotation;
            var road = prefab.AppendRoad(0, new Vector3(100, 0, 20), "ger1");

            var newNode = road.ForwardNode.Split();

            Assert.NotEqual(prefab.Nodes[0], road.ForwardNode);
            Assert.Equal(newNode, road.ForwardNode);
            Assert.Null(road.ForwardNode.ForwardItem);
            Assert.Equal(road, newNode.BackwardItem);
            Assert.Null(prefab.Nodes[0].BackwardItem);
            Assert.Equal(prefab, prefab.Nodes[0].ForwardItem);
            Assert.False(newNode.IsRed);
            Assert.True(prefab.Nodes[0].IsRed);
            Assert.Equal(prefab.Nodes[0].Position, newNode.Position);
            AssertEx.Equal(expectedPfNodeRot, prefab.Nodes[0].Rotation, 0.001f);
        }

        [Fact]
        public void SplitPrefabs()
        {
            var map = new Map("foo");
            var prefab1 = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);
            var prefab2 = Prefab.Add(map, new Vector3(100, 0, 55), "dlc_blkw_02", fixture.CrossingPpd);
            prefab1.Attach(3, prefab2, 1);

            var newNode = prefab1.Nodes[3].Split();

            Assert.Equal(newNode, prefab2.Nodes[1]);
            Assert.Null(prefab1.Nodes[3].BackwardItem);
            Assert.Equal(prefab2, newNode.ForwardItem);
            Assert.Null(newNode.BackwardItem);
            Assert.NotEqual(prefab1.Nodes[3].Rotation, newNode.Rotation);
            Assert.False(newNode.IsRed);
        }
    }
}
