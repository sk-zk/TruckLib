using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib;
using TruckLib.ScsMap;
using TruckLib.ScsMap.Serialization;

namespace TruckLib.Tests.TruckLib.ScsMap.Serialization
{
    public class ModelSerializerTest
    {   
        private readonly Map map;
        private readonly Model model;

        public ModelSerializerTest()
        {
            map = Map.Open("Data/ModelSerializerTest/modeltest.mbd");
            model = (Model)map.MapItems.First().Value;
        }

        [Fact]
        public void DeserializerTest()
        {
            TestProperties(model);
        }

        private void TestProperties(Model model)
        {
            Assert.Equal("ar_5e001", model.Name);
            Assert.Equal("small", model.Variant);
            Assert.Equal("rusty_white", model.Look);

            Assert.False(model.Shadows);
            Assert.True(model.WaterReflection);
            Assert.True(model.Collision);

            Assert.Contains("_part04", model.AdditionalParts);
            Assert.Contains("_part05", model.AdditionalParts);

            Assert.Equal(720, model.ViewDistance);
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
    }
}
