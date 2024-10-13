using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib;
using TruckLib.ScsMap;
using TruckLib.ScsMap.Serialization;

namespace TruckLibTests.TruckLib.ScsMap.Serialization
{
    public class ModelSerializerTest
    {
        /*
        private readonly Sector sector;
        private readonly Model model;

        public ModelSerializerTest()
        {
            sector = new Sector();
            sector.ReadSector("Data/ModelSerializerTest/sec+0000+0000.base");
            model = (Model)sector.MapItems.First().Value;
        }

        [Fact]
        public void DeserializerTest()
        {
            TestProperties(model);
        }

        private void TestProperties(Model model)
        {
            Assert.Equal("ibe_0z001", model.Name);
            Assert.Equal("shop", model.Variant);
            Assert.Equal("default", model.Look);

            Assert.False(model.Shadows);
            Assert.True(model.WaterReflection);
            Assert.True(model.Collision);
            Assert.Equal((Nibble)2, model.ColorVariant);

            Assert.Equal(400, model.ViewDistance);
        }

        [Fact]
        public void SerializerTest()
        {
            var serializer = new ModelSerializer();

            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            serializer.Serialize(writer, model);
            memoryStream.Position = 0;

            using var reader = new BinaryReader(memoryStream);
            var clonedModel = (Model)serializer.Deserialize(reader);
            TestProperties(clonedModel);
        }
        */
    }
}
