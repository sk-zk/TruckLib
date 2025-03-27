using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap.Collections
{
    public class CompanySpawnPointListTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var c = Company.Add(map, null, new Vector3(50, 0, 50));

            c.SpawnPoints.Add(new(69, 0, 42), Quaternion.Identity, CompanySpawnPointType.UnloadEasy);

            Assert.Single(c.SpawnPoints);

            Assert.Equal(CompanySpawnPointType.UnloadEasy, c.SpawnPoints[0].Type);

            Assert.Equal(new(69, 0, 42), c.SpawnPoints[0].Node.Position);
            Assert.Equal(Quaternion.Identity, c.SpawnPoints[0].Node.Rotation);
            Assert.True(map.Nodes.ContainsKey(c.SpawnPoints[0].Node.Uid));
            Assert.False(c.SpawnPoints[0].Node.IsRed);
            Assert.Equal(c, c.SpawnPoints[0].Node.ForwardItem);
        }

        [Fact]
        public void Insert()
        {
            var map = new Map("foo");
            var c = Company.Add(map, null, new Vector3(50, 0, 50));

            c.SpawnPoints.Add(new(69, 0, 42), Quaternion.Identity, CompanySpawnPointType.UnloadEasy);
            c.SpawnPoints.Insert(0, new(72, 0, 27), Quaternion.Identity, CompanySpawnPointType.UnloadEasy);

            Assert.Equal(2, c.SpawnPoints.Count);

            Assert.Equal(CompanySpawnPointType.UnloadEasy, c.SpawnPoints[0].Type);

            Assert.Equal(new(72, 0, 27), c.SpawnPoints[0].Node.Position);
            Assert.Equal(new(69, 0, 42), c.SpawnPoints[1].Node.Position);
            Assert.True(map.Nodes.ContainsKey(c.SpawnPoints[0].Node.Uid));
            Assert.False(c.SpawnPoints[0].Node.IsRed);
            Assert.Equal(c, c.SpawnPoints[0].Node.ForwardItem);
        }

        [Fact]
        public void RemoveAt()
        {
            var map = new Map("foo");
            var c = Company.Add(map, null, new Vector3(50, 0, 50));

            var p1 = c.SpawnPoints.Add(new(69, 0, 42), Quaternion.Identity, CompanySpawnPointType.UnloadEasy);
            var p2 = c.SpawnPoints.Add(new(72, 0, 27), Quaternion.Identity, CompanySpawnPointType.UnloadEasy);

            c.SpawnPoints.RemoveAt(0);
            Assert.Single(c.SpawnPoints);

            Assert.False(map.Nodes.ContainsKey(p1.Node.Uid));
        }

        [Fact]
        public void Clear()
        {
            var map = new Map("foo");
            var c = Company.Add(map, null, new Vector3(50, 0, 50));

            var p1 = c.SpawnPoints.Add(new(69, 0, 42), Quaternion.Identity, CompanySpawnPointType.UnloadEasy);
            var p2 = c.SpawnPoints.Add(new(72, 0, 27), Quaternion.Identity, CompanySpawnPointType.UnloadEasy);

            c.SpawnPoints.Clear();

            Assert.Empty(c.SpawnPoints);
            Assert.False(map.Nodes.ContainsKey(p1.Node.Uid));
            Assert.False(map.Nodes.ContainsKey(p2.Node.Uid));
        }
    }
}
