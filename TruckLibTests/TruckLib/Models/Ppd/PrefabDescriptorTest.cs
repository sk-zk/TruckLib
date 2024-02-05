using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.Models.Ppd;

namespace TruckLibTests.TruckLib.Models.Ppd
{
    public class PrefabDescriptorTest
    {
        [Fact]
        public void OpenCrossing()
        {
            var ppd = PrefabDescriptor.Open("Data/PrefabDescriptorTest/blkw_r1_x_r1_narrow_tmpl.ppd.17");

            Assert.Equal(0x17u, ppd.Version);

            Assert.Equal(4, ppd.Nodes.Count);
            Assert.Equal(new Vector3(-18f, 0, 0), ppd.Nodes[0].Position);
            Assert.Equal(1f, ppd.Nodes[0].Direction.X, 0.0001f);
            Assert.Equal(0f, ppd.Nodes[0].Direction.Y, 0.0001f);
            Assert.Equal(0f, ppd.Nodes[0].Direction.Z, 0.0001f);

            // TODO test all the other stuff.
            // I don't know a whole lot about how their model files work,
            // so I'm focusing on the parts I need for mapping
        }
    }
}
