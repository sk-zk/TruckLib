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
    /// <para>Trying to use this item crashes the editor as of 1.36.</para>
    /// </summary>
    public class Hinge : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.Hinge;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        public Token Model { get; set; }

        public Token Look { get; set; }

        public float MinRotation { get; set; }

        public float MaxRotation { get; set; }

        public Hinge() : base() { }

        internal Hinge(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
        }

        public static Hinge Add(IItemContainer map, Vector3 position, float minRot, float maxRot)
        {
            var hinge = Add<Hinge>(map, position);

            hinge.Model = "door";
            hinge.Look = "default";
            hinge.MinRotation = minRot;
            hinge.MaxRotation = maxRot;

            return hinge;
        }
    }
}
