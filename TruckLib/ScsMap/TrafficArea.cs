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

        public List<Token> Tags { get; set; }

        public Token Rule { get; set; }

        public float Range { get; set; }

        public bool CrossroadArea
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public bool RemoveSemaphores
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        public TrafficArea() : base() { }

        internal TrafficArea(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Tags = new List<Token>();
        }

        public static TrafficArea Add(IItemContainer map, IList<Vector3> nodePositions, Token rule)
        {
            var ta = Add<TrafficArea>(map, nodePositions);
            ta.Rule = rule;
            return ta;
        }
    }
}
