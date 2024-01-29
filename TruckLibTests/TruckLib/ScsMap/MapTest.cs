using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class MapTest
    {
        [Fact]
        public void ThrowIfNameInvalid()
        {
            var map = new Map("foo");
            Assert.Throws<ArgumentNullException>(() => map.Name = null);
            Assert.Throws<ArgumentNullException>(() => map.Name = " ");
            Assert.Throws<ArgumentException>(() => map.Name = "a/b");
        }

        [Fact]
        public void HasItem()
        {
            var map = new Map("foo");
            var model = Model.Add(map, new Vector3(69, 42, 0), "aaa", "bbb", "ccc");

            Assert.True(map.HasItem(model.Uid));
            Assert.False(map.HasItem(12345678));
        }

        [Fact]
        public void GetItem()
        {
            var map = new Map("foo");
            var model = Model.Add(map, new Vector3(69, 42, 0), "aaa", "bbb", "ccc");

            Assert.Equal(model, map.GetItem(model.Uid));
            Assert.Null(map.GetItem(12345678));
        }

        [Fact]
        public void TryGetItem()
        {
            var map = new Map("foo");
            var model = Model.Add(map, new Vector3(69, 42, 0), "aaa", "bbb", "ccc");

            Assert.True(map.TryGetItem(model.Uid, out var _model));
            Assert.Equal(model, _model);
            Assert.False(map.TryGetItem(12345678, out var nothing));
            Assert.Null(nothing);
        }
    }
}
