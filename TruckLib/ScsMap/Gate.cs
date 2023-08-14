using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public class Gate : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.Gate;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Unit name of the model.
        /// </summary>
        public Token Model { get; set; }

        private const int GateTypeStart = 0;
        private const int GateTypeLength = 2;
        /// <summary>
        /// The behaviour of the gate.
        /// <para>If the gate is changed from TriggerActivated to AlwaysOpen or AlwaysClosed,
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
        public GateActivationPoint[] ActivationPoints { get => activationPoints; }

        public Gate() : base()
        {
        }

        internal Gate(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Type = GateType.AlwaysClosed;
        }

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
        /// <param name="model">The gate model.</param>
        /// <param name="type">The gate activation type.</param>
        /// <returns></returns>
        public static Gate Add(IItemContainer map, Vector3 position, Token model, GateType type)
        {
            var gate = Add<Gate>(map, position);
            gate.Model = model;
            gate.Type = type;
            return gate;
        }
    }
}
