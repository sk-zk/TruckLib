using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap.Collections;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A gate which can be activated in various different ways.
    /// </summary>
    public class Gate : SingleNodeItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Gate;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Unit name of the model, as defined in <c>/def/world/gate_model.sii</c>.
        /// </summary>
        public Token Model { get; set; }

        /// <summary>
        /// Gets or sets the view distance of the item in meters.
        /// </summary>
        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        private const int GateTypeStart = 0;
        private const int GateTypeLength = 2;
        /// <summary>
        /// The activation type of the gate.
        /// <para>If the type is changed from <c>TriggerActivated</c> to <c>AlwaysOpen</c> or <c>AlwaysClosed</c>,
        /// the activation points will be cleared.</para>
        /// </summary>
        public GateType Type {
            get => (GateType)Kdop.Flags.GetBitString(GateTypeStart, GateTypeLength);
            set
            {
                Kdop.Flags.SetBitString(GateTypeStart, GateTypeLength, (uint)value);
                if (value != GateType.TriggerActivated)
                {
                    ActivationPoints?.Clear();
                }
            }
        }

        /// <summary>
        /// Activation points of the gate. Only used if <see cref="Type">Type</see> is <c>TriggerActivated</c>.
        /// </summary>
        public GateActivationPointList ActivationPoints { get; set; }

        public Gate() : base()
        {
        }

        internal Gate(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Type = GateType.AlwaysClosed;
            ActivationPoints = new GateActivationPointList(this);
        }

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(IDictionary<ulong, INode> allNodes)
        {
            base.UpdateNodeReferences(allNodes);
            for (int i = 0; i < ActivationPoints.Count; i++)
            {
                var point = ActivationPoints[i];
                point.Node = ResolveNodeReference(point.Node, allNodes);
                ActivationPoints[i] = point;
            }
        }

        /// <summary>
        /// Adds a gate to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the new gate.</param>
        /// <param name="model">Unit name of the gate model.</param>
        /// <param name="type">The gate activation type.</param>
        /// <returns>The newly created gate.</returns>
        public static Gate Add(IItemContainer map, Vector3 position, Token model, GateType type)
        {
            var gate = Add<Gate>(map, position);
            gate.Model = model;
            gate.Type = type;
            return gate;
        }
     
        /// <inheritdoc/>
        internal override Vector3 GetCenter()
        {
            if (Type == GateType.TriggerActivated && ActivationPoints.Count > 0)
            {
                var acc = Vector3.Zero;
                foreach (var point in ActivationPoints)
                {
                    acc += point.Node.Position;
                }
                return acc / ActivationPoints.Count;
            } 
            else
            {
                return base.GetCenter();
            }
        }

        internal override IEnumerable<INode> GetItemNodes()
        {
            var nodes = new INode[ActivationPoints.Count + 1];
            nodes[0] = Node;
            for (int i = 0; i < ActivationPoints.Count; i++)
            {
                nodes[i + 1] = ActivationPoints[i].Node;
            }
            return nodes;
        }
    }
}
