using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Creates pedestrians along a given path.
    /// <para>This item has been replaced by Movers and will probably be 
    /// removed from the game at some point.</para>
    /// </summary>
    public class Walker : MapItem
    {
        public override ItemType ItemType => ItemType.Walker;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => 120;

        public List<Node> Nodes { get; set; } = new List<Node>();

        public Token NamePrefix { get; set; } = "walker_";

        public float Speed { get; set; } = 1f;

        public float EndDelay { get; set; } = 0f;

        public uint Count { get; set; } = 1;

        public float Width { get; set; } = 2f;

        public float Angle { get; set; } = 0f;

        public List<float> Lengths { get; set; } = new List<float>();

        public bool UseCurvedPath
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        public bool ActiveDuringDay
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        public bool ActiveDuringNight
        {
            get => Flags[2];
            set => Flags[2] = value;
        }

        public bool BounceAtEnd
        {
            get => Flags[3];
            set => Flags[3] = value;
        }

        public bool FollowDir
        {
            get => Flags[4];
            set => Flags[4] = value;
        }

        public bool ActiveDuringNiceWeather
        {
            get => !Flags[5];
            set => Flags[5] = !value;
        }

        public bool ActiveDuringBadWeather
        {
            get => !Flags[6];
            set => Flags[6] = !value;
        }

        public bool RandomSpeedFactor
        {
            get => !Flags[7];
            set => Flags[7] = !value;
        }

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        internal override IEnumerable<Node> GetItemNodes()
        {
            return new List<Node>(Nodes);
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            NamePrefix = r.ReadToken();
            Speed = r.ReadSingle();         
            EndDelay = r.ReadSingle();        
            Count = r.ReadUInt32();
            Width = r.ReadSingle();
            Angle = r.ReadSingle();
            Lengths = ReadObjectList<float>(r);
            Nodes = ReadNodeRefList(r);
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(NamePrefix);
            w.Write(Speed);
            w.Write(EndDelay);
            w.Write(Count);
            w.Write(Width);
            w.Write(Angle);
            WriteObjectList(w, Lengths);
            WriteNodeRefList(w, Nodes);
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is UnresolvedNode && allNodes.ContainsKey(Nodes[i].Uid))
                {
                    Nodes[i] = allNodes[Nodes[i].Uid];
                }
            }
        }
    }
}
