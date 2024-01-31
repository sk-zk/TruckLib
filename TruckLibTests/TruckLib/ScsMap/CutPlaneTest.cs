using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class CutPlaneTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var cutPlane = CutPlane.Add(map, new List<Vector3>()
            {
                new(10, 0, 10),
                new(-10, 0, -10),
                new(-30, 0, -20)
            });

            Assert.Equal(3, cutPlane.Nodes.Count);
            Assert.Equal(new Vector3(10, 0, 10), cutPlane.Nodes[0].Position);
            for (int i = 0; i < cutPlane.Nodes.Count; i++)
            {
                Assert.Equal(cutPlane, cutPlane.Nodes[i].ForwardItem);
                Assert.Null(cutPlane.Nodes[i].BackwardItem);
                Assert.Equal(Quaternion.Identity, cutPlane.Nodes[i].Rotation);
            }

            Assert.True(cutPlane.Nodes[0].IsRed);
            for (int i = 1; i < cutPlane.Nodes.Count; i++)
            {
                Assert.False(cutPlane.Nodes[i].IsRed);
            }
        }

        [Fact]
        public void Move()
        {
            var map = new Map("foo");
            var cutPlane = CutPlane.Add(map, new List<Vector3>() {
                new(-30, 0, -30),
                new(-10, 0, -10),
            });

            cutPlane.Move(new Vector3(20, 0, 20));

            Assert.Equal(new Vector3(20, 0, 20), cutPlane.Nodes[0].Position);
            Assert.Equal(new Vector3(40, 0, 40), cutPlane.Nodes[1].Position);
            Assert.Equal(0, cutPlane.Nodes[0].Sectors[0].X);
            Assert.Equal(0, cutPlane.Nodes[0].Sectors[0].Z);
            Assert.False(map.Sectors[(-1, -1)].MapItems.ContainsKey(cutPlane.Uid));
            Assert.True(map.Sectors[(0, 0)].MapItems.ContainsKey(cutPlane.Uid));
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var cutPlane = CutPlane.Add(map, new List<Vector3>() {
                new(-30, 0, -30),
                new(-10, 0, -10),
            });

            cutPlane.Translate(new Vector3(50, 0, 50));

            Assert.Equal(new Vector3(20, 0, 20), cutPlane.Nodes[0].Position);
            Assert.Equal(new Vector3(40, 0, 40), cutPlane.Nodes[1].Position);
            Assert.Equal(0, cutPlane.Nodes[0].Sectors[0].X);
            Assert.Equal(0, cutPlane.Nodes[0].Sectors[0].Z);
            Assert.False(map.Sectors[(-1, -1)].MapItems.ContainsKey(cutPlane.Uid));
            Assert.True(map.Sectors[(0, 0)].MapItems.ContainsKey(cutPlane.Uid));
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var cutPlane = CutPlane.Add(map, new List<Vector3>()
            {
                new(10, 0, 10),
                new(-10, 0, -10),
                new(-30, 0, -20)
            });

            map.Delete(cutPlane);

            Assert.False(map.HasItem(cutPlane.Uid));
            Assert.False(map.Sectors[(-1, -1)].MapItems.ContainsKey(cutPlane.Uid));
            for (int i = 0; i < cutPlane.Nodes.Count; i++)
            {
                Assert.False(map.Nodes.ContainsKey(cutPlane.Nodes[i].Uid));
            }
        }
    }
}
