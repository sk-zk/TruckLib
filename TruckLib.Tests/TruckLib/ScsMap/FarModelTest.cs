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

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var fm = FarModel.Add(map, new Vector3(50, 0, 50), 60, 50);

            fm.Models.Add(new Vector3(69, 42, 0), "bar", Vector3.One);

            fm.Move(new Vector3(10, 20, 30));

            Assert.Equal(new Vector3(10, 20, 30), fm.Node.Position);
            Assert.Equal(new Vector3(29, 62, -20), fm.Models[0].Node.Position);
        }

        [Fact]
        public void GetCenterWithMapItems()
        {
            var map = new Map("foo");
            var fm = FarModel.Add(map, new Vector3(50, 0, 50), 60, 50);
            var model1 = Model.Add(map, new Vector3(10, 0, 10), "aaa", "bbb", "ccc");
            var model2 = Model.Add(map, new Vector3(-42, 0, -727), "aaa", "bbb", "ccc");

            fm.UseMapItems = true;
            fm.Children.Add(model1);
            fm.Children.Add(model2);

            // This should not influence the result because it is not used
            fm.Models.Add(new Vector3(69, 42, 0), "bar", Vector3.One);

            Assert.Equal(fm.Node.Position, fm.GetCenter());
        }

        [Fact]
        public void GetCenterWithFarModelItems()
        {
            var map = new Map("foo");
            var fm = FarModel.Add(map, new Vector3(50, 0, 50), 60, 50);
            var model1 = Model.Add(map, new Vector3(10, 0, 10), "aaa", "bbb", "ccc");
            var model2 = Model.Add(map, new Vector3(-42, 0, -727), "aaa", "bbb", "ccc");

            fm.Models.Add(new Vector3(69, 42, 0), "bar", Vector3.One);

            // These should not influence the result because they are not used
            fm.Children.Add(model1);
            fm.Children.Add(model2);

            Assert.Equal(new(59.5f, 21, 25), fm.GetCenter());
        }
    }
}
