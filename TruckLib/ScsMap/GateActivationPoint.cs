using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public class GateActivationPoint
    {
        /// <summary>
        /// Name of the trigger.
        /// </summary>
        public string Trigger { get; set; }

        /// <summary>
        /// Position of the trigger.
        /// </summary>
        public INode Node { get; internal set; }

        public GateActivationPoint()
        {
        }

        internal GateActivationPoint(string trigger, INode node)
        {
            Trigger = trigger;
            Node = node;
        }

        public GateActivationPoint(string trigger, Vector3 position, Gate parent)
        {
            Trigger = trigger;
            Node = parent.Node.Sectors[0].Map.AddNode(position, false);
            Node.ForwardItem = parent;
        }
    }
}
