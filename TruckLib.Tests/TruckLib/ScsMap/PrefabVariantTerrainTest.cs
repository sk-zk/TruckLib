using System.Collections.Generic;
using System.Numerics;
using TruckLib.Models;
using TruckLib.Models.Ppd;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    /// <summary>
    /// Selecting a model variant must not throw when a control node carries
    /// terrain points for only some of the prefab's variants. The installed
    /// gas station kits (e.g. gas_station_car_modern_01_blkw) define terrain
    /// points for their first variant index only, while the model offers more
    /// variants; before this was handled, selecting such a variant threw
    /// KeyNotFoundException from Prefab.UpdateTerrainCols.
    /// </summary>
    [Collection("Prefab collection")]
    public class PrefabVariantTerrainTest
    {
        [Fact]
        public void VariantWithoutTerrainPointsOnEveryNodeIsHandled()
        {
            var terrainPoints = new List<TerrainPoint>
            {
                new TerrainPoint(new Vector3(0, 0, 0), Vector3.UnitY),
                new TerrainPoint(new Vector3(0, 0, 1), Vector3.UnitY),
                new TerrainPoint(new Vector3(0, 0, 2), Vector3.UnitY),
            };
            var descriptor = new PrefabDescriptor
            {
                Nodes =
                [
                    new ControlNode { TerrainPoints = new() { [0] = terrainPoints } },
                    new ControlNode { TerrainPoints = new() { [0] = terrainPoints } },
                ]
            };
            var map = new Map();
            var prefab = Prefab.Add(map, Vector3.Zero, "test_prefab", descriptor);
            prefab.Variants = [new Variant("first"), new Variant("second")];

            prefab.Variant = "first";
            Assert.Equal(2, prefab.PrefabNodes[0].Terrain.QuadData.Cols);
            Assert.Equal(2, prefab.PrefabNodes[1].Terrain.QuadData.Cols);

            // The second variant index has no terrain points on any node.
            prefab.Variant = "second";
            Assert.Equal(0, prefab.PrefabNodes[0].Terrain.QuadData.Cols);
            Assert.Equal(0, prefab.PrefabNodes[1].Terrain.QuadData.Cols);

            // Switching back restores the terrain for the nodes that define it.
            prefab.Variant = "first";
            Assert.Equal(2, prefab.PrefabNodes[0].Terrain.QuadData.Cols);
        }
    }
}
