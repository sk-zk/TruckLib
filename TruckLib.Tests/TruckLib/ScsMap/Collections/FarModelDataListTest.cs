using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLibTests.TruckLib.ScsMap.Collections
{
    public class FarModelDataListTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var fm = FarModel.Add(map, new Vector3(50, 0, 50), 60, 50);

            fm.Models.Add(new Vector3(69, 42, 0), "bar", Vector3.One);

            Assert.Single(fm.Models);

            Assert.Equal("bar", fm.Models[0].Model);
            Assert.Equal(Vector3.One, fm.Models[0].Scale);

            Assert.Equal(new Vector3(69, 42, 0), fm.Models[0].Node.Position);
            Assert.True(map.Nodes.ContainsKey(fm.Models[0].Node.Uid));
            Assert.False(fm.Models[0].Node.IsRed);
            Assert.Equal(fm, fm.Models[0].Node.ForwardItem);
        }

        [Fact]
        public void Insert()
        {
            var map = new Map("foo");
            var fm = FarModel.Add(map, new Vector3(50, 0, 50), 60, 50);

            fm.Models.Add(new Vector3(69, 42, 0), "bar", Vector3.One);
            fm.Models.Insert(0, new Vector3(12, 34, 56), "baz", Vector3.One);

            Assert.Equal(2, fm.Models.Count);
            Assert.Equal(new Vector3(12, 34, 56), fm.Models[0].Node.Position);
        }

        [Fact]
        public void RemoveAt()
        {
            var map = new Map("foo");
            var fm = FarModel.Add(map, new Vector3(50, 0, 50), 60, 50);

            fm.Models.Add(new Vector3(69, 42, 0), "bar", Vector3.One);
            var fmData = fm.Models[0];
            fm.Models.RemoveAt(0);

            Assert.False(map.Nodes.ContainsKey(fmData.Node.Uid));
        }

        [Fact]
        public void Clear()
        {
            var map = new Map("foo");
            var fm = FarModel.Add(map, new Vector3(50, 0, 50), 60, 50);

            fm.Models.Add(new Vector3(69, 42, 0), "bar", Vector3.One);
            var fmData1 = fm.Models[0];
            fm.Models.Add(new Vector3(12, 34, 56), "baz", Vector3.One);
            var fmData2 = fm.Models[1];
            fm.Models.Clear();

            Assert.Empty(fm.Models);
            Assert.False(map.Nodes.ContainsKey(fmData1.Node.Uid));
            Assert.False(map.Nodes.ContainsKey(fmData2.Node.Uid));
        }
    }
}
