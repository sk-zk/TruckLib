using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    public class CameraPathTest
    {
        [Fact]
        public void Add()
        {
            var points = new List<Vector3>() {
                new(-13.11f, 0, 14.21f),
                new(-4.35f, 0, 3.08f),
                new(6.25f, 0, 3.16f),
                new(12.89f, 0, 14.29f)
            };

            var map = new Map();
            var path = CameraPath.Add(map, points);

            Assert.Equal(4, path.Keyframes.Count);

            Assert.Equal(4, path.Nodes.Count);
            for (int i = 0; i < path.Nodes.Count; i++)
            {
                Assert.Equal(points[i], path.Nodes[i].Position);
                Assert.Equal(path, path.Nodes[i].ForwardItem);
                Assert.Null(path.Nodes[i].BackwardItem);
                Assert.True(map.Nodes.ContainsKey(path.Nodes[i].Uid));
                Assert.Equal(Quaternion.Identity, path.Nodes[i].Rotation);
            }
            Assert.True(path.Nodes[0].IsRed);
            for (int i = 1; i < path.Nodes.Count; i++)
            {
                Assert.False(path.Nodes[i].IsRed);
            }
        }

        [Fact]
        public void Move()
        {
            var map = new Map();
            var path = CameraPath.Add(map, new List<Vector3>() {
                new(-30, 0, -30),
                new(-10, 0, -10),
            });

            path.Move(new Vector3(20, 0, 20));

            Assert.Equal(new Vector3(20, 0, 20), path.Nodes[0].Position);
            Assert.Equal(new Vector3(40, 0, 40), path.Nodes[1].Position);
        }

        [Fact]
        public void Translate()
        {
            var map = new Map();
            var path = CameraPath.Add(map, new List<Vector3>() {
                new(-30, 0, -30),
                new(-10, 0, -10),
            });

            path.Translate(new Vector3(50, 0, 50));

            Assert.Equal(new Vector3(20, 0, 20), path.Nodes[0].Position);
            Assert.Equal(new Vector3(40, 0, 40), path.Nodes[1].Position);
        }

        [Fact]
        public void Delete()
        {
            var map = new Map();
            var path = CameraPath.Add(map, new List<Vector3>() {
                new(-13.11f, 0, 14.21f),
                new(-4.35f, 0, 3.08f),
                new(6.25f, 0, 3.16f),
                new(12.89f, 0, 14.29f)
            });

            map.Delete(path);

            Assert.Empty(map.MapItems);
            Assert.Empty(map.Nodes);
        }
    }
}
