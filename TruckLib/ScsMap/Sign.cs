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
    /// A traffic sign or navigation sign.
    /// </summary>
    public class Sign : SingleNodeItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Sign;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <summary>
        /// Gets or sets the sector file this sign should be written to. If it has a traffic rule
        /// associated with it, set it to <see cref="ItemFile.Base">Base</see>. Otherwise, set it to 
        /// <see cref="ItemFile.Aux">Aux</see>.
        /// </summary>
        public new ItemFile ItemFile
        {
            get => base.ItemFile;
            set => base.ItemFile = value;
        }

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
        /// The unit name of the sign model.
        /// </summary>
        public Token Model { get; set; }

        /// <summary>
        /// The look of the model.
        /// </summary>
        public Token Look { get; set; }

        /// <summary>
        /// The variant of the model.
        /// </summary>
        public Token Variant { get; set; }

        /// <summary>
        /// Sign text for legacy navigation signs.
        /// </summary>
        public SignBoard[] SignBoards { get; set; }

        /// <summary>
        /// Full name of the sign template on this sign.
        /// </summary>
        public string SignTemplate { get; set; }

        /// <summary>
        /// The attribute overrides used on the sign template.
        /// </summary>
        public List<SignOverride> SignOverrides { get; set; } 

        /// <summary>
        /// Gets or sets if this sign should be rotated to align with the direction of a nearby <see cref="Road"/>.
        /// </summary>
        public bool FollowRoadDir
        {
            get => Kdop.Flags[24];
            set => Kdop.Flags[24] = value;
        }

        /// <summary>
        /// Gets or sets if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection
        {
            get => Kdop.Flags[26];
            set => Kdop.Flags[26] = value;
        }

        /// <summary>
        /// Gets or sets if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[27];
            set => Kdop.Flags[27] = value;
        }

        public Sign() : base() 
        {
        }

        internal Sign(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            SignOverrides = new List<SignOverride>();
        }

        /// <summary>
        /// Adds a sign to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the sign.</param>
        /// <param name="model">The unit name of the model.</param>
        /// <param name="template">The full name of the sign template.</param>
        /// <returns>The newly created sign.</returns>
        public static Sign Add(IItemContainer map, Vector3 position, Token model, string template)
        {
            var sign = Add<Sign>(map, position);
            sign.Model = model;
            sign.SignTemplate = template;
            return sign;
        }

        /// <summary>
        /// Sign text for legacy navigation signs.
        /// </summary>
        public struct SignBoard
        {
            public Token Road;
            public Token City1;
            public Token City2;
        }

    }
}
