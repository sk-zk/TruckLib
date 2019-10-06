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

        static readonly int ControlPointCols = 4;
        static readonly int ControlPointRows = 4;
        /// <summary>
        /// Control points of the bezier patch, relative to the item node.
        /// </summary>
        public Vector3[,] ControlPoints = new Vector3[ControlPointCols, ControlPointRows];

        public ushort XTesselation { get; set; } = 4;

        public ushort ZTesselation { get; set; } = 4;

        public float UVRotation { get; set; }

        public uint RandomSeed { get; set; }

        public Vegetation[] Vegetation { get; set; } = new Vegetation[3];

        public List<VegetationSphere> VegetationSpheres { get; set; } 
            = new List<VegetationSphere>();

        static readonly ushort DefaultQuadRows = 5;
        static readonly ushort DefaultQuadCols = 5;
        public TerrainQuadData QuadData { get; set; } = new TerrainQuadData();

        private readonly int noisePowerStart = 2;
        private readonly int noisePowerLength = 2;
        public TerrainNoise NoisePower
        {
            get => (TerrainNoise)Flags.GetBitString(noisePowerStart, noisePowerLength);
            set => Flags.SetBitString((uint)value, noisePowerStart, noisePowerLength);
        }

        /// <summary>
        /// Determines if the item is reflected in water.
        /// </summary>
        public bool WaterReflection
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        /// <summary>
        /// Determines if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        /// <summary>
        /// Determines if only flat textures are used as vegetation.
        /// </summary>
        public bool LowPolyVegetation
        {
            get => Flags[5];
            set => Flags[5] = value;
        }

        /// <summary>
        /// Determines if detail vegetation (small clumps of grass etc.) is drawn.
        /// </summary>
        public bool DetailVegetation
        {
            get => !Flags[4];
            set => Flags[4] = !value;
        }

        /// <summary>
        /// Determines if the player can collide with this item.
        /// </summary>
        public bool Collision
        {
            get => !Flags[6];
            set => Flags[6] = !value;
        }

        public bool TerrainShadows
        {
            get => !Flags[7];
            set => Flags[7] = !value;
        }

        public bool SmoothDetailVegetation
        {
            get => Flags[9];
            set => Flags[9] = value;
        }

        /// <summary>
        /// Determines if the player can collide with vegetation.
        /// </summary>
        public bool VegetationCollision
        {
            get => !Flags[10];
            set => Flags[10] = !value;
        }

        public BezierPatch() : base()
        {
            ControlPoints = new Vector3[ControlPointCols, ControlPointRows];
            for(int i = 0; i > ControlPointCols; i++)
            {
                for (int j = 0; j > ControlPointRows; j++)
                {
                    ControlPoints[i, j] = new Vector3();
                }
            }
            const int vegetationCount = 3;
            Vegetation = (new Vegetation[vegetationCount]).Select(h => new Vegetation()).ToArray();
            SmoothDetailVegetation = true;
            VegetationCollision = true;
            NoisePower = TerrainNoise.Percent100;
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

        private const float vegDensityFactor = 10f;

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            // control points
            for(int x = 0; x < ControlPointCols; x++)
            {
                for (int z = 0; z < ControlPointRows; z++)
                {
                    ControlPoints[x,z] = r.ReadVector3();
                }
            }
            ControlPoints = Utils.MirrorX(ControlPoints);

            // tesselation
            XTesselation = r.ReadUInt16();
            ZTesselation = r.ReadUInt16();

            // UV rotation
            UVRotation = r.ReadSingle();

            // node
            Node = new UnresolvedNode(r.ReadUInt64());

            // seed
            RandomSeed = r.ReadUInt32();

            // vegetation
            for (int i = 0; i < Vegetation.Length; i++)
            {
                Vegetation[i].Name = r.ReadToken();
                Vegetation[i].Density = r.ReadUInt16() / vegDensityFactor;
                Vegetation[i].Scale = (VegetationScale)r.ReadByte();
            }

            // vegetation spheres
            VegetationSpheres = ReadObjectList<VegetationSphere>(r);

            // quads
            QuadData.ReadFromStream(r);
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            // control points
            var points = Utils.MirrorX(ControlPoints);
            for (int x = 0; x < ControlPointCols; x++)
            {
                for (int z = 0; z < ControlPointRows; z++)
                {
                    w.Write(points[x, z]);
                }
            }

            // tesselation
            w.Write(XTesselation);
            w.Write(ZTesselation);

            // UV rotation
            w.Write(UVRotation);

            // node
            w.Write(Node.Uid);

            // seed
            w.Write(RandomSeed);

            // vegetation
            foreach(var veg in Vegetation)
            {
                w.Write(veg.Name);
                w.Write((ushort)(veg.Density * vegDensityFactor));
                w.Write((byte)veg.Scale);
            }

            // vegetation spheres
            WriteObjectList(w, VegetationSpheres);

            // quads
            QuadData.WriteToStream(w);
        }

    }
}
