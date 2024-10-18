using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    public class VisibilityAreaTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var va = VisibilityArea.Add(map, new Vector3(10, 0, 10), VisibilityAreaBehavior.HideObjects, 50, 60);

            Assert.True(map.MapItems.ContainsKey(va.Uid));

            Assert.Equal(VisibilityAreaBehavior.HideObjects, va.Behavior);
            Assert.Equal(50, va.Width);
            Assert.Equal(60, va.Height);

            Assert.Equal(new Vector3(10, 0, 10), va.Node.Position);
            Assert.True(va.Node.IsRed);
            Assert.Equal(va, va.Node.ForwardItem);
            Assert.Null(va.Node.BackwardItem);
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var va = VisibilityArea.Add(map, new Vector3(10, 0, 10), VisibilityAreaBehavior.HideObjects, 50, 60);

            va.Move(new Vector3(-10, -20, -30));

            Assert.Equal(new Vector3(-10, -20, -30), va.Node.Position);
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var va = VisibilityArea.Add(map, new Vector3(10, 0, 10), VisibilityAreaBehavior.HideObjects, 50, 60);

            va.Translate(new Vector3(-20, -20, -40));

            Assert.Equal(new Vector3(-10, -20, -30), va.Node.Position);
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var va = VisibilityArea.Add(map, new Vector3(10, 0, 10), VisibilityAreaBehavior.HideObjects, 50, 60);

            map.Delete(va);

            Assert.Empty(map.MapItems);
            Assert.Empty(map.Nodes);
        }
    }
}
