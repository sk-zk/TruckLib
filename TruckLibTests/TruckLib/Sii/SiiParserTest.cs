using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.Sii;

namespace TruckLibTests.TruckLib.Sii
{
    public class SiiParserTest
    {
        [Fact]
        public void DeserializeFromString()
        {
            var str = File.ReadAllText("Data/SiiParserTest/sample.sii");
            var parser = new SiiParser();
            var file = parser.DeserializeFromString(str);

            Assert.True(file.GlobalScope);
            Assert.True(file.Units.Count == 1);
            TestUnit(file.Units[0]);
        }

        private void TestUnit(Unit unit)
        {
            Assert.NotNull(unit);
            Assert.Equal("curve_model", unit.Class);
            Assert.Equal("curve.ibe_1200i", unit.Name);

            Assert.Equal("/model2/fences/fence_01_ibe.pmd", unit.Attributes["model_desc"]);

            Assert.True(unit.Attributes["variation"].Length == 2);
            Assert.Equal("v1 | center2 : 1.0", unit.Attributes["variation"][1]);

            Assert.Equal(0, unit.Attributes["high_tess"]);
        }
    }
}
