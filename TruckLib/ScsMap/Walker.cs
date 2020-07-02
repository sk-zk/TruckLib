using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Creates pedestrians along a given path.
    /// <para>This item has been replaced by Movers and will probably be 
    /// removed from the game at some point.</para>
    /// </summary>
    public class Walker : PathItem, IRecalculatable
    {
        public override ItemType ItemType => ItemType.Walker;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => 120;

        public Token NamePrefix { get; set; } 

        public float Speed { get; set; }

        public float EndDelay { get; set; } 

        public uint Count { get; set; } 

        public float Width { get; set; } 

        public float Angle { get; set; } 

        public List<float> Lengths { get; set; }

        public bool UseCurvedPath
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public bool ActiveDuringDay
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        public bool ActiveDuringNight
        {
            get => Kdop.Flags[2];
            set => Kdop.Flags[2] = value;
        }

        public bool BounceAtEnd
        {
            get => Kdop.Flags[3];
            set => Kdop.Flags[3] = value;
        }

        public bool FollowDir
        {
            get => Kdop.Flags[4];
            set => Kdop.Flags[4] = value;
        }

        public bool ActiveDuringNiceWeather
        {
            get => !Kdop.Flags[5];
            set => Kdop.Flags[5] = !value;
        }

        public bool ActiveDuringBadWeather
        {
            get => !Kdop.Flags[6];
            set => Kdop.Flags[6] = !value;
        }

        public bool RandomSpeedFactor
        {
            get => !Kdop.Flags[7];
            set => Kdop.Flags[7] = !value;
        }

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        public Walker() : base() { }

        internal Walker(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Lengths = new List<float>();
            NamePrefix = "walker_";
            Speed = 1f;
            EndDelay = 0f;
            Count = 1;
            Width = 2f;
            Angle = 0f;
        }

        public void Recalculate()
        {
            // TODO: Properly calculate lengths
            Lengths = new List<float>(Nodes.Count);
            Lengths.ForEach(x => x = 1);
        }
    }
}
