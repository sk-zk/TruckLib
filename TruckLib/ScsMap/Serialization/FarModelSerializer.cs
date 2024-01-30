using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class FarModelSerializer : MapItemSerializer
    {
        private const float sizeFactor = 2f;

        public override MapItem Deserialize(BinaryReader r)
        {
            var fm = new FarModel(false);
            ReadKdopItem(r, fm);

            fm.Width = r.ReadSingle() * sizeFactor;
            fm.Height = r.ReadSingle() * sizeFactor;

            // models
            fm.Models = new List<FarModelData>();
            var modelCount = r.ReadUInt32();
            for (int i = 0; i < modelCount; i++)
            {
                var model = new FarModelData();
                model.Model = r.ReadToken();
                model.Scale = r.ReadVector3();
                fm.Models.Add(model);
            }

            // map items
            fm.Children = new List<IMapItem>();
            var childCount = r.ReadUInt32();
            for (int i = 0; i < childCount; i++)
            {
                fm.Children.Add(new UnresolvedItem(r.ReadUInt64()));
            }

            // node_uids
            // first node is the object node, the rest are the model nodes
            var nodeCount = r.ReadUInt32();
            fm.Node = new UnresolvedNode(r.ReadUInt64());
            for (int i = 1; i < nodeCount; i++)
            {
                var uid = r.ReadUInt64();
                // There are two Far Models in the 1.46 map which have n models
                // but n+1 model nodes. Which of them is the bugged one that
                // should be dropped? I don't know, but I hope it's the last one
                // becasue that's what I'm doing.
                if (i <= fm.Models.Count)
                {
                    var farModelData = fm.Models[i-1];
                    farModelData.Node = new UnresolvedNode(uid);
                    fm.Models[i-1] = farModelData;
                }
            }

            return fm;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var fm = item as FarModel;
            WriteKdopItem(w, fm);

            w.Write(fm.Width / sizeFactor);
            w.Write(fm.Height / sizeFactor);

            // models
            w.Write(fm.Models.Count);
            foreach (var model in fm.Models)
            {
                w.Write(model.Model);
                w.Write(model.Scale);
            }

            // map items
            w.Write(fm.Children.Count);
            foreach (var child in fm.Children)
            {
                w.Write(child.Uid);
            }

            // node_uids
            w.Write(fm.Models.Count + 1);
            w.Write(fm.Node.Uid);
            foreach (var model in fm.Models)
            {
                w.Write(model.Node.Uid);
            }
        }
    }
}
