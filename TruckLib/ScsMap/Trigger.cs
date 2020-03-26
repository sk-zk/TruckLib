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
    /// Defines a trigger area. 
    /// <para>If only two nodes are used, the area is a sphere, 
    /// its radius being distance between the two nodes.</para>
    /// </summary>
    public class Trigger : PolygonItem
    {
        public override ItemType ItemType => ItemType.Trigger;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public List<Token> Tags { get; set; } = new List<Token>();

        public List<TriggerAction> Actions { get; set; } = new List<TriggerAction>();

        /// <summary>
        /// Legacy parameter. Do not use.
        /// </summary>
        public float Range { get; set; }

        public byte DlcGuard
        {
            get => Kdop.Flags.GetByte(1);
            set => Kdop.Flags.SetByte(1, value);
        }

        public bool PartialActivation
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public bool VehicleActivation
        {
            get => Kdop.Flags[2];
            set => Kdop.Flags[2] = value;
        }

        public bool ConnectedTrailerActivation
        {
            get => Kdop.Flags[3];
            set => Kdop.Flags[3] = value;
        } 
        public bool DisconnectedTrailerActivation
        {
            get => Kdop.Flags[4];
            set => Kdop.Flags[4] = value;
        }

        public bool OrientedActivation
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }
        
        public bool InitDisabled
        {
            get => Kdop.Flags[8];
            set => Kdop.Flags[8] = value;
        }

        public Trigger() : base()
        {
            PartialActivation = true;
            VehicleActivation = true;
            ConnectedTrailerActivation = true;
        }

        public static Trigger Add(IItemContainer map, Vector3[] nodePositions, List<TriggerAction> actions)
        {
            var trigger = Add<Trigger>(map, nodePositions);
            trigger.Actions = actions;
            return trigger;
        }
    }
}
