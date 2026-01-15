using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap.Collections
{
    public class PolygonNodeListTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map();
            var points = new Vector3[] { new(30, 0, 10), new(45, 0, 30), new(10, 0, 30) };
            var area = MapArea.Add(map, points, MapAreaType.Visual);

            area.Nodes.Add(new Vector3(10, 0, 10));

            Assert.True(map.Nodes.ContainsKey(area.Nodes[3].Uid));
            Assert.Equal(new Vector3(10, 0, 10), area.Nodes[3].Position);
            Assert.False(area.Nodes[3].IsRed);
        }

        [Fact]
        public void InsertAtZero()
        {
            var map = new Map();
            var points = new Vector3[] { new(30, 0, 10), new(45, 0, 30), new(10, 0, 30) };
            var area = MapArea.Add(map, points, MapAreaType.Visual);

            area.Nodes.Insert(0, new Vector3(10, 0, 10));

            Assert.True(map.Nodes.ContainsKey(area.Nodes[0].Uid));
            Assert.Equal(new Vector3(10, 0, 10), area.Nodes[0].Position);
            Assert.True(area.Nodes[0].IsRed);
            Assert.False(area.Nodes[1].IsRed);
        }

        [Fact]
        public void Remove()
        {
            var map = new Map();
            var points = new Vector3[] { new(30, 0, 10), new(45, 0, 30), new(10, 0, 30), new(10, 0, 10) };
            var area = MapArea.Add(map, points, MapAreaType.Visual);

            var nodeToRemove = area.Nodes[1];
            area.Nodes.Remove(nodeToRemove);

            Assert.False(map.Nodes.ContainsKey(nodeToRemove.Uid));
            Assert.Equal(-1, area.Nodes.IndexOf(nodeToRemove));
        }

        [Fact]
        public void RemoveAt()
        {
            var map = new Map();
            var points = new Vector3[] { new(30, 0, 10), new(45, 0, 30), new(10, 0, 30), new(10, 0, 10) };
            var area = MapArea.Add(map, points, MapAreaType.Visual);

            var nodeToRemove = area.Nodes[1];
            area.Nodes.RemoveAt(1);

            Assert.False(map.Nodes.ContainsKey(nodeToRemove.Uid));
            Assert.Equal(-1, area.Nodes.IndexOf(nodeToRemove));
        }

        [Fact]
        public void RemoveZero()
        {
            var map = new Map();
            var points = new Vector3[] { new(30, 0, 10), new(45, 0, 30), new(10, 0, 30), new(10, 0, 10) };
            var area = MapArea.Add(map, points, MapAreaType.Visual);

            var nodeToRemove = area.Nodes[0];
            area.Nodes.RemoveAt(0);

            Assert.Equal(-1, area.Nodes.IndexOf(nodeToRemove));
            Assert.True(area.Nodes[0].IsRed);
        }

        [Fact]
        public void ClearThenReadd()
        {
            var map = new Map();
            var points = new Vector3[] { new(30, 0, 10), new(45, 0, 30), new(10, 0, 30), new(10, 0, 10) };
            var area = MapArea.Add(map, points, MapAreaType.Visual);

            area.Nodes.Clear();

            Assert.Empty(area.Nodes);
            Assert.Empty(map.Nodes);

            area.Nodes.Add(new Vector3(42, 0, 42));
            Assert.Equal(new Vector3(42, 0, 42), area.Nodes[0].Position);
            Assert.True(map.Nodes.ContainsKey(area.Nodes[0].Uid));
        }
    }
}
