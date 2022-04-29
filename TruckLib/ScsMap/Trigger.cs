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

        public List<Token> Tags { get; set; } 

        public List<TriggerAction> Actions { get; set; }

        /// <summary>
        /// Legacy parameter. Do not use.
        /// </summary>
        [Obsolete]
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

        public bool FarViewDistance
        {
            get => Kdop.Flags[17];
            set => Kdop.Flags[17] = value;
        }

        /// <summary>
        /// Gets or sets if this trigger's icon, if one exists, is only visible in the UI map once discovered.
        /// </summary>
        public bool Secret
        {
            get => Kdop.Flags[18];
            set => Kdop.Flags[18] = value;
        }

        public Trigger() : base() { }

        internal Trigger(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            PartialActivation = true;
            VehicleActivation = true;
            ConnectedTrailerActivation = true;
            Tags = new List<Token>();
            Actions = new List<TriggerAction>();
        }

        public static Trigger Add(IItemContainer map, IList<Vector3> nodePositions)
        {
            var trigger = Add<Trigger>(map, nodePositions);
            return trigger;
        }
    }
}
