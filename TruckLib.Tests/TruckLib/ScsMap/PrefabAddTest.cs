using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;
using TruckLib;
using TruckLib.Models.Ppd;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    [Collection("Prefab collection")]
    public class PrefabAddTest
    {
        private PrefabFixture fixture;

        public PrefabAddTest(PrefabFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void AddCrossing()
        {
            var map = new Map();
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);

            var expectedPositions = new Vector3[] {
                new(50, 0, 50), new(32, 0, 32), new(50, 0, 14), new(68, 0, 32)
            };
            var expectedRotations = new Quaternion[] {
                new(0, 0, 0, -1), new(0, 0.707107f, 0, -0.707107f), new(0, 1, 0, 0), new(0, 0.707107f, 0, 0.707107f)
            };

            for (int i = 0; i < expectedPositions.Length; i++)
            {
                Assert.True(map.Nodes.ContainsKey(prefab.Nodes[i].Uid));
                AssertEx.Equal(expectedPositions[i], prefab.Nodes[i].Position, 0.001f);
                AssertEx.Equal(expectedRotations[i], prefab.Nodes[i].Rotation, 0.001f);
                Assert.Equal(i == 0, prefab.Nodes[i].IsRed);
                Assert.Equal(prefab, prefab.Nodes[i].ForwardItem);
                Assert.Null(prefab.Nodes[i].BackwardItem);
            }
        }

        [Fact]
        public void AddCrossingRotated()
        {
            var map = new Map();
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd,
                Quaternion.CreateFromYawPitchRoll(MathEx.Rad(-90f), 0, 0));

            var expectedPositions = new Vector3[] {
                new(50, 0, 50), new(68, 0, 32), new(86, 0, 50), new(68, 0, 68)
            };
            var expectedRotations = new Quaternion[] {
                new(0, 0.707107f, 0, -0.707107f), new(0, 1, 0, 0), new(0, 0.707107f, 0, 0.707107f), new(0, 0, 0, 1)
            };

            for (int i = 0; i < expectedPositions.Length; i++)
            {
                Assert.True(map.Nodes.ContainsKey(prefab.Nodes[i].Uid));
                AssertEx.Equal(expectedPositions[i], prefab.Nodes[i].Position, 0.001f);
                AssertEx.Equal(expectedRotations[i], prefab.Nodes[i].Rotation, 0.001f);
                Assert.Equal(i == 0, prefab.Nodes[i].IsRed);
            }
        }

        [Fact]
        public void AddCompany()
        {
            var map = new Map();
            var prefab = Prefab.Add(map, new Vector3(80, 0, 80), "dlc_fr_14", fixture.CompanyPpd);

            // test prefab item
            var expectedPositions = new Vector3[] {
                new (80, 0, 80), new(69.9453f, 0, 48.8359f)
            };
            var expectedRotations = new Quaternion[] {
                new(0, 0, 0, -1), new(0, 0.707107f, 0, 0.707107f)
            };

            for (int i = 0; i < expectedPositions.Length; i++)
            {
                Assert.True(map.Nodes.ContainsKey(prefab.Nodes[i].Uid));
                AssertEx.Equal(expectedPositions[i], prefab.Nodes[i].Position, 0.001f);
                AssertEx.Equal(expectedRotations[i], prefab.Nodes[i].Rotation, 0.001f);
                Assert.Equal(i == 0, prefab.Nodes[i].IsRed);
            }

            // test company item
            Assert.Single(prefab.SlaveItems);
            Assert.IsType<Company>(prefab.SlaveItems[0]);
            var company = prefab.SlaveItems[0] as Company;
            AssertEx.Equal(new Vector3(80.0352f, 0, 71.0938f), company.Node.Position, 0.01f);
            AssertEx.Equal(new Quaternion(0, -1, 0, 0), company.Node.Rotation, 0.001f);
            Assert.True(company.Node.IsRed);
            Assert.Equal(prefab, company.Prefab);

            var orderedSpawnPoints = company.SpawnPoints.OrderBy(x => x.Node.Position.X).ToList();
            expectedPositions = [
                new(45.5664f, 0, 23.1367f), new(56.7188f, 0, 43.4961f), new(57.0586f, 0, 34.0781f),
                new(57.7266f, 0, 29.1563f), new(92.3828f, 0, 54.582f)
            ];
            expectedRotations = [
               new(0, 0.707107f, 0, -0.707107f), new(0, 0.707107f, 0, -0.707107f), new(0, 0.707107f, 0, -0.707107f),
               new(0, 0.707107f, 0, -0.707107f), new(0, 0, 0, -1)
            ];
            for (int i = 0; i < expectedPositions.Length; i++)
            {
                Assert.True(map.Nodes.ContainsKey(orderedSpawnPoints[i].Node.Uid));
                AssertEx.Equal(expectedPositions[i], orderedSpawnPoints[i].Node.Position, 0.01f);
                AssertEx.Equal(expectedRotations[i], orderedSpawnPoints[i].Node.Rotation, 0.01f);
                Assert.False(orderedSpawnPoints[i].Node.IsRed);
            }
        }

        [Fact]
        public void AddCompanyRotated()
        {
            var map = new Map();
            var prefab = Prefab.Add(map, new Vector3(80, 0, 80), "dlc_fr_14", fixture.CompanyPpd,
                Quaternion.CreateFromYawPitchRoll(MathEx.Rad(90f), 0, 0));

            // test prefab item
            var expectedPositions = new Vector3[] {
                new (80, 0, 80), new(48.8359f, 0, 90.0547f)
            };
            var expectedRotations = new Quaternion[]{
                new(0, 0.707107f, 0, 0.707107f), new(0, 1, 0, 0)
            };

            for (int i = 0; i < expectedPositions.Length; i++)
            {
                Assert.True(map.Nodes.ContainsKey(prefab.Nodes[i].Uid));
                AssertEx.Equal(expectedPositions[i], prefab.Nodes[i].Position, 0.001f);
                AssertEx.Equal(expectedRotations[i], prefab.Nodes[i].Rotation, 0.001f);
                Assert.Equal(i == 0, prefab.Nodes[i].IsRed);
            }

            // test company item
            Assert.Single(prefab.SlaveItems);
            Assert.IsType<Company>(prefab.SlaveItems[0]);
            var company = prefab.SlaveItems[0] as Company;
            AssertEx.Equal(new Vector3(71.0938f, 0, 79.9648f), company.Node.Position, 0.01f);
            AssertEx.Equal(new Quaternion(0, 0.707107f, 0, -0.707107f), company.Node.Rotation, 0.001f);
            Assert.True(company.Node.IsRed);
            Assert.Equal(prefab, company.Prefab);

            var orderedSpawnPoints = company.SpawnPoints.OrderBy(x => x.Node.Position.X).ToList();
            expectedPositions = [
                new(23.1367f, 0, 114.434f), new(29.1563f, 0, 102.273f), new(34.0781f, 0, 102.941f),
                new(43.4961f, 0, 103.281f), new(54.582f, 0, 67.6172f),
            ];
            expectedRotations = [
                new(0, 0, 0, 1), new(0, 0, 0, 1), new(0, 0, 0, 1),
                new(0, 0, 0, 1), new(0, 0.707107f, 0, 0.707107f),
            ];
            for (int i = 0; i < expectedPositions.Length; i++)
            {
                Assert.True(map.Nodes.ContainsKey(orderedSpawnPoints[i].Node.Uid));
                AssertEx.Equal(expectedPositions[i], orderedSpawnPoints[i].Node.Position, 0.01f);
                AssertEx.Equal(expectedRotations[i], orderedSpawnPoints[i].Node.Rotation, 0.01f);
                Assert.False(orderedSpawnPoints[i].Node.IsRed);
            }
        }

        [Fact]
        public void AddServiceWithTerrain()
        {
            var map = new Map();
            var service = Prefab.Add(map, new(30, 0, 30), "bla", 
                fixture.ServicePpd, fixture.ServicePmd);

            service.Variant = "default";
            service.Look = "ger_1";

            service.PrefabNodes[0].Terrain.Size = 10;
            service.PrefabNodes[1].Terrain.Size = 20;
            service.PrefabNodes[2].Terrain.Size = 30;

            Assert.Equal(21, service.PrefabNodes[0].Terrain.QuadData.Cols);
            Assert.Equal(7, service.PrefabNodes[1].Terrain.QuadData.Cols);
            Assert.Equal(0, service.PrefabNodes[2].Terrain.QuadData.Cols);

            Assert.Equal(2, service.SlaveItems.Count);
            Assert.IsType<Service>(service.SlaveItems[0]);
            Assert.IsType<Service>(service.SlaveItems[1]);
            Assert.True(map.MapItems.ContainsKey(service.SlaveItems[0].Uid));
            Assert.True(map.MapItems.ContainsKey(service.SlaveItems[1].Uid));
            Assert.Equal(service, (service.SlaveItems[0] as Service).Prefab);
            Assert.Equal(service, (service.SlaveItems[1] as Service).Prefab);
            Assert.Equal(ServiceType.GasStation, (service.SlaveItems[0] as Service).ServiceType);
            Assert.Equal(ServiceType.GasStation, (service.SlaveItems[1] as Service).ServiceType);
        }
    }
}
