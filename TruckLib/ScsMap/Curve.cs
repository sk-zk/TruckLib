using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Buildings on steroids. TODO: Write actual description.
    /// </summary>
    public class Curve : PolylineItem
    {
        public override ItemType ItemType => ItemType.Curve;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public float MinLength => 0f;

        public float MaxLength => 1000;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        public Token Model { get; set; }

        public Token Look { get; set; } = "default";

        public Token FirstPart { get; set; }

        public Token CenterPartVariation { get; set; }

        public Token LastPart { get; set; }

        public Token TerrainMaterial { get; set; }

        public Color TerrainColor { get; set; }

        public uint RandomSeed { get; set; }

        public float Stretch { get; set; } = 1f;

        public float FixedStep { get; set; } = 0f;

        public float Scale { get; set; } = 1;

        public List<float> HeightOffsets { get; set; } = new List<float>();

        /// <summary>
        /// Determines if the item is reflected in water.
        /// </summary>
        public bool WaterReflection
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        /// <summary>
        /// Determines if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        /// <summary>
        /// Determines if the player can collide with this item.
        /// </summary>
        public bool Collision
        {
            get => !Flags[2];
            set => Flags[2] = !value;
        }

        public bool InvertGeometry
        {
            get => Flags[3];
            set => Flags[3] = value;
        }

        public bool SteppedGeometry
        {
            get => Flags[4];
            set => Flags[4] = value;
        }

        public bool PerlinNoise
        {
            get => Flags[5];
            set => Flags[5] = value;
        }

        /// <summary>
        /// Determines if this item casts shadows.
        /// </summary>
        public bool Shadows
        {
            get => !Flags[6];
            set => Flags[6] = !value;
        }

        /// <summary>
        /// Determines if this item is visible in mirrors.
        /// </summary>
        public bool MirrorReflection
        {
            get => !Flags[7];
            set => Flags[7] = !value;
        }

        /// <summary>
        /// Determines if only flat textures are used as vegetation.
        /// </summary>
        public bool LowPolyVegetation
        {
            get => Flags[20];
            set => Flags[20] = value;
        }

        public bool UseLinearPath
        {
            get => Flags[21];
            set => Flags[21] = value;
        }

        /// <summary>
        /// Determines if detail vegetation (small clumps of grass etc.) is drawn.
        /// </summary>
        public bool DetailVegetation
        {
            get => Flags[22];
            set => Flags[22] = value;
        }

        public static Curve Add(IItemContainer map, Vector3 backwardPos, Vector3 forwardPos, Token model)
        {
            var curve = Add<Curve>(map, backwardPos, forwardPos);

            curve.InitFromAddOrAppend(backwardPos, forwardPos, model);

            return curve;
        }

        public Curve Append(Vector3 position, bool cloneSettings = true)
        {
            if (!cloneSettings)
            {
                return Append(position, Model);
            }

            var b = Append(position, Model);
            CopySettingsTo(b);
            return b;
        }

        public Curve Append(Vector3 position, Token model)
        {
            var curve = Append<Curve>(position);
            curve.InitFromAddOrAppend(ForwardNode.Position, position, model);
            return curve;
        }

        private void InitFromAddOrAppend(Vector3 backwardPos, Vector3 forwardPos, Token model)
        {
            Model = model;
            Length = Vector3.Distance(backwardPos, forwardPos);
        }

        private void CopySettingsTo(Curve c)
        {
            c.Flags = Flags;
            c.FirstPart = FirstPart;
            c.CenterPartVariation = CenterPartVariation;
            c.LastPart = LastPart;
            c.Look = Look;
            c.TerrainMaterial = TerrainMaterial;
            c.TerrainColor = TerrainColor;
            c.ViewDistance = ViewDistance;
            c.RandomSeed = RandomSeed;
            c.Stretch = Stretch;
            c.FixedStep = FixedStep;
            c.HeightOffsets = new List<float>(HeightOffsets);
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Model = r.ReadToken();

            Node = new UnresolvedNode(r.ReadUInt64());
            ForwardNode = new UnresolvedNode(r.ReadUInt64());

            Length = r.ReadSingle();

            RandomSeed = r.ReadUInt32();

            Stretch = r.ReadSingle();
            Scale = r.ReadSingle();
            FixedStep = r.ReadSingle();

            TerrainMaterial = r.ReadToken();
            TerrainColor = r.ReadColor();

            FirstPart = r.ReadToken();
            LastPart = r.ReadToken();
            CenterPartVariation = r.ReadToken();

            Look = r.ReadToken();

            HeightOffsets = ReadObjectList<float>(r);
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(Model);

            w.Write(Node.Uid);
            w.Write(ForwardNode.Uid);

            w.Write(Length);

            w.Write(RandomSeed);

            w.Write(Stretch);
            w.Write(Scale);
            w.Write(FixedStep);

            w.Write(TerrainMaterial);
            w.Write(TerrainColor);

            w.Write(FirstPart);
            w.Write(LastPart);
            w.Write(CenterPartVariation);

            w.Write(Look);

            WriteObjectList(w, HeightOffsets);
        }
    }
}
