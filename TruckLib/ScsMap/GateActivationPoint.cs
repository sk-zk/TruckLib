using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Properties of an activation point of a <see cref="Gate"/> item.
    /// </summary>
    public class GateActivationPoint
    {
        /// <summary>
        /// Name of the trigger type.
        /// </summary>
        public string Trigger { get; set; }

        /// <summary>
        /// Position of the trigger.
        /// </summary>
        public INode Node { get; internal set; }

        internal GateActivationPoint()
        {
        }

        internal GateActivationPoint(string trigger, INode node)
        {
            Trigger = trigger;
            Node = node;
        }

        /// <summary>
        /// Creates a gate activation point.
        /// </summary>
        /// <param name="trigger">Name of the trigger type.</param>
        /// <param name="position">The position of the activation point.</param>
        /// <param name="parent">The gate item this point belongs to.</param>
        public GateActivationPoint(string trigger, Vector3 position, Gate parent)
        {
            Trigger = trigger;
            Node = parent.Node.Parent.AddNode(position, false);
            Node.ForwardItem = parent;
        }
    }
}
