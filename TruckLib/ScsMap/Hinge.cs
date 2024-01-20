using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// According to the wiki: "Currently unused. It defined object that could be placed 
    /// on map and be swung by player truck e.g. swing doors."
    /// </summary>
    /// <remarks>In the editor, trying to open the properties dialog for this item
    /// will cause it to crash.</remarks>
    [Obsolete]
    public class Hinge : SingleNodeItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Hinge;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Gets or sets the view distance of the item in meters.
        /// </summary>
        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// Unit name of the model.
        /// </summary>
        public Token Model { get; set; }

        /// <summary>
        /// Look of the model.
        /// </summary>
        public Token Look { get; set; }

        public float MinRotation { get; set; }

        public float MaxRotation { get; set; }

        public Hinge() : base() { }

        internal Hinge(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Model = "door";
            Look = "default";
        }

        /// <summary>
        /// Adds a hinge to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the hinge.</param>
        /// <param name="model">The unit name of the model.</param>
        /// <returns>The newly created hinge.</returns>
        public static Hinge Add(IItemContainer map, Vector3 position, Token model)
        {
            var hinge = Add<Hinge>(map, position);

            hinge.Model = model;

            return hinge;
        }
    }
}
