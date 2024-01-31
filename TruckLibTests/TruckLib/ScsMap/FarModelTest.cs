using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;
using System.Numerics;
using System.Drawing;

namespace TruckLibTests.TruckLib.ScsMap
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
            Assert.True(fm.Node.Sectors.Length == 1);
            Assert.Equal(0, fm.Node.Sectors[0].X);
            Assert.Equal(0, fm.Node.Sectors[0].Z);
        }

    }

}
