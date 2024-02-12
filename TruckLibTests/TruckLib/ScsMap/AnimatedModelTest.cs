using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class AnimatedModelTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var am = AnimatedModel.Add(map, new Vector3(10, 0, 10), "bar");

            Assert.True(map.MapItems.ContainsKey(am.Uid));

            Assert.Equal("bar", am.Model);

            Assert.Equal(new Vector3(10, 0, 10), am.Node.Position);
            Assert.True(am.Node.IsRed);
            Assert.Equal(am, am.Node.ForwardItem);
            Assert.Null(am.Node.BackwardItem);
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var am = AnimatedModel.Add(map, new Vector3(10, 0, 10), "bar");

            am.Move(new Vector3(-10, -20, -30));

            Assert.Equal(new Vector3(-10, -20, -30), am.Node.Position);
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var am = AnimatedModel.Add(map, new Vector3(10, 0, 10), "bar");

            am.Translate(new Vector3(-20, -20, -40));

            Assert.Equal(new Vector3(-10, -20, -30), am.Node.Position);
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var am = AnimatedModel.Add(map, new Vector3(10, 0, 10), "bar");

            map.Delete(am);

            Assert.Empty(map.MapItems);
            Assert.Empty(map.Nodes);
        }
    }
}
