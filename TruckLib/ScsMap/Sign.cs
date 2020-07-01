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
        public override ItemType ItemType => ItemType.Sign;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        public new ItemFile ItemFile
        {
            get => itemFile;
            // Most signs go to aux, unless they have a traffic_rule attached to them
            // in the .sii definition, in which case they go to base.
            // This library has no way of knowing that, so it relies on the user
            // to deal with it.
            set => itemFile = value;
        }

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// The unit name of the sign model.
        /// </summary>
        public Token Model { get; set; }

        public Token Look { get; set; }

        public Token Variant { get; set; }

        /// <summary>
        /// Sign text for legacy navigation signs (e.g. be-navigation/board straight left right b).
        /// </summary>
        public SignBoard[] SignBoards { get; set; }

        /// <summary>
        /// The sign template on this sign.
        /// </summary>
        /// <example>sign_templ.balt_40</example>
        public string SignTemplate { get; set; }

        /// <summary>
        /// The attribute overrides used on the sign template.
        /// </summary>
        public List<SignOverride> SignOverrides { get; set; } 

        public bool FollowRoadDir
        {
            get => Kdop.Flags[24];
            set => Kdop.Flags[24] = value;
        }

        /// <summary>
        /// Determines if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection
        {
            get => Kdop.Flags[26];
            set => Kdop.Flags[26] = value;
        }

        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[27];
            set => Kdop.Flags[27] = value;
        }

        public Sign() : base() 
        {
            SignBoards = new SignBoard[3];
        }

        internal Sign(bool initFields) : base(initFields)
        {
            if (initFields) Init();
            SignBoards = new SignBoard[3];
        }

        protected override void Init()
        {
            base.Init();
            SignOverrides = new List<SignOverride>();
        }

        /// <summary>
        /// Adds a sign to the map.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position"></param>
        /// <param name="model"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static Sign Add(IItemContainer map, Vector3 position, Token model, string template)
        {
            var sign = Add<Sign>(map, position);
            sign.Model = model;
            sign.SignTemplate = template;
            return sign;
        }

        /// <summary>
        /// Sign text struct for legacy navigation signs,
        /// e.g. "be-navigation/board straight left right b".
        /// </summary>
        public struct SignBoard
        {
            public Token Road;
            public Token City1;
            public Token City2;
        }

    }
}
