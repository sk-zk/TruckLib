using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    public class Hinge : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.Hinge;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        public Token Model;

        public Token Variant;

        public float MinRotation;

        public float MaxRotation;

        public static Hinge Add(IItemContainer map, Vector3 position, float minRot, float maxRot)
        {
            var hinge = Add<Hinge>(map, position);

            hinge.Model = "door";
            hinge.Variant = "default";
            hinge.MinRotation = minRot;
            hinge.MaxRotation = maxRot;

            return hinge;
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Model = r.ReadToken();
            Variant = r.ReadToken();
            Node = new UnresolvedNode(r.ReadUInt64());
            MinRotation = r.ReadSingle();
            MaxRotation = r.ReadSingle();
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(Model);
            w.Write(Variant);
            w.Write(Node.Uid);
            w.Write(MinRotation);
            w.Write(MaxRotation);
        }
    }
}
