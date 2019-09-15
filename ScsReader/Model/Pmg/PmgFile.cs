using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ScsReader.Model.Pmg
{
    /// <summary>
    /// Prism Model Geometry.
    /// </summary>
    public class PmgFile : IBinarySerializable
    {
        private byte Version = 0x15;
        private readonly string Signature = "Pmg";

        public List<Bone> Skeleton { get; set; } = new List<Bone>();

        public List<Part> Parts { get; set; } = new List<Part>();

        public List<Locator> Locators { get; set; } = new List<Locator>();

        public List<Piece> Pieces { get; set; } = new List<Piece>();

        public Vector3 BoundingBoxCenter { get; set; }

        public float BoundingBoxDiagonalSize { get; set; }

        public AxisAlignedBox BoundingBox { get; set; }

        public void Open(string path)
        {
            using (var r = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                ReadFromStream(r);
            }
        }

        public void ReadFromStream(BinaryReader r)
        {
            Version = r.ReadByte();

            var signature = Encoding.ASCII.GetString(r.ReadBytes(3).Reverse().ToArray());
            if(signature != Signature)
            {
                throw new InvalidDataException($"Not a pmg file? Expected '{Signature}', got '{signature}'");
            }

            var pieceCount = r.ReadUInt32();
            var partCount = r.ReadUInt32();
            var boneCount = r.ReadUInt32();
            var weightWidth = r.ReadInt32();
            var locatorCount = r.ReadUInt32();
            var skeletonHash = r.ReadUInt64();

            BoundingBoxCenter = r.ReadVector3();
            BoundingBoxDiagonalSize = r.ReadSingle();
            BoundingBox = new AxisAlignedBox();
            BoundingBox.ReadFromStream(r);

            var skeletonOffset = r.ReadUInt32();
            var partsOffset = r.ReadUInt32();
            var locatorsOffset = r.ReadUInt32();
            var piecesOffset = r.ReadUInt32();

            var stringPoolOffset = r.ReadUInt32();
            var stringPoolSize = r.ReadUInt32();
            var vertexPoolOffset = r.ReadUInt32();
            var vertexPoolSize = r.ReadUInt32();
            var indexPoolOffset = r.ReadUInt32();
            var indexPoolSize = r.ReadUInt32();

            Skeleton = r.ReadObjectList<Bone>(boneCount);
            Parts = r.ReadObjectList<Part>(partCount);
            Locators = r.ReadObjectList<Locator>(locatorCount);

            // ! BE ADVISED ! 
            // I usually prefer deserialization to be forward-only,
            // but with this particular file structure, the reader 
            // has to jump back and forth a few times
            Pieces = r.ReadObjectList<Piece>(pieceCount);

            // TODO: deal with these.
            // referenced by Locator.HookupOffset I think??
            r.BaseStream.Position = stringPoolOffset;
            var stringsBytes = r.ReadBytes((int)stringPoolSize);
            var strings = Encoding.ASCII.GetString(stringsBytes).Split(
                new char[] { '\0' }, StringSplitOptions.None);
        }

        public void WriteToStream(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}
