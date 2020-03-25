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
            var fm = new FarModel();
            ReadKdopItem(r, fm);

            fm.Width = r.ReadSingle() * sizeFactor;
            fm.Height = r.ReadSingle() * sizeFactor;

            var modelCount = r.ReadUInt32();
            for (int i = 0; i < modelCount; i++)
            {
                fm.Models[i].Model = r.ReadToken();
                fm.Models[i].Scale = r.ReadVector3();
            }

            // nodes
            // first node is the object node, the other nodes are the perspective nodes
            var nodeCount = r.ReadUInt32();
            fm.Node = new UnresolvedNode(r.ReadUInt64());
            for (int i = 1; i < nodeCount; i++)
            {
                fm.Models[i - 1].PerspectiveNode = new UnresolvedNode(r.ReadUInt64());
            }

            return fm;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var fm = item as FarModel;
            WriteKdopItem(w, fm);

            w.Write(fm.Width / sizeFactor);
            w.Write(fm.Height / sizeFactor);

            var notNullModels = fm.Models.Where(x => x.Model != 0);
            w.Write(notNullModels.Count());
            foreach (var model in notNullModels)
            {
                w.Write(model.Model);
                w.Write(model.Scale);
            }

            w.Write(notNullModels.Count() + 1);
            w.Write(fm.Node.Uid);
            foreach (var model in notNullModels)
            {
                w.Write(model.PerspectiveNode.Uid);
            }
        }
    }
}
