using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;
using TruckLib.Models.Ppd;
using System.Numerics;
using TruckLib;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class PrefabTest
    {
        [Fact]
        public void AddCrossing()
        {
            var ppd = PrefabDescriptor.Open("Data/PrefabTest/blkw_r1_x_r1_narrow_tmpl.ppd");
            var map = new Map("foo");

            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", ppd);

            var expectedPositions = new Vector3[] {
                new(50, 0, 50), new(32, 0, 32), new(50, 0, 14), new(68, 0, 32)
            };
            var expectedRotations = new Quaternion[]{
                new(0, 0, 0, -1), new(0, 0.707107f, 0, -0.707107f), new(0, 1, 0, 0), new(0, 0.707107f, 0, 0.707107f)
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
        public void AddCrossingRotated()
        {
            var ppd = PrefabDescriptor.Open("Data/PrefabTest/blkw_r1_x_r1_narrow_tmpl.ppd");
            var map = new Map("foo");

            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", ppd, 
                Quaternion.CreateFromYawPitchRoll((float)(-90 * MathEx.DegToRad), 0, 0));

            var expectedPositions = new Vector3[] {
                new(50, 0, 50), new(68, 0, 32), new(86, 0, 50), new(68, 0, 68)
            };
            var expectedRotations = new Quaternion[]{
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
            var ppd = PrefabDescriptor.Open("Data/PrefabTest/car_dealer_01_fr.ppd");
            var map = new Map("foo");

            var prefab = Prefab.Add(map, new Vector3(80, 0, 80), "dlc_fr_14", ppd);

            // test prefab item
            var expectedPositions = new Vector3[] {
                new (80, 0, 80), new(69.9453f, 0, 48.8359f)
            };
            var expectedRotations = new Quaternion[]{
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
            Assert.Equal(prefab, company.PrefabLink);

            company.SpawnPoints = company.SpawnPoints.OrderBy(x => x.Node.Position.X).ToList();
            expectedPositions = new Vector3[] {
                new(45.5664f, 0, 23.1367f), new(56.7188f, 0, 43.4961f), new(57.0586f, 0, 34.0781f), 
                new(57.7266f, 0, 29.1563f), new(92.3828f, 0, 54.582f)
            };
            expectedRotations = new Quaternion[]{
               new(0, 0.707107f, 0, -0.707107f), new(0, 0.707107f, 0, -0.707107f), new(0, 0.707107f, 0, -0.707107f), 
               new(0, 0.707107f, 0, -0.707107f), new(0, 0, 0, -1)
            };
            for (int i = 0; i < expectedPositions.Length; i++)
            {
                Assert.True(map.Nodes.ContainsKey(company.SpawnPoints[i].Node.Uid));
                AssertEx.Equal(expectedPositions[i], company.SpawnPoints[i].Node.Position, 0.01f);
                AssertEx.Equal(expectedRotations[i], company.SpawnPoints[i].Node.Rotation, 0.01f);
                Assert.False(company.SpawnPoints[i].Node.IsRed);
            }
        }

        [Fact]
        public void AddCompanyRotated()
        {
            var ppd = PrefabDescriptor.Open("Data/PrefabTest/car_dealer_01_fr.ppd");
            var map = new Map("foo");

            var prefab = Prefab.Add(map, new Vector3(80, 0, 80), "dlc_fr_14", ppd,
                Quaternion.CreateFromYawPitchRoll((float)(90 * MathEx.DegToRad), 0, 0));

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
            Assert.Equal(prefab, company.PrefabLink);

            company.SpawnPoints = company.SpawnPoints.OrderBy(x => x.Node.Position.X).ToList();
            expectedPositions = new Vector3[] {
                new(23.1367f, 0, 114.434f), new(29.1563f, 0, 102.273f), new(34.0781f, 0, 102.941f), 
                new(43.4961f, 0, 103.281f), new(54.582f, 0, 67.6172f),
            };
            expectedRotations = new Quaternion[]{
                new(0, 0, 0, 1), new(0, 0, 0, 1), new(0, 0, 0, 1),
                new(0, 0, 0, 1), new(0, 0.707107f, 0, 0.707107f), 
            };
            for (int i = 0; i < expectedPositions.Length; i++)
            {
                Assert.True(map.Nodes.ContainsKey(company.SpawnPoints[i].Node.Uid));
                AssertEx.Equal(expectedPositions[i], company.SpawnPoints[i].Node.Position, 0.01f);
                AssertEx.Equal(expectedRotations[i], company.SpawnPoints[i].Node.Rotation, 0.01f);
                Assert.False(company.SpawnPoints[i].Node.IsRed);
            }
        }

        [Fact]
        public void MoveCompany()
        {
            var ppd = PrefabDescriptor.Open("Data/PrefabTest/car_dealer_01_fr.ppd");
            var map = new Map("foo");

            var prefab = Prefab.Add(map, new Vector3(80, 0, 80), "dlc_fr_14", ppd,
                Quaternion.CreateFromYawPitchRoll((float)(90 * MathEx.DegToRad), 0, 0));

            prefab.Move(new Vector3(90, 0, 90));

            AssertEx.Equal(new Vector3(90, 0, 90), prefab.Nodes[0].Position, 0.01f);
            AssertEx.Equal(new(58.8359f, 0, 100.0547f), prefab.Nodes[1].Position, 0.01f);

            var company = prefab.SlaveItems[0] as Company;
            company.SpawnPoints = company.SpawnPoints.OrderBy(x => x.Node.Position.X).ToList();
            AssertEx.Equal(new Vector3(81.0938f, 0, 89.9648f), company.Node.Position, 0.01f);
            AssertEx.Equal(new(33.1367f, 0, 124.434f), company.SpawnPoints[0].Node.Position, 0.01f);
        }

        [Fact]
        public void DeleteCompany()
        {
            var ppd = PrefabDescriptor.Open("Data/PrefabTest/car_dealer_01_fr.ppd");
            var map = new Map("foo");

            var prefab = Prefab.Add(map, new Vector3(80, 0, 80), "dlc_fr_14", ppd,
                Quaternion.CreateFromYawPitchRoll((float)(90 * MathEx.DegToRad), 0, 0));

            map.Delete(prefab);

            Assert.False(map.MapItems.ContainsKey(prefab.Uid));
            Assert.False(map.Nodes.ContainsKey(prefab.Nodes[0].Uid));
            Assert.False(map.Nodes.ContainsKey(prefab.Nodes[1].Uid));
            Assert.False(map.MapItems.ContainsKey(prefab.SlaveItems[0].Uid));
            Assert.False(map.Nodes.ContainsKey((prefab.SlaveItems[0] as Company).SpawnPoints[0].Node.Uid));
        }
    }
}
