using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Applies a traffic rule inside a polygonal area.
    /// </summary>
    public class TrafficArea : PolygonItem
    {
        public override ItemType ItemType => ItemType.TrafficArea;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public List<Token> Tags { get; set; } = new List<Token>();

        public Token Rule { get; set; }

        public float Range { get; set; }

        public bool CrossroadArea
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        public bool RemoveSemaphores
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        public static TrafficArea Add(IItemContainer map, Vector3[] nodePositions, Token rule)
        {
            var ta = Add<TrafficArea>(map, nodePositions);
            ta.Rule = rule;
            return ta;
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Tags = ReadObjectList<Token>(r);
            Nodes = ReadNodeRefList(r);
            Rule = r.ReadToken();
            Range = r.ReadSingle();
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            WriteObjectList(w, Tags);
            WriteNodeRefList(w, Nodes);
            w.Write(Rule);
            w.Write(Range);
        }
    }
}
