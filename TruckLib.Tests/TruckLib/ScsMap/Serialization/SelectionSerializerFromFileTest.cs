using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap.Serialization
{
    public class SelectionSerializerFromFileTest
    {
        private readonly Selection selection;

        public SelectionSerializerFromFileTest()
        {
            selection = Selection.Open("Data/SelectionSerializerTest/selectiontest.sbd");
        }

        [Fact]
        public void NodesAreCorrect()
        {
            Assert.Equal(3, selection.Nodes.Count);
            AssertEx.Equal(new(8.816406f, 0, 13.0625f), selection.Nodes[0x57c7438e8f000000].Position);
            AssertEx.Equal(new(15.0859375f, 0, 12.9570313f), selection.Nodes[0x57c743997c800000].Position);
            AssertEx.Equal(new(12.0039063f, 0, 7.21875f), selection.Nodes[0x57c743a4e5000000].Position);
        }

        [Fact]
        public void ModelsAreCorrect()
        {
            Assert.Equal(3, selection.MapItems.Count);
            Assert.Equal("dlc_no_660", ((Model)selection.MapItems[0x57c7438c38000001]).Name);
            Assert.Equal(selection.Nodes[0x57c7438e8f000000], ((Model)selection.MapItems[0x57c7438c38000001]).Node);
            // probably don't need to check the others
        }

        [Fact]
        public void OriginIsCorrect()
        {
            Assert.Equal(new(11.953125f, 0, 10.140625f), selection.Origin);
        }
    }
}
