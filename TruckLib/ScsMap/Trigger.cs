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
    /// </summary>
    /// <remarks>Note that, if only two nodes are used, the area is a sphere, 
    /// with the first node defining the center and the second node
    /// defining the radius.</remarks>
    public class Trigger : PolygonItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Trigger;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Base;

        /// <inheritdoc/>
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

        /// <summary>
        /// Gets or sets if the trigger requires a specific orientation to activate(?).
        /// The orientation for the trigger is defined by the yaw of the 0th node.
        /// </summary>
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
        /// Gets or sets if this trigger's icon, if one exists, is only visible
        /// in the UI map once discovered.
        /// </summary>
        public bool Secret
        {
            get => Kdop.Flags[18];
            set => Kdop.Flags[18] = value;
        }

        public bool CollisionActivation
        {
            get => Kdop.Flags[5];
            set => Kdop.Flags[5] = value;
        }

        public Trigger() : base() { }

        internal Trigger(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            PartialActivation = true;
            VehicleActivation = true;
            ConnectedTrailerActivation = true;
            Tags = [];
            Actions = [];
        }

        /// <summary>
        /// Adds a trigger to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="positions">The points of the polygon.</param>
        /// <returns>The newly created trigger.</returns>
        public static Trigger Add(IItemContainer map, IList<Vector3> positions)
        {
            var trigger = Add<Trigger>(map, positions);
            return trigger;
        }
    }
}
