using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    public class ModelTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var model = Model.Add(map, new Vector3(10, 0, 10), "aaa", "bbb", "ccc");

            Assert.True(map.MapItems.ContainsKey(model.Uid));

            Assert.Equal("aaa", model.Name);
            Assert.Equal("bbb", model.Variant);
            Assert.Equal("ccc", model.Look);

            Assert.Equal(new Vector3(10, 0, 10), model.Node.Position);
            Assert.True(model.Node.IsRed);
            Assert.Equal(model, model.Node.ForwardItem);
            Assert.Null(model.Node.BackwardItem);
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var model = Model.Add(map, new Vector3(10, 0, 10), "aaa", "bbb", "ccc");

            model.Move(new Vector3(-10, -20, -30));

            Assert.Equal(new Vector3(-10, -20, -30), model.Node.Position);
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var model = Model.Add(map, new Vector3(10, 0, 10), "aaa", "bbb", "ccc");

            model.Translate(new Vector3(-20, -20, -40));

            Assert.Equal(new Vector3(-10, -20, -30), model.Node.Position);
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var model = Model.Add(map, new Vector3(10, 0, 10), "aaa", "bbb", "ccc");

            map.Delete(model);

            Assert.Empty(map.MapItems);
            Assert.Empty(map.Nodes);
        }
    }
}
