using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class CurveTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var curve = Curve.Add(map, new Vector3(-15, 0, 35), new Vector3(35, 0, -15), "bar");

            Assert.Equal(curve.Model, "bar");
            Assert.Equal(70.71f, curve.Length, 0.01f);

            Assert.True(curve.Node.IsRed);
            Assert.False(curve.ForwardNode.IsRed);
            Assert.Equal(curve, curve.Node.ForwardItem);
            Assert.Null(curve.Node.BackwardItem);
            Assert.Equal(curve, curve.ForwardNode.BackwardItem);
            Assert.Null(curve.ForwardNode.ForwardItem);
        }

        [Fact]
        public void Append()
        {
            var map = new Map("foo");
            var curve1 = Curve.Add(map, new Vector3(20, 0, 20), new Vector3(10, 0, 10), "bar");

            // set some settings we expect to get cloned
            curve1.Stretch = 1.5f;
            curve1.MirrorReflection = false;
            curve1.FirstPart = "aaa";
            curve1.CenterPartVariation = "bbb";
            curve1.LastPart = "ccc";
            curve1.TerrainMaterial = "ddd";
            curve1.TerrainRotation = 2f;

            var curve2 = curve1.Append(new Vector3(-50, 0, -50), true);

            Assert.Equal(curve1.Stretch, curve2.Stretch);
            Assert.Equal(curve1.MirrorReflection, curve2.MirrorReflection);
            Assert.Equal(curve1.FirstPart, curve2.FirstPart);
            Assert.Equal(curve1.CenterPartVariation, curve2.CenterPartVariation);
            Assert.Equal(curve1.LastPart, curve2.LastPart);
            Assert.Equal(curve1.TerrainMaterial, curve2.TerrainMaterial);
            Assert.Equal(curve1.TerrainRotation, curve2.TerrainRotation);

            Assert.True(curve2.Node.IsRed);
            Assert.False(curve2.ForwardNode.IsRed);
            Assert.Equal(curve2, curve2.Node.ForwardItem);
            Assert.Equal(curve1, curve2.Node.BackwardItem);
            Assert.Equal(curve2, curve2.ForwardNode.BackwardItem);
            Assert.Null(curve2.ForwardNode.ForwardItem);
        }

        [Fact]
        public void DisallowAppendIfForwardItemExists()
        {
            var map = new Map("foo");
            var curve1 = Curve.Add(map, new Vector3(20, 0, 20), new Vector3(10, 0, 10), "bar");
            var curve2 = curve1.Append(new Vector3(-10, 0, -10), true);

            Assert.Throws<InvalidOperationException>(
                () => curve1.Append(new Vector3(-20, 0, -20)));
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var curve = Curve.Add(map, new Vector3(-20, 0, -20), new Vector3(-10, 0, -10), "bar");

            curve.Move(new Vector3(30, 0, 30));

            Assert.Equal(new Vector3(30, 0, 30), curve.Node.Position);
            Assert.Equal(new Vector3(40, 0, 40), curve.ForwardNode.Position);
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var curve = Curve.Add(map, new Vector3(-20, 0, -20), new Vector3(-10, 0, -10), "bar");

            curve.Translate(new Vector3(30, 0, 30));

            Assert.Equal(new Vector3(10, 0, 10), curve.Node.Position);
            Assert.Equal(new Vector3(20, 0, 20), curve.ForwardNode.Position);
        }

        [Fact]
        public void DeleteIndividual()
        {
            var map = new Map("foo");
            var curve = Curve.Add(map, new(10, 0, 10), new(30, 0, 10), "bar");
            curve.Locators.Add(new(10.29f, 0, 7.97f), Quaternion.Identity);
            curve.Locators.Add(new(29.71f, 0, 7.97f), Quaternion.Identity);

            map.Delete(curve);

            Assert.Empty(map.MapItems);
            Assert.Empty(map.Nodes);
        }

        [Fact]
        public void DeleteInChain()
        {
            var map = new Map("foo");
            var curve1 = Curve.Add(map, new Vector3(-10, 0, -10), new Vector3(10, 0, 10), "bar");
            var curve2 = curve1.Append(new Vector3(30, 0, 10));
            curve2.Locators.Add(new(10.29f, 0, 7.97f), Quaternion.Identity);
            var locator1 = curve2.Locators[0];
            curve2.Locators.Add(new(29.71f, 0, 7.97f), Quaternion.Identity);
            var locator2 = curve2.Locators[1];
            var curve3 = curve2.Append(new Vector3(40, 0, 40));

            map.Delete(curve2);

            Assert.False(map.MapItems.ContainsKey(curve2.Uid));
            Assert.Null(curve1.ForwardItem);
            Assert.Null(curve1.ForwardNode.ForwardItem);
            Assert.Null(curve3.BackwardItem);
            Assert.Null(curve3.Node.BackwardItem);
            Assert.False(map.Nodes.ContainsKey(locator1.Uid));
            Assert.False(map.Nodes.ContainsKey(locator2.Uid));
        }
    }
}
