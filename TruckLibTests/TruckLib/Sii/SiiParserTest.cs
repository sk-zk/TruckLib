using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib;
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

        [Fact]
        public void ParseFloats()
        {
            var unit = @"
                foo : .bar {
                    decimal_float: 1.0
                    hex_float: &3f800000
                    exponent: 1.312e3
                }
            ";
            var parser = new SiiParser();
            var file = parser.DeserializeFromString(unit);
            Assert.Equal(1.0f, file.Units[0].Attributes["decimal_float"]);
            Assert.Equal(1.0f, file.Units[0].Attributes["hex_float"]);
            Assert.Equal(1312f, file.Units[0].Attributes["exponent"]);
        }

        [Fact]
        public void ParseTuples()
        {
            var unit = @"
                foo : .bar {
                    float3: (1.0, 2.0, 3.0)
                    int3: (1, 2, 3)
                }
            ";
            var parser = new SiiParser();
            var file = parser.DeserializeFromString(unit);
            Assert.Equal(new[] { 1.0f, 2.0f, 3.0f }, file.Units[0].Attributes["float3"]);
            Assert.Equal(new[] { 1, 2, 3 }, file.Units[0].Attributes["int3"]);
        }

        [Fact]
        public void ParseToken()
        {
            var unit = @"
                foo : .bar {
                    valid_token: default 
                }
            ";
            var parser = new SiiParser();
            var file = parser.DeserializeFromString(unit);
            Assert.IsType<Token>(file.Units[0].Attributes["valid_token"]);
            Assert.Equal((Token)"default", file.Units[0].Attributes["valid_token"]);
        }

        [Fact]
        public void ParseBooleans()
        {
            var unit = @"
                foo : .bar {
                    a: true
                    b: false
                }
            ";
            var parser = new SiiParser();
            var file = parser.DeserializeFromString(unit);
            Assert.True(file.Units[0].Attributes["a"]);
            Assert.False(file.Units[0].Attributes["b"]);
        }

        [Fact]
        public void ParsePlacement()
        {
            var unit = @"
                foo : .bar {
                    a: (1, 2, 3) (4; 5, 6, 7)
                    b: (&c6b5d1a7, &41e27800, &c48e31db) (&3f29a17a; 0, &3f3fbb90, 0)
                }
            ";
            var parser = new SiiParser();
            var file = parser.DeserializeFromString(unit);

            Assert.Equal(new Vector3(1, 2, 3),
                file.Units[0].Attributes["a"].Position);
            Assert.Equal(new Quaternion(5, 6, 7, 4),
                file.Units[0].Attributes["a"].Rotation);

            Assert.Equal(new Vector3(-23272.826f, 28.308594f, -1137.558f),
                file.Units[0].Attributes["b"].Position);
            Assert.Equal(new Quaternion(0, 0.74895573f, 0, 0.66262019f),
                file.Units[0].Attributes["b"].Rotation);
        }

        [Fact]
        public void DontInsertIncludes()
        {
            var unit = @"
@include ""global_include.sui""
foo : .bar {
    some_val: 1
@include ""unit_include.sui""
}
            ";
            var parser = new SiiParser();
            var file = parser.DeserializeFromString(unit);
            Assert.True(file.Includes.Count == 1);
            Assert.Equal("global_include.sui", file.Includes[0]);
            Assert.True(file.Units[0].Includes.Count == 1);
            Assert.Equal("unit_include.sui", file.Units[0].Includes[0]);
        }
    }
}
