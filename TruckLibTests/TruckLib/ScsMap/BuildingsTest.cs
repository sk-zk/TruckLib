using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TruckLib.Model;
using TruckLib.ScsMap;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class BuildingsTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var buildings = Buildings.Add(map, new Vector3(10, 0, 10), new Vector3(-10, 0, -10), "bar");

            Assert.Equal(buildings.Name, "bar");
            Assert.Equal(28.28f, buildings.Length, 0.01f);

            Assert.Equal(buildings.ForwardNode, buildings.GetMainNode());
            Assert.True(buildings.Node.IsRed);
            Assert.False(buildings.ForwardNode.IsRed);
            Assert.Equal(buildings, buildings.Node.ForwardItem);
            Assert.Null(buildings.Node.BackwardItem);
            Assert.Equal(buildings, buildings.ForwardNode.BackwardItem);
            Assert.Null(buildings.ForwardNode.ForwardItem);
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(buildings.Uid));
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(buildings.Uid));
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var buildings = Buildings.Add(map, new Vector3(10, 0, 10), new Vector3(-10, 0, -10), "bar");

            buildings.Move(new Vector3(30, 0, 30));

            Assert.Equal(new Vector3(30, 0, 30), buildings.Node.Position);
            Assert.Equal(new Vector3(10, 0, 10), buildings.ForwardNode.Position);
            Assert.Equal(0, buildings.ForwardNode.Sectors[0].X);
            Assert.Equal(0, buildings.ForwardNode.Sectors[0].Z);
            Assert.False(map.Sectors[(-1, -1)].MapItems.ContainsKey(buildings.Uid));
            Assert.True(map.Sectors[(0, 0)].MapItems.ContainsKey(buildings.Uid));
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var buildings = Buildings.Add(map, new Vector3(10, 0, 10), new Vector3(-10, 0, -10), "bar");

            buildings.Translate(new Vector3(20, 0, 20));

            Assert.Equal(new Vector3(30, 0, 30), buildings.Node.Position);
            Assert.Equal(new Vector3(10, 0, 10), buildings.ForwardNode.Position);
            Assert.Equal(0, buildings.ForwardNode.Sectors[0].X);
            Assert.Equal(0, buildings.ForwardNode.Sectors[0].Z);
            Assert.False(map.Sectors[(-1, -1)].MapItems.ContainsKey(buildings.Uid));
            Assert.True(map.Sectors[(0, 0)].MapItems.ContainsKey(buildings.Uid));
        }

        [Fact]
        public void DeleteIndividual()
        {
            var map = new Map("foo");
            var buildings = Buildings.Add(map, new Vector3(10, 0, 10), new Vector3(-10, 0, -10), "bar");

            map.Delete(buildings);

            Assert.False(map.HasItem(buildings.Uid));
            Assert.False(map.Sectors[(-1, -1)].MapItems.ContainsKey(buildings.Uid));
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(buildings.Uid));
            Assert.False(map.Nodes.ContainsKey(buildings.Node.Uid));
            Assert.False(map.Nodes.ContainsKey(buildings.ForwardNode.Uid));
        }
    }
}
