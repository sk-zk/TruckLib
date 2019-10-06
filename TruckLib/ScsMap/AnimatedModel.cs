using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// TODO: What even is this?
    /// </summary>
    public class AnimatedModel : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.AnimatedModel;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        public List<Token> Tags { get; set; } = new List<Token>();

        public Token Model { get; set; } = new Token();
     
        public bool InitHidden
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public static AnimatedModel Add(IItemContainer map, Vector3 position, Token model, List<Token> tags)
        {
            var anim = Add<AnimatedModel>(map, position);

            anim.Tags = tags;
            anim.Model = model;

            return anim;
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Tags = ReadObjectList<Token>(r);
            Model = r.ReadToken();
            Node = new UnresolvedNode(r.ReadUInt64());
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            WriteObjectList(w, Tags);
            w.Write(Model);
            w.Write(Node.Uid);
        }

    }
}
