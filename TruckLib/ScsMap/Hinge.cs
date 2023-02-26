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
    /// <para>In the editor, trying to open the properties dialog for this item
    /// will cause it to crash.</para>
    /// </summary>
    [Obsolete]
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
            Model = "door";
            Look = "default";
        }

        public static Hinge Add(IItemContainer map, Vector3 position, Token model, 
            Token look, float minRot, float maxRot)
        {
            var hinge = Add<Hinge>(map, position);

            hinge.Model = model;
            hinge.Look = look;
            hinge.MinRotation = minRot;
            hinge.MaxRotation = maxRot;

            return hinge;
        }
    }
}
