using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TruckLib.Model;
using TruckLib.ScsMap;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class BuildingsTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var buildings = Buildings.Add(map, new Vector3(-15, 0, 35), new Vector3(35, 0, -15), "bar");

            Assert.Equal(buildings.Name, "bar");
            Assert.Equal(70.71f, buildings.Length, 0.01f);

            Assert.True(buildings.Node.IsRed);
            Assert.False(buildings.ForwardNode.IsRed);
            Assert.Equal(buildings, buildings.Node.ForwardItem);
            Assert.Null(buildings.Node.BackwardItem);
            Assert.Equal(buildings, buildings.ForwardNode.BackwardItem);
            Assert.Null(buildings.ForwardNode.ForwardItem);
            Assert.True(map.Sectors[(0, 0)].MapItems.ContainsKey(buildings.Uid));
            Assert.False(map.Sectors[(-1, 0)].MapItems.ContainsKey(buildings.Uid));
            Assert.False(map.Sectors[(0, -1)].MapItems.ContainsKey(buildings.Uid));
        }

        [Fact]
        public void Append()
        {
            var map = new Map("foo");
            var buildings1 = Buildings.Add(map, new Vector3(20, 0, 20), new Vector3(10, 0, 10), "bar");

            // set some settings we expect to get cloned
            buildings1.Stretch = 1.5f;
            buildings1.MirrorReflection = false;
            buildings1.ColorVariant = 2;

            var buildings2 = buildings1.Append(new Vector3(-50, 0, -50), true);

            Assert.Equal(buildings1.Stretch, buildings2.Stretch);
            Assert.Equal(buildings1.MirrorReflection, buildings2.MirrorReflection);
            Assert.Equal(buildings1.ColorVariant, buildings2.ColorVariant);

            Assert.True(buildings2.Node.IsRed);
            Assert.False(buildings2.ForwardNode.IsRed);
            Assert.Equal(buildings2, buildings2.Node.ForwardItem);
            Assert.Equal(buildings1, buildings2.Node.BackwardItem);
            Assert.Equal(buildings2, buildings2.ForwardNode.BackwardItem);
            Assert.Null(buildings2.ForwardNode.ForwardItem);
            Assert.True(map.Sectors[(-1, -1)].MapItems.ContainsKey(buildings2.Uid));
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(buildings2.Uid));
        }

        [Fact]
        public void DisallowAppendIfForwardItemExists()
        {
            var map = new Map("foo");
            var buildings1 = Buildings.Add(map, new Vector3(20, 0, 20), new Vector3(10, 0, 10), "bar");
            var buildings2 = buildings1.Append(new Vector3(-10, 0, -10), true);

            Assert.Throws<InvalidOperationException>(
                () => buildings1.Append(new Vector3(-20, 0, -20)));
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var buildings = Buildings.Add(map, new Vector3(-20, 0, -20), new Vector3(-10, 0, -10), "bar");

            buildings.Move(new Vector3(30, 0, 30));

            Assert.Equal(new Vector3(30, 0, 30), buildings.Node.Position);
            Assert.Equal(new Vector3(40, 0, 40), buildings.ForwardNode.Position);
            Assert.Equal(0, buildings.ForwardNode.Sectors[0].X);
            Assert.Equal(0, buildings.ForwardNode.Sectors[0].Z);
            Assert.False(map.Sectors[(-1, -1)].MapItems.ContainsKey(buildings.Uid));
            Assert.True(map.Sectors[(0, 0)].MapItems.ContainsKey(buildings.Uid));
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var buildings = Buildings.Add(map, new Vector3(-20, 0, -20), new Vector3(-10, 0, -10), "bar");

            buildings.Translate(new Vector3(30, 0, 30));

            Assert.Equal(new Vector3(10, 0, 10), buildings.Node.Position);
            Assert.Equal(new Vector3(20, 0, 20), buildings.ForwardNode.Position);
            Assert.Equal(0, buildings.ForwardNode.Sectors[0].X);
            Assert.Equal(0, buildings.ForwardNode.Sectors[0].Z);
            Assert.False(map.Sectors[(-1, -1)].MapItems.ContainsKey(buildings.Uid));
            Assert.True(map.Sectors[(0, 0)].MapItems.ContainsKey(buildings.Uid));
        }

        [Fact]
        public void DeleteIndividual()
        {
            var map = new Map("foo");
            var buildings = Buildings.Add(map, new Vector3(10, 0, 10), new Vector3(-10, 0, -10), "bar");

            map.Delete(buildings);

            Assert.False(map.HasItem(buildings.Uid));
            Assert.False(map.Sectors[(-1, -1)].MapItems.ContainsKey(buildings.Uid));
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(buildings.Uid));
            Assert.False(map.Nodes.ContainsKey(buildings.Node.Uid));
            Assert.False(map.Nodes.ContainsKey(buildings.ForwardNode.Uid));
        }

        [Fact]
        public void DeleteInChain()
        {
            var map = new Map("foo");
            var buildings1 = Buildings.Add(map, new Vector3(10, 0, 10), new Vector3(20, 0, 20), "bar");
            var buildings2 = buildings1.Append(new Vector3(30, 0, 30));
            var buildings3 = buildings2.Append(new Vector3(40, 0, 40));

            map.Delete(buildings2);

            Assert.False(map.HasItem(buildings2.Uid));
            Assert.Null(buildings1.ForwardItem);
            Assert.Null(buildings1.ForwardNode.ForwardItem);
            Assert.Null(buildings3.BackwardItem);
            Assert.Null(buildings3.Node.BackwardItem);
        }
    }
}
