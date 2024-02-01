using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Properties of an activation point of a <see cref="Gate"/> item.
    /// </summary>
    public struct GateActivationPoint
    {
        /// <summary>
        /// Position of the trigger.
        /// </summary>
        public INode Node { get; internal set; }

        /// <summary>
        /// Name of the trigger type.
        /// </summary>
        public string Trigger { get; set; }

        public GateActivationPoint(INode node, string trigger)
        {
            Node = node;
            Trigger = trigger;
        }
    }
}
