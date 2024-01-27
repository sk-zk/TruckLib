using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib;
using TruckLib.ScsMap;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class CameraPointTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var point = CameraPoint.Add(map, new Vector3(10, 0, 10), new List<Token> { "bar" });

            Assert.True(map.HasItem(point.Uid));

            Assert.Equal(new List<Token> { "bar" }, point.Tags);

            Assert.Equal(new Vector3(10, 0, 10), point.Node.Position);
            Assert.True(point.Node.IsRed);
            Assert.Equal(point, point.Node.ForwardItem);
            Assert.Null(point.Node.BackwardItem);
            Assert.True(point.Node.Sectors.Length == 1);
            Assert.Equal(0, point.Node.Sectors[0].X);
            Assert.Equal(0, point.Node.Sectors[0].Z);
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var point = CameraPoint.Add(map, new Vector3(10, 0, 10), new List<Token>());

            point.Move(new Vector3(-10, -20, -30));

            Assert.Equal(new Vector3(-10, -20, -30), point.Node.Position);
            Assert.True(point.Node.Sectors.Length == 1);
            Assert.Equal(-1, point.Node.Sectors[0].X);
            Assert.Equal(-1, point.Node.Sectors[0].Z);
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(point.Uid));
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(point.Uid));
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var point = CameraPoint.Add(map, new Vector3(10, 0, 10), new List<Token>());

            point.Translate(new Vector3(-20, -20, -40));

            Assert.Equal(new Vector3(-10, -20, -30), point.Node.Position);
            Assert.True(point.Node.Sectors.Length == 1);
            Assert.Equal(-1, point.Node.Sectors[0].X);
            Assert.Equal(-1, point.Node.Sectors[0].Z);
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(point.Uid));
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(point.Uid));
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var point = CameraPoint.Add(map, new Vector3(10, 0, 10), new List<Token>());

            map.Delete(point);

            Assert.False(map.HasItem(point.Uid));
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(point.Uid));
            Assert.False(map.Nodes.ContainsKey(point.Node.Uid));
        }
    }
}
