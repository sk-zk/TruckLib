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
    /// Applies a traffic rule inside a polygon.
    /// </summary>
    public class TrafficArea : PolygonItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.TrafficArea;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Base;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public List<Token> Tags { get; set; }

        /// <summary>
        /// Unit name of the traffic rule, as defined in <c>/def/world/traffic_rules.sii</c>.
        /// </summary>
        public Token Rule { get; set; }

        /// <summary>
        /// If greater than 0, the traffic area is limited to a polyhedron with this height.
        /// The polygon created by the nodes of this item are the bottom face.
        /// </summary>
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

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Tags = new List<Token>();
        }

        /// <summary>
        /// Adds a traffic area to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="positions">The points of the polygon.</param>
        /// <param name="rule">The unit name of the traffic rule.</param>
        /// <returns>The newly created traffic area.</returns>
        public static TrafficArea Add(IItemContainer map, IList<Vector3> positions, Token rule)
        {
            var ta = Add<TrafficArea>(map, positions);
            ta.Rule = rule;
            return ta;
        }
    }
}
