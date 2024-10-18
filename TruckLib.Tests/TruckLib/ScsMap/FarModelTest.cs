using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;
using System.Numerics;
using System.Drawing;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    public class FarModelTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var fm = FarModel.Add(map, new Vector3(50, 0, 50), 60, 50);

            Assert.Equal(new Vector3(50, 0, 50), fm.Node.Position);
            Assert.True(fm.Node.IsRed);
            Assert.Equal(fm, fm.Node.ForwardItem);
            Assert.Null(fm.Node.BackwardItem);
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var fm = FarModel.Add(map, new Vector3(50, 0, 50), 60, 50);

            fm.Models.Add(new Vector3(69, 42, 0), "bar", Vector3.One);
            var fmData = fm.Models[0];
            Assert.True(map.Nodes.ContainsKey(fmData.Node.Uid));

            map.Delete(fm);

            Assert.Empty(map.MapItems);
            Assert.Empty(map.Nodes);
        }
    }
}
