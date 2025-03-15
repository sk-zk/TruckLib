using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;
using System.Numerics;
using TruckLib;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    [Collection("Prefab collection")]
    public class RoadTest
    {
        PrefabFixture fixture;
        public RoadTest(PrefabFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var road = Road.Add(map, new Vector3(10, 0, 10), new Vector3(15, 0, 40), "bar");

            Assert.True(map.MapItems.ContainsKey(road.Uid));

            Assert.Equal(new Vector3(10, 0, 10), road.Node.Position);
            Assert.Equal(new Vector3(15, 0, 40), road.ForwardNode.Position);
            Assert.Equal(road, road.Node.ForwardItem);
            Assert.Null(road.Node.BackwardItem);
            Assert.Equal(road, road.ForwardNode.BackwardItem);
            Assert.Null(road.ForwardNode.ForwardItem);
            Assert.True(map.Nodes.ContainsKey(road.Node.Uid));
            Assert.True(map.Nodes.ContainsKey(road.ForwardNode.Uid));
            Assert.True(road.Node.IsRed);
            AssertEx.Equal(new Quaternion(0, 0.996593f, 0, -0.0824807f), road.Node.Rotation, 0.001f);
            AssertEx.Equal(new Quaternion(0, 0.996593f, 0, -0.0824807f), road.ForwardNode.Rotation, 0.001f);
        }

        [Fact]
        public void Append()
        {
            var map = new Map("foo");
            var points = new List<Vector3>()
            {
                new(25.2734f, 0, 33.1445f),
                new(48.2188f, 0, 6.16406f),
                new(81.8906f, 0, 11.582f),
                new(99.5234f, 0, 32.1875f),
            };
            var expectedRotations = new List<Quaternion>()
            {
                new(0, 0.345128f, 0, -0.938556f),
                new(0, 0.57186f, 0, -0.820351f),
                new(0, 0.862924f, 0, -0.505333f),
                new(0, 0.938026f, 0, -0.346565f)
            };
            var expectedTerrainColumns = new int[]
            {
                9, 9, 6
            };

            var road1 = Road.Add(map, points[0], points[1], "bar", 30, 30);
            road1.IgnoreCutPlanes = true;
            road1.Left.Variant = "baz";
            road1.Left.ShoulderBlocked = true;
            var road2 = road1.Append(points[2]);
            var road3 = road2.Append(points[3]);

            Assert.Equal(road1.RoadType, road3.RoadType);
            Assert.Equal(road1.IgnoreCutPlanes, road3.IgnoreCutPlanes);
            Assert.Equal(road1.Left.Variant, road3.Left.Variant);
            Assert.Equal(road1.Left.ShoulderBlocked, road3.Left.ShoulderBlocked);

            Assert.True(map.MapItems.ContainsKey(road2.Uid));
            Assert.True(map.MapItems.ContainsKey(road3.Uid));
            Assert.True(map.Nodes.ContainsKey(road2.ForwardNode.Uid));
            Assert.True(map.Nodes.ContainsKey(road3.ForwardNode.Uid));

            Assert.Equal(road2, road2.Node.ForwardItem);
            Assert.Equal(road1, road2.Node.BackwardItem);
            Assert.Equal(road2, road2.ForwardNode.BackwardItem);

            Assert.True(road2.Node.IsRed);
            Assert.True(road3.Node.IsRed);
            Assert.False(road3.ForwardNode.IsRed);

            AssertEx.Equal(points[0], road1.Node.Position, 0.001f);
            AssertEx.Equal(points[1], road1.ForwardNode.Position, 0.001f);
            AssertEx.Equal(points[2], road2.ForwardNode.Position, 0.001f);
            AssertEx.Equal(points[3], road3.ForwardNode.Position, 0.001f);
            AssertEx.Equal(expectedRotations[0], road1.Node.Rotation, 0.001f);
            AssertEx.Equal(expectedRotations[1], road1.ForwardNode.Rotation, 0.001f);
            AssertEx.Equal(expectedRotations[2], road2.ForwardNode.Rotation, 0.001f);
            AssertEx.Equal(expectedRotations[3], road3.ForwardNode.Rotation, 0.001f);
            Assert.Equal(expectedTerrainColumns[0], road1.Left.Terrain.QuadData.Cols);
            Assert.Equal(expectedTerrainColumns[0], road1.Right.Terrain.QuadData.Cols);
            Assert.Equal(expectedTerrainColumns[1], road2.Left.Terrain.QuadData.Cols);
            Assert.Equal(expectedTerrainColumns[1], road2.Right.Terrain.QuadData.Cols);
            Assert.Equal(expectedTerrainColumns[2], road3.Left.Terrain.QuadData.Cols);
            Assert.Equal(expectedTerrainColumns[2], road3.Right.Terrain.QuadData.Cols);
        }

        [Fact]
        public void DisallowAppendIfForwardItemExists()
        {
            var map = new Map("foo");
            var road1 = Road.Add(map, new Vector3(20, 0, 20), new Vector3(10, 0, 10), "bar");
            var road2 = road1.Append(new Vector3(-10, 0, -10), true);

            Assert.Throws<InvalidOperationException>(
                () => road1.Append(new Vector3(-20, 0, -20)));
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var road = Road.Add(map, new Vector3(10, 0, 10), new Vector3(15, 0, 40), "ger1");

            road.Move(new Vector3(20, 0, 20));

            AssertEx.Equal(new Vector3(20, 0, 20), road.Node.Position);
            AssertEx.Equal(new Vector3(25, 0, 50), road.ForwardNode.Position);
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var road = Road.Add(map, new Vector3(10, 0, 10), new Vector3(15, 0, 40), "ger1");

            road.Translate(new Vector3(10, 0, 10));

            AssertEx.Equal(new Vector3(20, 0, 20), road.Node.Position);
            AssertEx.Equal(new Vector3(25, 0, 50), road.ForwardNode.Position);
        }

        [Fact]
        public void DeleteIndividual()
        {
            var map = new Map("foo");
            var road = Road.Add(map, new Vector3(10, 0, 10), new Vector3(15, 0, 40), "ger1");

            map.Delete(road);

            Assert.Empty(map.MapItems);
            Assert.Empty(map.Nodes);
        }

        [Fact]
        public void DeleteInChain()
        {
            var map = new Map("foo");
            var points = new List<Vector3>()
            {
                new(25.2734f, 0, 33.1445f),
                new(48.2188f, 0, 6.16406f),
                new(81.8906f, 0, 11.582f),
                new(99.5234f, 0, 32.1875f),
            };
            var road1 = Road.Add(map, points[0], points[1], "bar");
            var road2 = road1.Append(points[2]);
            var road3 = road2.Append(points[3]);

            map.Delete(road2);

            Assert.False(map.MapItems.ContainsKey(road2.Uid));
            AssertEx.Equal(new Quaternion(0, 0.345128f, 0, -0.938556f), road1.Node.Rotation, 0.001f);
            AssertEx.Equal(new Quaternion(0, 0.345128f, 0, -0.938556f), road1.ForwardNode.Rotation, 0.001f);
            AssertEx.Equal(new Quaternion(0, 0.938026f, 0, -0.346565f), road3.Node.Rotation, 0.001f);
            AssertEx.Equal(new Quaternion(0, 0.938026f, 0, -0.346565f), road3.ForwardNode.Rotation, 0.001f);
            Assert.False(road1.ForwardNode.IsRed);
            Assert.Null(road1.ForwardNode.ForwardItem);
            Assert.True(road3.Node.IsRed);
            Assert.Null(road3.Node.BackwardItem);
        }

        [Fact]
        public void DeleteWhileBwItemIsPrefab()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd,
                Quaternion.CreateFromYawPitchRoll(MathEx.Rad(-90f), 0, 0));
            var road = prefab.AppendRoad(1, new Vector3(55, 0, 10), "blkw1");

            map.Delete(road);

            AssertEx.Equal(new Quaternion(0, 1, 0, 0), prefab.Nodes[1].Rotation);
            Assert.False(prefab.Nodes[1].IsRed);
        }
    }
}
