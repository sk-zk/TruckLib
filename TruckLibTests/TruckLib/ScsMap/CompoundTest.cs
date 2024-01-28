using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;
using TruckLib;
using System.Drawing;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class CompoundTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var compound = Compound.Add(map, new Vector3(10, 0, 10));

            Assert.True(map.HasItem(compound.Uid));

            Assert.Equal(new Vector3(10, 0, 10), compound.Node.Position);
            Assert.True(compound.Node.IsRed);
            Assert.Equal(compound, compound.Node.ForwardItem);
            Assert.Null(compound.Node.BackwardItem);
            Assert.True(compound.Node.Sectors.Length == 1);
            Assert.Equal(0, compound.Node.Sectors[0].X);
            Assert.Equal(0, compound.Node.Sectors[0].Z);
        }

        [Fact]
        public void AddToCompound()
        {
            var map = new Map("foo");
            var compound = Compound.Add(map, new Vector3(10, 0, 10));
            var model = Model.Add(compound, new Vector3(20, 0, 20), "aaa", "bbb", "ccc");

            Assert.Single(compound.Items);
            Assert.Equal(model, compound.Items[0]);
            Assert.Single(compound.Nodes);
            Assert.Equal(new Vector3(20, 0, 20), compound.Nodes[0].Position);
            Assert.Equal(model.Node, compound.Nodes[0]);
        }

        [Fact]
        public void DeleteFromCompound()
        {
            var map = new Map("foo");
            var compound = Compound.Add(map, new Vector3(10, 0, 10));
            var model = Model.Add(compound, new Vector3(20, 0, 20), "aaa", "bbb", "ccc");

            compound.Delete(model);

            Assert.Empty(compound.Items);
            Assert.Empty(compound.Nodes);
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var compound = Compound.Add(map, new Vector3(10, 0, 10));
            var model = Model.Add(compound, new Vector3(20, 0, 20), "aaa", "bbb", "ccc");

            compound.Move(new Vector3(-10, -20, -30));

            Assert.Equal(new Vector3(0, -20, -20), model.Node.Position);

            Assert.Equal(new Vector3(-10, -20, -30), compound.Node.Position);
            Assert.True(compound.Node.Sectors.Length == 1);
            Assert.Equal(-1, compound.Node.Sectors[0].X);
            Assert.Equal(-1, compound.Node.Sectors[0].Z);
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(compound.Uid));
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(compound.Uid));
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var compound = Compound.Add(map, new Vector3(10, 0, 10));
            var model = Model.Add(compound, new Vector3(20, 0, 20), "aaa", "bbb", "ccc");

            compound.Translate(new Vector3(-20, -20, -40));

            Assert.Equal(new Vector3(0, -20, -20), model.Node.Position);

            Assert.Equal(new Vector3(-10, -20, -30), compound.Node.Position);
            Assert.True(compound.Node.Sectors.Length == 1);
            Assert.Equal(-1, compound.Node.Sectors[0].X);
            Assert.Equal(-1, compound.Node.Sectors[0].Z);
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(compound.Uid));
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(compound.Uid));
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var compound = Compound.Add(map, new Vector3(10, 0, 10));
            Model.Add(compound, new Vector3(20, 0, 20), "aaa", "bbb", "ccc");

            map.Delete(compound);

            Assert.False(map.HasItem(compound.Uid));
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(compound.Uid));
            Assert.False(map.Nodes.ContainsKey(compound.Node.Uid));
        }
    }
}
