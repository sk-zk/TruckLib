using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Text;

namespace ScsReader.Model.Pmg
{
    public class Piece : IBinarySerializable
    {
        public List<Vertex> Vertices { get; set; } = new List<Vertex>();

        public List<Triangle> Triangles { get; set; } = new List<Triangle>();

        public Vector3 BoundingBoxCenter { get; set; }

        public float BoundingBoxDiagonalSize { get; set; }

        public AxisAlignedBox BoundingBox { get; set; }

        /// <summary>
        /// ! BE ADVISED ! <br />
        /// I usually prefer deserialization to be forward-only,
        /// but with this particular file structure, the reader 
        /// has to jump back and forth a few times.
        /// </summary>
        /// <param name="r"></param>
        public void ReadFromStream(BinaryReader r)
        {
            var edges = r.ReadUInt32();
            var verts = r.ReadUInt32();
            var texCoordMask = r.ReadUInt32();
            var texCoordWidth = r.ReadInt32();
            var material = r.ReadUInt32();
            BoundingBoxCenter = r.ReadVector3();
            BoundingBoxDiagonalSize = r.ReadSingle();
            var BoundingBox = new AxisAlignedBox();
            BoundingBox.ReadFromStream(r);

            var skeletonOffset = r.ReadInt32();
            var vertPositionOffset = r.ReadInt32();
            var vertNormalOffset = r.ReadInt32();
            var vertTexcoordOffset = r.ReadInt32();
            var vertColorOffset = r.ReadInt32();
            var vertColor2Offset = r.ReadInt32();
            var vertTangentOffset = r.ReadInt32();
            var vertBoneIndexOffset = r.ReadInt32();
            var vertBoneWeightOffset = r.ReadInt32();
            var indexOffset = r.ReadInt32();

            var prevStreamPosition = r.BaseStream.Position;
            r.BaseStream.Position = vertPositionOffset;

            for (int i = 0; i < verts; i++)
            {
                var vertex = new Vertex();
                vertex.Position = r.ReadVector3();
                vertex.Normal = r.ReadVector3();
                if (vertTangentOffset != -1)
                {
                    vertex.Tangent = r.ReadVector4();
                }
                vertex.Color = r.ReadColor();
                if (vertColor2Offset != -1)
                {
                    vertex.SecondaryColor = r.ReadColor();
                }
                if (vertTexcoordOffset != -1)
                {
                    vertex.TextureCoordinates = r.ReadObjectList<Vector2>((uint)texCoordWidth);
                }
                if (vertBoneIndexOffset != -1)
                {
                    vertex.BoneIndexes = r.ReadBytes(4);
                }
                if (vertBoneWeightOffset != -1)
                {
                    vertex.BoneWeights = r.ReadBytes(4);
                }

                Vertices.Add(vertex);
            }

            r.BaseStream.Position = indexOffset;
            for (int i = 0; i < edges / 3; i++)
            {
                var t = new Triangle();
                t.ReadFromStream(r);
                Triangles.Add(t);
            }

            Console.WriteLine(r.BaseStream.Position);
            r.BaseStream.Position = prevStreamPosition;
        }

        public void ReadSecondPart(BinaryReader r)
        {

        }

        public void WriteToStream(BinaryWriter w)
        {
            throw new NotImplementedException();
        }

    }
}
