using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class ModelTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var model = Model.Add(map, new Vector3(10, 0, 10), "aaa", "bbb", "ccc");

            Assert.True(map.HasItem(model.Uid));

            Assert.Equal("aaa", model.Name);
            Assert.Equal("bbb", model.Variant);
            Assert.Equal("ccc", model.Look);

            Assert.Equal(new Vector3(10, 0, 10), model.Node.Position);
            Assert.True(model.Node.IsRed);
            Assert.Equal(model, model.Node.ForwardItem);
            Assert.Null(model.Node.BackwardItem);
            Assert.True(model.Node.Sectors.Length == 1);
            Assert.Equal(0, model.Node.Sectors[0].X);
            Assert.Equal(0, model.Node.Sectors[0].Z);
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var model = Model.Add(map, new Vector3(10, 0, 10), "aaa", "bbb", "ccc");

            model.Move(new Vector3(-10, -20, -30));

            Assert.Equal(new Vector3(-10, -20, -30), model.Node.Position);
            Assert.True(model.Node.Sectors.Length == 1);
            Assert.Equal(-1, model.Node.Sectors[0].X);
            Assert.Equal(-1, model.Node.Sectors[0].Z);
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(model.Uid));
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(model.Uid));
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var model = Model.Add(map, new Vector3(10, 0, 10), "aaa", "bbb", "ccc");

            model.Translate(new Vector3(-20, -20, -40));

            Assert.Equal(new Vector3(-10, -20, -30), model.Node.Position);
            Assert.True(model.Node.Sectors.Length == 1);
            Assert.Equal(-1, model.Node.Sectors[0].X);
            Assert.Equal(-1, model.Node.Sectors[0].Z);
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(model.Uid));
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(model.Uid));
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var model = Model.Add(map, new Vector3(10, 0, 10), "aaa", "bbb", "ccc");

            map.Delete(model);

            Assert.False(map.HasItem(model.Uid));
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(model.Uid));
            Assert.False(map.Nodes.ContainsKey(model.Node.Uid));
        }
    }
}
