using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A model with a static loop animation.
    /// </summary>
    public class Mover : MapItem
    {
        public override ItemType ItemType => ItemType.Mover;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        public Token Model { get; set; }

        public Token Look { get; set; }

        public float Speed { get; set; } = 1;

        public float DelayAtEnd { get; set; } = 0;

        public float Width { get; set; } = 0;

        public uint Count { get; set; } = 1;

        public List<Node> Nodes { get; set; } = new List<Node>();

        public List<float> Lengths { get; set; } = new List<float>();

        public List<Token> Tags { get; set; } = new List<Token>();

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

        /// <summary>
        /// Determines if the item uses a curved path 
        /// rather than a linear path.
        /// </summary>
        public bool UseCurvedPath
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        /// <summary>
        /// Determines if the item is reflected in water.
        /// </summary>
        public bool WaterReflection
        {
            get => Flags[5];
            set => Flags[5] = value;
        }

        /* This property has apparently been removed
         * as of 1.35 (map ver. 868)
        public bool Pedestrian
        {
            get => Flags[8];
            set => Flags[8] = value;
        }
        */

        public bool UseSound
        {
            get => !Flags[9];
            set => Flags[9] = !value;
        }

        /// <summary>
        /// Determines if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Flags[6];
            set => Flags[6] = value;
        }

        public bool IgnoreBoundingVolume
        {
            get => Flags[7];
            set => Flags[7] = value;
        }

        public bool ActiveDuringBadWeather
        {
            get => Flags[11];
            set => Flags[11] = value;
        }

        public bool ActiveDuringNiceWeather
        {
            get => Flags[10];
            set => Flags[10] = value;
        }

        public bool PreferNonMovableAnimation
        {
            get => Flags[12];
            set => Flags[12] = value;
        }

        public bool UniformItemPlacement
        {
            get => Flags[13];
            set => Flags[13] = value;
        }

        public bool KeepOrientationOnBounce
        {
            get => Flags[14];
            set => Flags[14] = value;
        }

        public Mover() : base()
        {
            ActiveDuringDay = true;
            ActiveDuringNight = true;
            ActiveDuringNiceWeather = true;
            ActiveDuringBadWeather = true;
            FollowDir = true;
            UseCurvedPath = true;
        }

        internal override IEnumerable<Node> GetItemNodes()
        {
            return new List<Node>(Nodes);
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Tags = ReadObjectList<Token>(r);

            Model = r.ReadToken();
            Look = r.ReadToken();
            Speed = r.ReadSingle();        
            DelayAtEnd = r.ReadSingle();
            Width = r.ReadSingle();
            Count = r.ReadUInt32();
            Lengths = ReadObjectList<float>(r);
            Nodes = ReadNodeRefList(r);
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            WriteObjectList(w, Tags);

            w.Write(Model);
            w.Write(Look);
            w.Write(Speed);
            w.Write(DelayAtEnd);
            w.Write(Width);
            w.Write(Count);
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
