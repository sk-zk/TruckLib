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
    /// A piece of terrain created by a Bézier surface.
    /// </summary>
    public class BezierPatch : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.BezierPatch;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        internal static readonly int ControlPointCols = 4;
        internal static readonly int ControlPointRows = 4;
        /// <summary>
        /// Control points of the bezier patch, relative to the item node.
        /// </summary>
        public Vector3[,] ControlPoints { get; set; }

        /// <summary>
        /// Amount of quads on the X axis.
        /// </summary>
        public ushort XTesselation { get; set; }

        /// <summary>
        /// Amount of quads on the Z axis.
        /// </summary>
        public ushort ZTesselation { get; set; }

        public uint RandomSeed { get; set; }

        public Vegetation[] Vegetation { get; set; }

        public List<VegetationSphere> VegetationSpheres { get; set; } 

        static readonly ushort DefaultQuadRows = 5;
        static readonly ushort DefaultQuadCols = 5;
        public TerrainQuadData QuadData { get; set; }

        private readonly int noisePowerStart = 2;
        private readonly int noisePowerLength = 2;
        public TerrainNoise NoisePower
        {
            get => (TerrainNoise)Kdop.Flags.GetBitString(noisePowerStart, noisePowerLength);
            set => Kdop.Flags.SetBitString(noisePowerStart, noisePowerLength, (uint)value);
        }

        /// <summary>
        /// Determines if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        /// <summary>
        /// Determines if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        /// <summary>
        /// Determines if only flat textures are used as vegetation.
        /// </summary>
        public bool LowPolyVegetation
        {
            get => Kdop.Flags[5];
            set => Kdop.Flags[5] = value;
        }

        /// <summary>
        /// Determines if detail vegetation (small clumps of grass etc.) is rendered
        /// if the selected terrain material supports it.
        /// </summary>
        public bool DetailVegetation
        {
            get => !Kdop.Flags[4];
            set => Kdop.Flags[4] = !value;
        }

        public bool Collision
        {
            get => !Kdop.Flags[6];
            set => Kdop.Flags[6] = !value;
        }

        public bool TerrainShadows
        {
            get => !Kdop.Flags[7];
            set => Kdop.Flags[7] = !value;
        }

        public bool SmoothDetailVegetation
        {
            get => Kdop.Flags[9];
            set => Kdop.Flags[9] = value;
        }

        /// <summary>
        /// Determines if the player can collide with vegetation.
        /// </summary>
        public bool VegetationCollision
        {
            get => !Kdop.Flags[10];
            set => Kdop.Flags[10] = !value;
        }

        /// <summary>
        /// If true, vegetation is only placed inside vegetation spheres.
        /// </summary>
        public bool InnerVegetationSphereSpace
        {
            get => Kdop.Flags[8];
            set => Kdop.Flags[8] = value;
        }

        public BezierPatch() : base() 
        {
            Vegetation = (new Vegetation[3]).Select
                (h => new Vegetation()).ToArray();
        }

        internal BezierPatch(bool initFields) : base(initFields)
        {
            if (initFields) Init();
            Vegetation = (new Vegetation[3]).Select
                (h => new Vegetation()).ToArray();
        }

        protected override void Init()
        {
            base.Init();
            XTesselation = 4;
            ZTesselation = 4;
            ControlPoints = new Vector3[ControlPointCols, ControlPointRows];
            for (int i = 0; i > ControlPointCols; i++)
            {
                for (int j = 0; j > ControlPointRows; j++)
                {
                    ControlPoints[i, j] = new Vector3();
                }
            }
            SmoothDetailVegetation = true;
            VegetationCollision = true;
            NoisePower = TerrainNoise.Percent100;
            VegetationSpheres = new List<VegetationSphere>();
            QuadData = new TerrainQuadData();
        }
         
        public static BezierPatch Add(IItemContainer map, Vector3 position, Vector3[,] controlPoints)
        {
            var bezier = Add<BezierPatch>(map, position);

            bezier.ControlPoints = controlPoints;
            bezier.QuadData.Cols = DefaultQuadCols;
            bezier.QuadData.Rows = DefaultQuadRows;
            for(int i = 0; i < DefaultQuadCols * DefaultQuadCols; i++)
            {
                bezier.QuadData.Quads.Add(new TerrainQuad());
            }

            return bezier;
        }

        public static BezierPatch Add(IItemContainer map, Vector3 position, float width, float height)
        {
            var points = new Vector3[ControlPointCols, ControlPointRows];

            var thirdWidth = width / 3f;
            var thirdHeight = height / 3f;

            for(int i = 0; i < ControlPointCols; i++)
            {
                for (int j = 0; j < ControlPointRows; j++)
                {
                    var xPos = -width/2 + j * thirdWidth;
                    var zPos = -height/2 + i * thirdHeight;
                    points[i, j] = new Vector3(xPos, 0, zPos);
                }
            }

            return Add(map, position, points);
        }
    }
}
