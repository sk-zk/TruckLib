using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
                    ClearTriggers();
                }
            }
        }

        private const int ActivationPointStructAmount = 2;
        private GateActivationPoint[] activationPoints = new GateActivationPoint[ActivationPointStructAmount];
        /// <summary>
        /// Activation points of the gate. Only used if <see cref="Gate.Type">Type</see> is <c>TriggerActivated</c>.
        /// </summary>
        public GateActivationPoint[] ActivationPoints { get => activationPoints; }

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
        }

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            base.UpdateNodeReferences(allNodes);
            foreach (var point in ActivationPoints)
            {
                if (point is not null)
                {
                    point.Node = ResolveNodeReference(point.Node, allNodes);
                }
            }
        }

        private void ClearTriggers()
        {
            for (int i = 0; i < activationPoints.Length; i++)
            {
                if (activationPoints[i] is not null)
                {
                    var node = activationPoints[i].Node;
                    node.Sectors[0].Map.Nodes.Remove(node.Uid);
                    activationPoints[i] = null;
                }
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
    }
}
