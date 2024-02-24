using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.Sii;

namespace TruckLibTests.TruckLib.Sii
{
    public class MatParserTest
    {
        [Fact]
        public void DeserializeFromString()
        {
            var str = File.ReadAllText("Data/MatParserTest/sample.mat");
            var file = MatParser.DeserializeFromString(str);

            Assert.Equal("eut2.dif.spec.mult.dif.iamod.dif.add.env.tsnmap.rfx", file.Effect);
            Assert.Equal(new Vector2(0.2f, 0.9f), file.Attributes["fresnel"]);
            Assert.Equal(25f, file.Attributes["shininess"]);
            Assert.Equal("texture_reflection", file.Textures[4].Name);
            Assert.Equal("/material/environment/building_reflection/building_ref.tobj", 
                file.Textures[4].Attributes["source"]);
            Assert.Equal("clamp", file.Textures[4].Attributes["u_address"]);
        }
    }
}
