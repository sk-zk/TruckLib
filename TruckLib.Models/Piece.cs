using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace TruckLib.Models
{
    public class Piece : IBinarySerializable
    {
        public List<Vertex> Vertices { get; set; } = [];

        public List<Triangle> Triangles { get; set; } = [];

        public AxisAlignedBox BoundingBox { get; set; } = new();

        public Vector3 BoundingBoxCenter { get; set; } = Vector3.Zero;

        public float BoundingBoxDiagonalSize { get; set; }

        public int TextureCoordinateWidth { get; set; } = 1;

        public uint Material { get; set; }

        public bool UseTangents { get; set; } = false;

        public bool UseSecondaryColor { get; set; } = false;

        public bool UseTextureCoordinates { get; set; } = false;

        public bool UseBoneIndexes { get; set; } = false;

        public bool UseBoneWeights { get; set; } = false;

        private const int Unused = -1;

        private int skeletonOffset_temp = 36;

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            var edges = r.ReadUInt32();
            var verts = r.ReadUInt32();
            var texCoordMask = r.ReadUInt32(); // what is this even used for?
            TextureCoordinateWidth = r.ReadInt32();
            Material = r.ReadUInt32();
            BoundingBoxCenter = r.ReadVector3();
            BoundingBoxDiagonalSize = r.ReadSingle();
            BoundingBox = new AxisAlignedBox();
            BoundingBox.Deserialize(r);

            // TODO: Why does this even point to when the
            // model has no bones??
            // used values are 36, 44 and 52?
            skeletonOffset_temp = r.ReadInt32(); 
            var vertPositionOffset = r.ReadInt32();
            var vertNormalOffset = r.ReadInt32();
            var vertTexcoordOffset = r.ReadInt32();
            var vertColorOffset = r.ReadInt32();
            var vertColor2Offset = r.ReadInt32();
            var vertTangentOffset = r.ReadInt32();
            var vertBoneIndexOffset = r.ReadInt32();
            var vertBoneWeightOffset = r.ReadInt32();
            var indexOffset = r.ReadInt32();

            UseTangents = (vertTangentOffset != Unused);
            UseSecondaryColor = (vertColor2Offset != Unused);
            UseTextureCoordinates = (vertTexcoordOffset != Unused);
            UseBoneIndexes = (vertBoneIndexOffset != Unused);
            UseBoneWeights = (vertBoneWeightOffset != Unused);

            var prevStreamPosition = r.BaseStream.Position;
            r.BaseStream.Position = vertPositionOffset;

            for (int i = 0; i < verts; i++)
            {
                var vertex = new Vertex(r.ReadVector3(), r.ReadVector3());

                if (vertTangentOffset != Unused)
                {
                    vertex.Tangent = r.ReadVector4();
                }
                vertex.Color = r.ReadColor();
                if (vertColor2Offset != Unused)
                {
                    vertex.SecondaryColor = r.ReadColor();
                }
                if (vertTexcoordOffset != Unused)
                {
                    vertex.TextureCoordinates = r.ReadObjectList<Vector2>((uint)TextureCoordinateWidth);
                }
                if (vertBoneIndexOffset != Unused)
                {
                    vertex.BoneIndexes = r.ReadBytes(4);
                }
                if (vertBoneWeightOffset != Unused)
                {
                    vertex.BoneWeights = r.ReadBytes(4);
                }

                Vertices.Add(vertex);
            }

            r.BaseStream.Position = indexOffset;
            for (int i = 0; i < edges / 3; i++)
            {
                var t = new Triangle();
                t.Deserialize(r);
                Triangles.Add(t);
            }

            r.BaseStream.Position = prevStreamPosition;
        }

        public void WriteHeaderPart(BinaryWriter w, int vertStart, int trisStart)
        {
            w.Write(Triangles.Count * 3); // Edge count
            w.Write(Vertices.Count);

            uint texCoordMask;
            texCoordMask = GetTexCoordMask();
            w.Write(texCoordMask);
            w.Write(TextureCoordinateWidth);

            w.Write(Material);

            w.Write(BoundingBoxCenter);
            w.Write(BoundingBoxDiagonalSize);
            BoundingBox.Serialize(w);

            w.Write(skeletonOffset_temp); // TODO: Skeleton offset

            // more fun with offsets
            int offset = vertStart;

            int vertPositionOffset = offset;
            offset += 3 * sizeof(float);

            int vertNormalOffset = offset;
            offset += 3 * sizeof(float);

            int vertTangentOffset;
            if (UseTangents)
            {
                vertTangentOffset = offset;
                offset += 4 * sizeof(float);
            }
            else
            {
                vertTangentOffset = Unused;
            }

            int vertColorOffset = offset;
            offset += sizeof(uint);

            int vertColor2Offset;
            if (UseSecondaryColor)
            {
                vertColor2Offset = offset;
                offset += sizeof(uint);
            }
            else
            {
                vertColor2Offset = Unused;
            }

            int vertTexcoordOffset;
            if (UseTextureCoordinates)
            {
                vertTexcoordOffset = offset;
                offset += 2 * sizeof(float) * TextureCoordinateWidth;
            }
            else
            {
                vertTexcoordOffset = Unused;
            }

            int vertBoneIndexOffset;
            if (UseBoneIndexes)
            {
                vertBoneIndexOffset = offset;
                offset += sizeof(byte) * 4;
            }
            else
            {
                vertBoneIndexOffset = Unused;
            }

            int vertBoneWeightOffset;
            if (UseBoneWeights)
            {
                vertBoneWeightOffset = offset;
                offset += sizeof(byte) * 4;
            }
            else
            {
                vertBoneWeightOffset = Unused;
            }

            w.Write(vertPositionOffset);
            w.Write(vertNormalOffset);
            w.Write(vertTexcoordOffset);
            w.Write(vertColorOffset);
            w.Write(vertColor2Offset);
            w.Write(vertTangentOffset);
            w.Write(vertBoneIndexOffset);
            w.Write(vertBoneWeightOffset);

            w.Write(trisStart);
        }

        private uint GetTexCoordMask()
        {
            uint texCoordMask;
            switch (TextureCoordinateWidth)
            {
                case 1:
                    texCoordMask = 0xFFFFFFF0;
                    break;
                case 2:
                    texCoordMask = 0xFFFFFF10;
                    break;
                case 3:
                    texCoordMask = 0xFFFFF210;
                    break;
                default:
                    throw new NotImplementedException("Turns out that a tex coord width of "
                    + $"{TextureCoordinateWidth} exists and that I need to reverse engineer this "
                    + "properly after all.");
            }

            return texCoordMask;
        }

        public void WriteVertPart(BinaryWriter w)
        {
            foreach (var vert in Vertices)
            {
                w.Write(vert.Position);
                w.Write(vert.Normal);
                if (UseTangents) w.Write(vert.Tangent.Value);
                w.Write(vert.Color);
                if (UseSecondaryColor) w.Write(vert.SecondaryColor.Value);
                if (UseTextureCoordinates) w.WriteObjectList(vert.TextureCoordinates.Take(TextureCoordinateWidth).ToList());
                if (UseBoneIndexes) w.Write(vert.BoneIndexes);
                if (UseBoneWeights) w.Write(vert.BoneWeights);
            }
        }

        public void WriteTriangles(BinaryWriter w)
        {
            w.WriteObjectList(Triangles);
        }

        public void Serialize(BinaryWriter w)
        {
            throw new NotImplementedException();
        }

    }
}
