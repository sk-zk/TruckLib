using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class ModelSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var model = new Model();
            ReadKdop(r, model);

            model.Name = r.ReadToken();
            model.Look = r.ReadToken();
            model.Variant = r.ReadToken();
            
            model.AdditionalParts = ReadObjectList<Token>(r);
            
            model.Node = new UnresolvedNode(r.ReadUInt64());
             
            model.Scale = r.ReadVector3();
           
            model.TerrainMaterial = r.ReadToken();
            model.TerrainColor = r.ReadColor();

            return model;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var model = item as Model;
            WriteKdop(w, model);

            w.Write(model.Name);
            w.Write(model.Look);
            w.Write(model.Variant);

            WriteObjectList(w, model.AdditionalParts);

            w.Write(model.Node.Uid);

            w.Write(model.Scale);

            w.Write(model.TerrainMaterial);
            w.Write(model.TerrainColor);
        }
    }
}
