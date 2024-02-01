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

            Assert.True(map.MapItems.ContainsKey(point.Uid));

            Assert.Equal(new List<Token> { "bar" }, point.Tags);

            Assert.Equal(new Vector3(10, 0, 10), point.Node.Position);
            Assert.True(point.Node.IsRed);
            Assert.Equal(point, point.Node.ForwardItem);
            Assert.Null(point.Node.BackwardItem);
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var point = CameraPoint.Add(map, new Vector3(10, 0, 10), new List<Token>());

            point.Move(new Vector3(-10, -20, -30));

            Assert.Equal(new Vector3(-10, -20, -30), point.Node.Position);
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var point = CameraPoint.Add(map, new Vector3(10, 0, 10), new List<Token>());

            point.Translate(new Vector3(-20, -20, -40));

            Assert.Equal(new Vector3(-10, -20, -30), point.Node.Position);
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var point = CameraPoint.Add(map, new Vector3(10, 0, 10), new List<Token>());

            map.Delete(point);

            Assert.False(map.MapItems.ContainsKey(point.Uid));
            Assert.False(map.Nodes.ContainsKey(point.Node.Uid));
        }
    }
}
