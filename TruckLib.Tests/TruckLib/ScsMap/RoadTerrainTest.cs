using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    public class RoadTerrainTest
    {
        [Fact]
        public void CalculateTerrainQuads()
        {
            var terrain = new RoadTerrain();
            terrain.Size = 10;

            terrain.CalculateQuadGrid(StepSize.Meters4, 7, false);
            Assert.Equal(2, terrain.QuadData.Cols);

            terrain.CalculateQuadGrid(StepSize.Meters4, 8, false);
            Assert.Equal(2, terrain.QuadData.Cols);

            terrain.CalculateQuadGrid(StepSize.Meters4, 9, false);
            Assert.Equal(3, terrain.QuadData.Cols);

            terrain.CalculateQuadGrid(StepSize.Meters12, 25, false);
            Assert.Equal(3, terrain.QuadData.Cols);
        }

        [Fact]
        public void CalculateTerrainQuadsAdaptive()
        {
            var terrain = new RoadTerrain();
            terrain.Size = 10;

            terrain.CalculateQuadGrid(StepSize.Meters2, 1.5f, true);
            Assert.Equal(8, terrain.QuadData.Cols);

            terrain.CalculateQuadGrid(StepSize.Meters16, 70f, true);
            Assert.Equal(3, terrain.QuadData.Cols);

            // The adaptive tessellation flag should be ignored for the following.
            // too long
            terrain.CalculateQuadGrid(StepSize.Meters2, 3f, true); 
            Assert.Equal(2, terrain.QuadData.Cols);

            // not active for this step size
            terrain.CalculateQuadGrid(StepSize.Meters4, 1.5f, true); 
            Assert.Equal(1, terrain.QuadData.Cols);

            // too short
            terrain.CalculateQuadGrid(StepSize.Meters16, 63f, true);
            Assert.Equal(4, terrain.QuadData.Cols);
        }
    }
}
