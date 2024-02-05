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

            AssertEx.Equal(new Vector3(50, 0, 50), prefab.Nodes[0].Position, 0.001f);
            AssertEx.Equal(new Vector3(32, 0, 32), prefab.Nodes[1].Position, 0.001f);
            AssertEx.Equal(new Vector3(50, 0, 14), prefab.Nodes[2].Position, 0.001f);
            AssertEx.Equal(new Vector3(68, 0, 32), prefab.Nodes[3].Position, 0.001f);

            AssertEx.Equal(new Quaternion(0, 0, 0, -1), prefab.Nodes[0].Rotation, 0.001f);
            AssertEx.Equal(new Quaternion(0, 0.707107f, 0, -0.707107f), prefab.Nodes[1].Rotation, 0.001f);
            AssertEx.Equal(new Quaternion(0, 1, 0, 0), prefab.Nodes[2].Rotation, 0.001f);
            AssertEx.Equal(new Quaternion(0, 0.707107f, 0, 0.707107f), prefab.Nodes[3].Rotation, 0.001f);
        }

        [Fact]
        public void AddCrossingRotated()
        {
            var ppd = PrefabDescriptor.Open("Data/PrefabTest/blkw_r1_x_r1_narrow_tmpl.ppd");
            var map = new Map("foo");

            var prefab = Prefab.Add(map, new Vector3(50, 0, 50), "dlc_blkw_02", ppd, 
                Quaternion.CreateFromYawPitchRoll((float)(-90 * MathEx.DegToRad), 0, 0));

            AssertEx.Equal(new Vector3(50, 0, 50), prefab.Nodes[0].Position, 0.001f);
            AssertEx.Equal(new Vector3(68, 0, 32), prefab.Nodes[1].Position, 0.001f);
            AssertEx.Equal(new Vector3(86, 0, 50), prefab.Nodes[2].Position, 0.001f);
            AssertEx.Equal(new Vector3(68, 0, 68), prefab.Nodes[3].Position, 0.001f);

            AssertEx.Equal(new Quaternion(0, 0.707107f, 0, -0.707107f), prefab.Nodes[0].Rotation, 0.001f);
            AssertEx.Equal(new Quaternion(0, 1, 0, 0), prefab.Nodes[1].Rotation, 0.001f);
            AssertEx.Equal(new Quaternion(0, 0.707107f, 0, 0.707107f), prefab.Nodes[2].Rotation, 0.001f);
            AssertEx.Equal(new Quaternion(0, 0, 0, 1), prefab.Nodes[3].Rotation, 0.001f);

            Assert.True(prefab.Nodes[0].IsRed);
        }

        [Fact]
        public void AddCompany()
        {
            var ppd = PrefabDescriptor.Open("Data/PrefabTest/car_dealer_01_fr.ppd");
            var map = new Map("foo");

            var prefab = Prefab.Add(map, new Vector3(80, 0, 80), "dlc_fr_14", ppd);

            AssertEx.Equal(new Vector3(80, 0, 80), prefab.Nodes[0].Position, 0.001f);
            AssertEx.Equal(new Vector3(69.9453f, 0, 48.8359f), prefab.Nodes[1].Position, 0.001f);
        }

        [Fact]
        public void AddCompanyRotated()
        {
            var ppd = PrefabDescriptor.Open("Data/PrefabTest/car_dealer_01_fr.ppd");
            var map = new Map("foo");

            var prefab = Prefab.Add(map, new Vector3(80, 0, 80), "dlc_fr_14", ppd,
                Quaternion.CreateFromYawPitchRoll((float)(90 * MathEx.DegToRad), 0, 0));

            AssertEx.Equal(new Vector3(80, 0, 80), prefab.Nodes[0].Position, 0.001f);
            AssertEx.Equal(new Vector3(48.8359f, 0, 90.0547f), prefab.Nodes[1].Position, 0.001f);
            AssertEx.Equal(new Quaternion(0, 0.707107f, 0, 0.707107f), prefab.Nodes[0].Rotation);
            AssertEx.Equal(new Quaternion(0, 1, 0, 0), prefab.Nodes[1].Rotation);
        }
    }
}
