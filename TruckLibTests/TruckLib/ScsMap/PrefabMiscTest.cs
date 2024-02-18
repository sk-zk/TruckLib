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
    [Collection("Prefab collection")]
    public class PrefabMiscTest
    {
        PrefabFixture fixture;
        public PrefabMiscTest(PrefabFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void MoveCompany()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(80, 0, 80), "dlc_fr_14", fixture.CompanyPpd,
                Quaternion.CreateFromYawPitchRoll(MathEx.Rad(90f), 0, 0));

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
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(80, 0, 80), "dlc_fr_14", fixture.CompanyPpd,
                Quaternion.CreateFromYawPitchRoll(MathEx.Rad(90f), 0, 0));

            map.Delete(prefab);

            Assert.Empty(map.MapItems);
            Assert.Empty(map.Nodes);
        }

        [Fact]
        public void ChangeOrigin()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);

            AssertEx.Equal(new Vector3(50, 0, 50), prefab.Nodes[0].Position, 0.01f);
            Assert.True(prefab.Nodes[0].IsRed);

            prefab.ChangeOrigin(1);

            AssertEx.Equal(new Vector3(32, 0, 32), prefab.Nodes[0].Position, 0.01f);
            Assert.True(prefab.Nodes[0].IsRed);
            Assert.False(prefab.Nodes[1].IsRed);
        }

        [Fact]
        public void ChangeOriginBack()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);

            prefab.ChangeOrigin(2);
            prefab.ChangeOrigin(0);

            AssertEx.Equal(new Vector3(50, 0, 50), prefab.Nodes[0].Position, 0.01f);
        }

        [Fact]
        public void ChangeOriginThrowsIfNodeOccupied()
        {
            var map = new Map("foo");
            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);

            prefab.AppendRoad(2, new Vector3(10, 0, 10), "ger1");
            Assert.Throws<InvalidOperationException>(() => prefab.ChangeOrigin(2));
        }

        [Fact]
        public void MoveConnected()
        {
            var map = new Map("foo");
            var prefab1 = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", fixture.CrossingPpd);
            var prefab2 = Prefab.Add(map, new Vector3(100, 0, 55), "dlc_blkw_02", fixture.CrossingPpd);
            prefab2.ChangeOrigin(1);
            prefab1.Attach(3, prefab2, 0);

            var translation = new Vector3(42, 0, 42);
            var expected = prefab2.Nodes.Select(x => x.Position + translation).ToArray();

            prefab1.Translate(translation);

            var actual = prefab2.Nodes.Select(x => x.Position).ToArray();
            Assert.Equal(expected, actual);
        }
    }
}
