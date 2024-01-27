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
    public class CityAreaTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var city = CityArea.Add(map, new Vector3(10, 0, 10), "bar", 50, 60);

            Assert.True(map.HasItem(city.Uid));

            Assert.Equal("bar", city.Name);
            Assert.Equal(50, city.Width);
            Assert.Equal(60, city.Height);

            Assert.Equal(new Vector3(10, 0, 10), city.Node.Position);
            Assert.True(city.Node.IsRed);
            Assert.Equal(city, city.Node.ForwardItem);
            Assert.Null(city.Node.BackwardItem);
            Assert.True(city.Node.Sectors.Length == 1);
            Assert.Equal(0, city.Node.Sectors[0].X);
            Assert.Equal(0, city.Node.Sectors[0].Z);
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var city = CityArea.Add(map, new Vector3(10, 0, 10), "bar", 50, 60);

            city.Move(new Vector3(-10, -20, -30));

            Assert.Equal(new Vector3(-10, -20, -30), city.Node.Position);
            Assert.True(city.Node.Sectors.Length == 1);
            Assert.Equal(-1, city.Node.Sectors[0].X);
            Assert.Equal(-1, city.Node.Sectors[0].Z);
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(city.Uid));
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(city.Uid));
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var city = CityArea.Add(map, new Vector3(10, 0, 10), "bar", 50, 60);

            city.Translate(new Vector3(-20, -20, -40));

            Assert.Equal(new Vector3(-10, -20, -30), city.Node.Position);
            Assert.True(city.Node.Sectors.Length == 1);
            Assert.Equal(-1, city.Node.Sectors[0].X);
            Assert.Equal(-1, city.Node.Sectors[0].Z);
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(city.Uid));
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(city.Uid));
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var city = CityArea.Add(map, new Vector3(10, 0, 10), "bar", 50, 60);

            map.Delete(city);

            Assert.False(map.HasItem(city.Uid));
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(city.Uid));
            Assert.False(map.Nodes.ContainsKey(city.Node.Uid));
        }
    }
}
