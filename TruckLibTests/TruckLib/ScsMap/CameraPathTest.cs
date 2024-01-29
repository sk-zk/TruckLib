﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class CameraPathTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var path = CameraPath.Add(map, new List<Vector3>() { 
                new(-13.11f, 0, 14.21f),
                new(-4.35f, 0, 3.08f),
                new(6.25f, 0, 3.16f),
                new(12.89f, 0, 14.29f)
            });

            Assert.Equal(4, path.Keyframes.Count);

            Assert.Equal(4, path.Nodes.Count);
            Assert.Equal(new Vector3(-13.11f, 0, 14.21f), path.Nodes[0].Position);
            Assert.Equal(path.Nodes[^1], path.GetMainNode());
            for (int i = 0; i < path.Nodes.Count; i++)
            {
                Assert.Equal(path, path.Nodes[i].ForwardItem);
                Assert.Null(path.Nodes[i].BackwardItem);
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
            var map = new Map("foo");
            var path = CameraPath.Add(map, new List<Vector3>() {
                new(-30, 0, -30),
                new(-10, 0, -10),
            });

            path.Move(new Vector3(20, 0, 20));

            Assert.Equal(new Vector3(20, 0, 20), path.Nodes[0].Position);
            Assert.Equal(new Vector3(40, 0, 40), path.Nodes[1].Position);
            Assert.Equal(0, path.Nodes[0].Sectors[0].X);
            Assert.Equal(0, path.Nodes[0].Sectors[0].Z);
            Assert.False(map.Sectors[(-1, -1)].MapItems.ContainsKey(path.Uid));
            Assert.True(map.Sectors[(0, 0)].MapItems.ContainsKey(path.Uid));
        }

        [Fact]
        public void Translate()
        {
            var map = new Map("foo");
            var path = CameraPath.Add(map, new List<Vector3>() {
                new(-30, 0, -30),
                new(-10, 0, -10),
            });

            path.Translate(new Vector3(50, 0, 50));

            Assert.Equal(new Vector3(20, 0, 20), path.Nodes[0].Position);
            Assert.Equal(new Vector3(40, 0, 40), path.Nodes[1].Position);
            Assert.Equal(0, path.Nodes[0].Sectors[0].X);
            Assert.Equal(0, path.Nodes[0].Sectors[0].Z);
            Assert.False(map.Sectors[(-1, -1)].MapItems.ContainsKey(path.Uid));
            Assert.True(map.Sectors[(0, 0)].MapItems.ContainsKey(path.Uid));
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var path = CameraPath.Add(map, new List<Vector3>() {
                new(-13.11f, 0, 14.21f),
                new(-4.35f, 0, 3.08f),
                new(6.25f, 0, 3.16f),
                new(12.89f, 0, 14.29f)
            });

            map.Delete(path);

            Assert.False(map.HasItem(path.Uid));
            Assert.False(map.Sectors[(0, 0)].MapItems.ContainsKey(path.Uid));
            for (int i = 0; i < path.Nodes.Count; i++)
            {
                Assert.False(map.Nodes.ContainsKey(path.Nodes[i].Uid));
            }
        }
    }
}