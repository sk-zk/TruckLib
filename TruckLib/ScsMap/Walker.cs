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

        public List<Node> Nodes { get; set; } = new List<Node>(2);

        public Token NamePrefix { get; set; } = "walker_";

        public float Speed { get; set; } = 1f;

        public float EndDelay { get; set; } = 0f;

        public uint Count { get; set; } = 1;

        public float Width { get; set; } = 2f;

        public float Angle { get; set; } = 0f;

        public List<float> Lengths { get; set; } = new List<float>();

        public bool UseCurvedPath
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public bool ActiveDuringDay
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        public bool ActiveDuringNight
        {
            get => Kdop.Flags[2];
            set => Kdop.Flags[2] = value;
        }

        public bool BounceAtEnd
        {
            get => Kdop.Flags[3];
            set => Kdop.Flags[3] = value;
        }

        public bool FollowDir
        {
            get => Kdop.Flags[4];
            set => Kdop.Flags[4] = value;
        }

        public bool ActiveDuringNiceWeather
        {
            get => !Kdop.Flags[5];
            set => Kdop.Flags[5] = !value;
        }

        public bool ActiveDuringBadWeather
        {
            get => !Kdop.Flags[6];
            set => Kdop.Flags[6] = !value;
        }

        public bool RandomSpeedFactor
        {
            get => !Kdop.Flags[7];
            set => Kdop.Flags[7] = !value;
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

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is UnresolvedNode
                    && allNodes.TryGetValue(Nodes[i].Uid, out var resolvedNode))
                {
                    Nodes[i] = resolvedNode;
                }
            }
        }
    }
}
