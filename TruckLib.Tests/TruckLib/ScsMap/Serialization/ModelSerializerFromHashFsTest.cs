﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib;
using TruckLib.HashFs;
using TruckLib.ScsMap;
using TruckLib.ScsMap.Serialization;

namespace TruckLib.Tests.TruckLib.ScsMap.Serialization
{
    public class ModelSerializerFromHashFsTest
    {   
        private readonly Map map;
        private readonly Model model;

        public ModelSerializerFromHashFsTest()
        {
            using var fs = HashFsReader.Open("Data/ModelSerializerTest/modeltest.scs");
            map = Map.Open("/map/modeltest.mbd", fs);
            model = (Model)map.MapItems.First().Value;
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
            Assert.Equal((Nibble)3, model.ColorVariant);

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
