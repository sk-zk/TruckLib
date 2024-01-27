using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.Model;
using TruckLib.ScsMap;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class VisibilityAreaTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var va = VisibilityArea.Add(map, new Vector3(10, 0, 10), VisibilityAreaBehavior.HideObjects, 50, 60);

            Assert.True(map.HasItem(va.Uid));

            Assert.Equal(VisibilityAreaBehavior.HideObjects, va.Behavior);
            Assert.Equal(50, va.Width);
            Assert.Equal(60, va.Height);

            Assert.Equal(new Vector3(10, 0, 10), va.Node.Position);
            Assert.True(va.Node.IsRed);
            Assert.Equal(va, va.Node.ForwardItem);
            Assert.Null(va.Node.BackwardItem);
            Assert.True(va.Node.Sectors.Length == 1);
            Assert.Equal(0, va.Node.Sectors[0].X);
            Assert.Equal(0, va.Node.Sectors[0].Z);
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var va = VisibilityArea.Add(map, new Vector3(10, 0, 10), VisibilityAreaBehavior.HideObjects, 50, 60);

            va.Move(new Vector3(-10, -20, -30));

            Assert.Equal(new Vector3(-10, -20, -30), va.Node.Position);
            Assert.True(va.Node.Sectors.Length == 1);
            Assert.Equal(-1, va.Node.Sectors[0].X);
            Assert.Equal(-1, va.Node.Sectors[0].Z);
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(va.Uid));
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(va.Uid));
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var va = VisibilityArea.Add(map, new Vector3(10, 0, 10), VisibilityAreaBehavior.HideObjects, 50, 60);

            va.Translate(new Vector3(-20, -20, -40));

            Assert.Equal(new Vector3(-10, -20, -30), va.Node.Position);
            Assert.True(va.Node.Sectors.Length == 1);
            Assert.Equal(-1, va.Node.Sectors[0].X);
            Assert.Equal(-1, va.Node.Sectors[0].Z);
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(va.Uid));
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(va.Uid));
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var va = VisibilityArea.Add(map, new Vector3(10, 0, 10), VisibilityAreaBehavior.HideObjects, 50, 60);

            map.Delete(va);

            Assert.False(map.HasItem(va.Uid));
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(va.Uid));
            Assert.False(map.Nodes.ContainsKey(va.Node.Uid));
        }
    }
}
