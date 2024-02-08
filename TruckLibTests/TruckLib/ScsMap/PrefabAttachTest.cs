using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;
using TruckLib;

namespace TruckLibTests.TruckLib.ScsMap
{
    [Collection("Prefab collection")]
    public class PrefabAttachTest
    {
        PrefabFixture fixture;
        public PrefabAttachTest(PrefabFixture fixture)
        {
            this.fixture = fixture;
        }


        [Fact]
        public void AppendRoad()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd,
                Quaternion.CreateFromYawPitchRoll(MathEx.Rad(-90f), 0, 0));

            var road = prefab.AppendRoad(1, new Vector3(55, 0, 10), "blkw1");

            Assert.True(map.MapItems.ContainsKey(road.Uid));
            Assert.True(map.Nodes.ContainsKey(road.ForwardNode.Uid));
            Assert.Equal(prefab.Nodes[1], road.Node);
            Assert.True(road.Node.IsRed);
            Assert.Equal(prefab, road.Node.BackwardItem);
            Assert.Equal(road, road.Node.ForwardItem);
            Assert.Equal(new Vector3(55, 0, 10), road.ForwardNode.Position);
            AssertEx.Equal(new Quaternion(0, 0, 0, -1), road.Node.Rotation, 0.01f);
            AssertEx.Equal(new Quaternion(0, 0.263698f, 0, 0.964605f), road.ForwardNode.Rotation, 0.01f);
        }

        [Fact]
        public void AppendRoadThrowsIfNodeNotFree()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd,
                Quaternion.CreateFromYawPitchRoll(MathEx.Rad(-90f), 0, 0));

            var road = prefab.AppendRoad(1, new Vector3(55, 0, 10), "blkw1");
            Assert.Throws<InvalidOperationException>(() =>
                prefab.AppendRoad(1, new Vector3(69, 0, 69), "blkw1"));
        }

        [Fact]
        public void PrependRoad()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd,
                Quaternion.CreateFromYawPitchRoll(MathEx.Rad(-90f), 0, 0));

            var road = prefab.AppendRoad(0, new Vector3(10, 0, 55), "blkw1");

            Assert.True(map.MapItems.ContainsKey(road.Uid));
            Assert.True(map.Nodes.ContainsKey(road.ForwardNode.Uid));
            Assert.Equal(prefab.Nodes[0], road.ForwardNode);
            Assert.True(road.Node.IsRed);
            Assert.Equal(prefab, road.ForwardNode.ForwardItem);
            Assert.Equal(road, road.ForwardNode.BackwardItem);
            Assert.Equal(new Vector3(10, 0, 55), road.Node.Position);
        }

        [Fact]
        public void PrependRoadThrowsIfNodeNotFree()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd,
                Quaternion.CreateFromYawPitchRoll(MathEx.Rad(-90f), 0, 0));

            var road = prefab.AppendRoad(0, new Vector3(10, 0, 55), "blkw1");
            Assert.Throws<InvalidOperationException>(() =>
                prefab.AppendRoad(0, new Vector3(69, 0, 69), "blkw1"));
        }


        [Fact]
        public void AttachPrefabs()
        {
            var map = new Map("foo");
            var prefab1 = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);
            var prefab2 = Prefab.Add(map, new Vector3(100, 0, 55), "dlc_blkw_02", fixture.CrossingPpd);

            prefab1.Attach(3, prefab2, 1);

            Assert.Equal(prefab1.Nodes[3], prefab2.Nodes[1]);
            AssertEx.Equal(new Vector3(86, 0, 50), prefab2.Nodes[0].Position, 0.001f);
            Assert.Equal(prefab1, prefab1.Nodes[3].ForwardItem);
            Assert.Equal(prefab2, prefab1.Nodes[3].BackwardItem);
            Assert.False(prefab1.Nodes[3].IsRed);
            AssertEx.Equal(new Quaternion(0, -0.707107f, 0, -0.707107f), prefab1.Nodes[3].Rotation, 0.001f);
        }

        [Fact]
        public void AttachPrefabsP1Origin()
        {
            var map = new Map("foo");
            var prefab1 = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);
            var prefab2 = Prefab.Add(map, new Vector3(48, 0, 89), "dlc_blkw_02", fixture.CrossingPpd);

            prefab1.Attach(0, prefab2, 2);

            Assert.Equal(prefab1.Nodes[0], prefab2.Nodes[2]);
            AssertEx.Equal(new Vector3(50, 0, 86), prefab2.Nodes[0].Position, 0.001f);
            Assert.Equal(prefab1, prefab1.Nodes[0].ForwardItem);
            Assert.Equal(prefab2, prefab1.Nodes[0].BackwardItem);
            Assert.True(prefab1.Nodes[0].IsRed);
            AssertEx.Equal(Quaternion.Identity, prefab1.Nodes[0].Rotation, 0.001f);
        }

        [Fact]
        public void AttachPrefabsP2Origin()
        {
            var map = new Map("foo");
            var prefab1 = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);
            var prefab2 = Prefab.Add(map, new Vector3(100, 0, 55), "dlc_blkw_02", fixture.CrossingPpd);
            prefab2.ChangeOrigin(1);

            prefab1.Attach(3, prefab2, 0);

            Assert.Equal(prefab1.Nodes[3], prefab2.Nodes[0]);
            AssertEx.Equal(new Vector3(86, 0, 50), prefab2.Nodes[3].Position, 0.001f);
            Assert.Equal(prefab1, prefab1.Nodes[3].BackwardItem);
            Assert.Equal(prefab2, prefab1.Nodes[3].ForwardItem);
            Assert.True(prefab1.Nodes[3].IsRed);
            AssertEx.Equal(new Quaternion(0, 0.707107f, 0, -0.707107f), prefab1.Nodes[3].Rotation, 0.001f);
        }

        [Fact]
        public void AttachThrowsIfNodeOccupied()
        {
            var map = new Map("foo");
            var prefab1 = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);
            var prefab2 = Prefab.Add(map, new Vector3(100, 0, 55), "dlc_blkw_02", fixture.CrossingPpd);

            prefab1.AppendRoad(3, new Vector3(69, 0, 69), "bar");

            Assert.Throws<InvalidOperationException>(() => prefab1.Attach(3, prefab2, 1));
        }

        [Fact]
        public void AttachThrowsIfBothNodesAreOrigin()
        {
            var map = new Map("foo");
            var prefab1 = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);
            prefab1.ChangeOrigin(3);
            var prefab2 = Prefab.Add(map, new Vector3(100, 0, 55), "dlc_blkw_02", fixture.CrossingPpd);
            prefab2.ChangeOrigin(1);

            Assert.Throws<InvalidOperationException>(() => prefab1.Attach(0, prefab2, 0));
        }
    }
}
