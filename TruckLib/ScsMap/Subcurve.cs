using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap
{
    public class Subcurve : IBinarySerializable
    {
        /// <summary>
        /// Unit name of the curve model, as defined in <c>/def/world/curve_model.sii</c>.
        /// </summary>
        public Token Model { get; set; }

        /// <summary>
        /// Unit name of the first part, if applicable.
        /// </summary>
        public Token FirstPart { get; set; }

        /// <summary>
        /// Unit name of the center part variation.
        /// </summary>
        public Token CenterPartVariation { get; set; }

        /// <summary>
        /// Unit name of the last part, if applicable.
        /// </summary>
        public Token LastPart { get; set; }

        /// <summary>
        /// Look of the model, if applicable.
        /// </summary>
        public Token Look { get; set; }

        /// <summary>
        /// Flips the model on the axis formed by the curve's two nodes.
        /// </summary>
        public bool InvertGeometry
        {
            get => flags[0];
            set => flags[0] = value;
        }

        /// <summary>
        /// Whether elements of a sloped item are rendered like stair steps
        /// rather than being stretched along the path.
        /// </summary>
        public bool SteppedGeometry
        {
            get => flags[1];
            set => flags[1] = value;
        }

        /// <summary>
        /// Whether vertices of the model are covered in a random tint noise.<br />
        /// That's what the editor tooltip says, anyway. I don't really see any difference.
        /// </summary>
        public bool PerlinNoise
        {
            get => flags[2];
            set => flags[2] = value;
        }

        public bool IgnoreHeightOffsets
        {
            get => flags[3];
            set => flags[3] = value;
        }

        /// <summary>
        /// Prevents the generated geometry of this subcurve from
        /// connecting to neighboring curves.
        /// <para>This flag can only be applied to subcurves B, C and D.
        /// If it is applied to the primary subcurve, it is ignored.</para>
        /// </summary>
        public bool NoPathInterpolation
        {
            get => flags[31];
            set => flags[31] = value;
        }

        /// <summary>
        /// The seed for the RNG which determines the positions and models
        /// of randomizable elements, particularly vegetation. 
        /// </summary>
        public uint Seed { get; set; }

        /// <summary>
        /// Coefficient for stretching the scheme along the path. For some curves,
        /// this stretches the model; for others, it places its elements further apart.
        /// </summary>
        public float Stretch { get; set; }

        /// <summary>
        /// Relative scale of the model.
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// If not 0, the model will repeat every <i>n</i> meters, overriding the default.
        /// </summary>
        public float FixedStep { get; set; }

        /// <summary>
        /// Unit name of the terrain material, if applicable.
        /// </summary>
        public Token TerrainMaterial { get; set; }

        /// <summary>
        /// Color tint of the terrain.
        /// </summary>
        public Color TerrainColor { get; set; }

        /// <summary>
        /// UV rotation of the terrain texture.
        /// </summary>
        public float TerrainRotation { get; set; }

        /// <summary>
        /// <para>Height offsets for individual elements of the curve.</para>
        /// <para>Offsets are applied to elements in order. For instance, if you want
        /// the third element to have an offset of 5, the content of the list should be
        /// [0, 0, 5].</para>
        /// </summary>
        public List<float> HeightOffsets { get; set; }

        public float InitialHeightOffset { get; set; }

        public float OffsetFromBaseCurveStartX { get; set; }

        public float OffsetFromBaseCurveStartY { get; set; }

        public float OffsetFromBaseCurveEndX { get; set; }

        public float OffsetFromBaseCurveEndY { get; set; }

        private FlagField flags;

        public Subcurve()
        {
            Init();
        }

        internal Subcurve(bool initFields)
        {
            if (initFields) Init();
        }

        /// <summary>
        /// Sets the object's properties to its default values.
        /// </summary>
        private void Init()
        {
            flags = new();
            Look = "default";
            HeightOffsets = [];
            Stretch = 1f;
            Scale = 1f;
            TerrainColor = Color.White;
        }

        /// <inheritdoc/>
        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Model = r.ReadToken();
            flags = new FlagField(r.ReadUInt32());
            Seed = r.ReadUInt32();
            Stretch = r.ReadSingle();
            Scale = r.ReadSingle();
            FixedStep = r.ReadSingle();
            TerrainMaterial = r.ReadToken();
            TerrainColor = r.ReadColor();
            TerrainRotation = r.ReadSingle();
            FirstPart = r.ReadToken();
            LastPart = r.ReadToken();
            CenterPartVariation = r.ReadToken();
            Look = r.ReadToken();

            var heightOffsetCount = r.ReadUInt32();
            HeightOffsets = r.ReadObjectList<float>(heightOffsetCount);

            InitialHeightOffset = r.ReadSingle();
            OffsetFromBaseCurveStartX = r.ReadSingle();
            OffsetFromBaseCurveStartY = r.ReadSingle();
            OffsetFromBaseCurveEndX = r.ReadSingle();
            OffsetFromBaseCurveEndY = r.ReadSingle();
        }

        /// <inheritdoc/>
        public void Serialize(BinaryWriter w)
        {
            w.Write(Model);
            w.Write(flags.Bits);
            w.Write(Seed);
            w.Write(Stretch);
            w.Write(Scale);
            w.Write(FixedStep);
            w.Write(TerrainMaterial);
            w.Write(TerrainColor);
            w.Write(TerrainRotation);
            w.Write(FirstPart);
            w.Write(LastPart);
            w.Write(CenterPartVariation);
            w.Write(Look);

            w.Write(HeightOffsets.Count);
            w.WriteObjectList(HeightOffsets);

            w.Write(InitialHeightOffset);
            w.Write(OffsetFromBaseCurveStartX);
            w.Write(OffsetFromBaseCurveStartY);
            w.Write(OffsetFromBaseCurveEndX);
            w.Write(OffsetFromBaseCurveEndY);
        }

        /// <summary>
        /// Creates a deep clone of this object.
        /// </summary>
        /// <returns>A deep clone of this object.</returns>
        public Subcurve Clone()
        {
            return new Subcurve
            {
                Model = Model,
                flags = flags,
                Seed = Seed,
                Stretch = Stretch,
                Scale = Scale,
                FixedStep = FixedStep,
                TerrainMaterial = TerrainMaterial,
                TerrainColor = TerrainColor,
                TerrainRotation = TerrainRotation,
                FirstPart = FirstPart,
                LastPart = LastPart,
                CenterPartVariation = CenterPartVariation,
                Look = Look,
                HeightOffsets = new(HeightOffsets),
                InitialHeightOffset = InitialHeightOffset,
                OffsetFromBaseCurveStartX = OffsetFromBaseCurveStartX,
                OffsetFromBaseCurveStartY = OffsetFromBaseCurveStartY,
                OffsetFromBaseCurveEndX = OffsetFromBaseCurveEndX,
                OffsetFromBaseCurveEndY = OffsetFromBaseCurveEndY
            };
        }
    }
}
