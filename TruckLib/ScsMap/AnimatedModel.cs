using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// <p>TODO: What even is this?</p>
    /// <p>Removed from the editor item menu, but it's still in the game and the
    /// item properties dialog still works.</p>
    /// </summary>
    [Obsolete]
    public class AnimatedModel : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.AnimatedModel;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public List<Token> Tags { get; set; }

        public Token Model { get; set; } = new Token();
     
        public bool InitHidden
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public AnimatedModel() : base() { }

        internal AnimatedModel(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Tags = new List<Token>();
        }

        public static AnimatedModel Add(IItemContainer map, Token model, Vector3 position, 
            List<Token> tags = null)
        {
            var anim = Add<AnimatedModel>(map, position);

            anim.Tags = tags;
            anim.Model = model;

            return anim;
        }

    }
}
