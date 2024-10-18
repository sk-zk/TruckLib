using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    public class MapTest
    {
        [Fact]
        public void ThrowIfNameInvalid()
        {
            var map = new Map("foo");
            Assert.Throws<ArgumentNullException>(() => map.Name = null);
            Assert.Throws<ArgumentNullException>(() => map.Name = " ");
            Assert.Throws<ArgumentException>(() => map.Name = "a/b");
        }

        [Fact]
        public void AddSector()
        {
            var map = new Map("foo");
            var sector = map.AddSector(-5, 5);

            Assert.Single(map.Sectors);
            Assert.Equal(sector, map.Sectors[new SectorCoordinate(-5, 5)]);
            Assert.Equal(-5, sector.Coordinate.X);
            Assert.Equal(5, sector.Coordinate.Z);
        }

        [Fact]
        public void AddNode()
        {
            var map = new Map("foo");
            var node = map.AddNode(new(-69, 0, -420), true);

            Assert.Single(map.Nodes);
            Assert.Equal(node, map.Nodes[node.Uid]);
            Assert.Equal(new(-69, 0, -420), node.Position);
            Assert.True(node.IsRed);
            Assert.Single(map.Nodes.Within(-80, -440, -60, -400));

            Assert.Single(map.Sectors);
            Assert.True(map.Sectors.ContainsKey(new SectorCoordinate(-1, -1)));
        }

        [Fact]
        public void DeleteNode()
        {
            var map = new Map("foo");
            var node = map.AddNode(new(-69, 0, -420), true);

            map.Delete(node);

            Assert.Empty(map.Nodes);
            Assert.Empty(map.Nodes.Within(-1000, -1000, 1000, 1000));
        }

        [Fact]
        public void GetSectorOfCoordinate()
        {
            var expected = new SectorCoordinate(-1, -1);
            var actual = Map.GetSectorOfCoordinate(new(-100, 200, -300));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CompoundItems()
        {
            var map = new Map("foo");
            var model1 = Model.Add(map, new Vector3(22, 0, 16), "aaa", "bbb", "ccc");
            var model2 = Model.Add(map, new Vector3(30, 0, 8), "aaa", "bbb", "ccc");

            var compound = map.CompoundItems(new[] {model1, model2});

            Assert.True(compound.Node.IsRed);
            Assert.False(map.MapItems.ContainsKey(model1.Uid));
            Assert.False(map.MapItems.ContainsKey(model2.Uid));
            Assert.True(compound.MapItems.ContainsKey(model1.Uid));
            Assert.True(compound.MapItems.ContainsKey(model2.Uid));
            Assert.False(map.Nodes.ContainsKey(model1.Node.Uid));
            Assert.False(map.Nodes.ContainsKey(model2.Node.Uid));
            Assert.True(compound.Nodes.ContainsKey(model1.Node.Uid));
            Assert.True(compound.Nodes.ContainsKey(model2.Node.Uid));
            Assert.Equal(compound, model1.Node.Parent);
            Assert.Equal(compound, model2.Node.Parent);
        }

        [Fact]
        public void UncompoundItems()
        {
            var map = new Map("foo");
            var compound = Compound.Add(map, new Vector3(25, 0, 10));
            var model1 = Model.Add(compound, new Vector3(22, 0, 16), "aaa", "bbb", "ccc");
            var model2 = Model.Add(compound, new Vector3(30, 0, 8), "aaa", "bbb", "ccc");

            map.UncompoundItems(compound);

            Assert.False(map.MapItems.ContainsKey(compound.Uid));
            Assert.False(map.Nodes.ContainsKey(compound.Node.Uid));
            Assert.True(map.MapItems.ContainsKey(model1.Uid));
            Assert.True(map.MapItems.ContainsKey(model2.Uid));
            Assert.True(map.Nodes.ContainsKey(model1.Node.Uid));
            Assert.True(map.Nodes.ContainsKey(model2.Node.Uid));
            Assert.Equal(map, model1.Node.Parent);
            Assert.Equal(map, model2.Node.Parent);
        }
    }
}
