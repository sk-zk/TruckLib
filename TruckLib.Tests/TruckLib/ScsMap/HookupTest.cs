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
    public class HookupTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var hookup = Hookup.Add(map, new Vector3(10, 0, 10), "bar");

            Assert.True(map.MapItems.ContainsKey(hookup.Uid));

            Assert.Equal("bar", hookup.Name);

            Assert.Equal(new Vector3(10, 0, 10), hookup.Node.Position);
            Assert.True(hookup.Node.IsRed);
            Assert.Equal(hookup, hookup.Node.ForwardItem);
            Assert.Null(hookup.Node.BackwardItem);
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var hookup = Hookup.Add(map, new Vector3(10, 0, 10), "bar");

            hookup.Move(new Vector3(-10, -20, -30));

            Assert.Equal(new Vector3(-10, -20, -30), hookup.Node.Position);
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var hookup = Hookup.Add(map, new Vector3(10, 0, 10), "bar");

            hookup.Translate(new Vector3(-20, -20, -40));

            Assert.Equal(new Vector3(-10, -20, -30), hookup.Node.Position);
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var hookup = Hookup.Add(map, new Vector3(10, 0, 10), "bar");

            map.Delete(hookup);

            Assert.Empty(map.MapItems);
            Assert.Empty(map.Nodes);
        }
    }
}
