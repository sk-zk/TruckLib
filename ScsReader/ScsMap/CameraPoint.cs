using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    public class CameraPoint : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.CameraPoint;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public List<Token> Tags = new List<Token>();

        public static CameraPoint Add(IItemContainer map, Vector3 position, List<Token> tags)
        {
            var point = Add<CameraPoint>(map, position);
            point.Tags = tags;
            return point;
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Tags = ReadObjectList<Token>(r);
            Node = new UnresolvedNode(r.ReadUInt64());
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            WriteObjectList(w, Tags);
            w.Write(Node.Uid);
        }
    }
}
