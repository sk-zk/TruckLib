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

            Assert.True(map.HasItem(am.Uid));

            Assert.Equal("bar", am.Model);

            Assert.Equal(new Vector3(10, 0, 10), am.Node.Position);
            Assert.True(am.Node.IsRed);
            Assert.Equal(am, am.Node.ForwardItem);
            Assert.Null(am.Node.BackwardItem);
            Assert.True(am.Node.Sectors.Length == 1);
            Assert.Equal(0, am.Node.Sectors[0].X);
            Assert.Equal(0, am.Node.Sectors[0].Z);
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var am = AnimatedModel.Add(map, new Vector3(10, 0, 10), "bar");

            am.Move(new Vector3(-10, -20, -30));

            Assert.Equal(new Vector3(-10, -20, -30), am.Node.Position);
            Assert.True(am.Node.Sectors.Length == 1);
            Assert.Equal(-1, am.Node.Sectors[0].X);
            Assert.Equal(-1, am.Node.Sectors[0].Z);
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(am.Uid));
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(am.Uid));
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var am = AnimatedModel.Add(map, new Vector3(10, 0, 10), "bar");

            am.Translate(new Vector3(-20, -20, -40));

            Assert.Equal(new Vector3(-10, -20, -30), am.Node.Position);
            Assert.True(am.Node.Sectors.Length == 1);
            Assert.Equal(-1, am.Node.Sectors[0].X);
            Assert.Equal(-1, am.Node.Sectors[0].Z);
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(am.Uid));
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(am.Uid));
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var am = AnimatedModel.Add(map, new Vector3(10, 0, 10), "bar");

            map.Delete(am);

            Assert.False(map.HasItem(am.Uid));
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(am.Uid));
            Assert.False(map.Nodes.ContainsKey(am.Node.Uid));
        }
    }
}
