using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    public class MapAreaTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map();
            var points = new Vector3[] { new(30, 0, 10), new(45, 0, 30), new(10, 0, 30) };
            var area = MapArea.Add(map, points, MapAreaType.Visual);

            Assert.True(map.MapItems.ContainsKey(area.Uid));
            for (int i = 0; i < points.Length; i++)
            {
                Assert.True(map.Nodes.ContainsKey(area.Nodes[i].Uid));
                Assert.Equal(points[i], area.Nodes[i].Position);
                Assert.Equal(i == 0, area.Nodes[i].IsRed);
                Assert.Equal(area, area.Nodes[i].ForwardItem);
            }
        }

        [Fact]
        public void Move()
        {
            var map = new Map();
            var points = new Vector3[] { new(30, 0, 10), new(45, 0, 30), new(10, 0, 30) };
            var area = MapArea.Add(map, points, MapAreaType.Visual);

            var translation = new Vector3(10, 0, 10);
            area.Move(points[0] + translation);

            for (int i = 0; i < points.Length; i++)
            {
                Assert.Equal(points[i] + translation, area.Nodes[i].Position);
            }
        }

        [Fact]
        public void MoveWithDifferentAnchor()
        {
            var map = new Map();
            var points = new Vector3[] { new(30, 0, 10), new(45, 0, 30), new(10, 0, 30) };
            var area = MapArea.Add(map, points, MapAreaType.Visual);

            var translation = new Vector3(10, 0, 10);
            area.Move(points[1] + translation, 1);

            for (int i = 0; i < points.Length; i++)
            {
                Assert.Equal(points[i] + translation, area.Nodes[i].Position);
            }
        }

        [Fact]
        public void Translate()
        {
            var map = new Map();
            var points = new Vector3[] { new(30, 0, 10), new(45, 0, 30), new(10, 0, 30) };
            var area = MapArea.Add(map, points, MapAreaType.Visual);

            var translation = new Vector3(10, 0, 10);
            area.Translate(translation);

            for (int i = 0; i < points.Length; i++)
            {
                Assert.Equal(points[i] + translation, area.Nodes[i].Position);
            }
        }

        [Fact]
        public void Delete()
        {
            var map = new Map();
            var points = new Vector3[] { new(30, 0, 10), new(45, 0, 30), new(10, 0, 30) };
            var area = MapArea.Add(map, points, MapAreaType.Visual);

            map.Delete(area);

            Assert.Empty(map.MapItems);
            Assert.Empty(map.Nodes);
        }
    }
}
