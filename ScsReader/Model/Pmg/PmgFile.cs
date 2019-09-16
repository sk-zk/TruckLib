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

        private List<string> strings = new List<string>();

        public void Open(string path)
        {
            using (var r = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                ReadFromStream(r);
            }
        }

        public void Save(string path)
        {
            using(var w = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                WriteToStream(w);
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
            if (stringPoolSize > 0)
            {
                r.BaseStream.Position = stringPoolOffset;
                var stringsBytes = r.ReadBytes((int)stringPoolSize);
                strings = StringUtils.CStringBytesToList(stringsBytes);
            }
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(Version);

            w.Write(Encoding.ASCII.GetBytes(Signature).Reverse().ToArray());

            w.Write(Pieces.Count);
            w.Write(Parts.Count);
            w.Write(Skeleton.Count);
            w.Write(0); // TODO: What is "weight width"?
            w.Write(Locators.Count);
            w.Write(Skeleton.Count == 0 ? 0UL : 0UL); // TODO: How is the "skeleton hash" calculated?

            w.Write(BoundingBoxCenter);
            w.Write(BoundingBoxDiagonalSize);
            BoundingBox.WriteToStream(w);

            // from this point onward we need to deal with offset vals
            // which pretty much breaks my workflow but I can't be bothered
            // to do it properly

            // start from the bottom with the triangles,
            // write everything to byte[]s, get the offsets,
            // then output everything in the correct order

            var offsetSectionLength = sizeof(int) * 10;

            byte[] skeleton = ListAsByteArray(Skeleton);
            byte[] parts = ListAsByteArray(Parts);
            byte[] locators = ListAsByteArray(Locators);

            // index pool
            var piecesTris = new List<byte[]>();
            foreach (var piece in Pieces)
            {
                using (var ms = new MemoryStream())
                using (var w2 = new BinaryWriter(ms))
                {
                    piece.WriteTriangles(w2);
                    piecesTris.Add(ms.ToArray());
                }
            }

            // vert pool
            var piecesVerts = new List<byte[]>();
            foreach (var piece in Pieces)
            {
                using (var ms = new MemoryStream())
                using (var w2 = new BinaryWriter(ms))
                {
                    piece.WriteVertPart(w2);
                    piecesVerts.Add(ms.ToArray());
                }
            }

            // string pool
            byte[] stringPool = new byte[0];
            if(strings != null && strings.Count > 0)
            {
                List<byte> bytes = new List<byte>(); 
                foreach (var str in strings)
                {
                    bytes.AddRange(Encoding.ASCII.GetBytes(str + '\0'));
                }
                stringPool = bytes.ToArray();
            }

            // pieces header
            // first, get the byte length of the pieces segment
            // because we need it to calculate the vert*Offset values.
            var skeletonOffset = (int)w.BaseStream.Position + offsetSectionLength;
            var partsOffset = skeletonOffset + skeleton.Length;
            var locatorsOffset = partsOffset + parts.Length;
            var pieceHeaderOffset = locatorsOffset + locators.Length;
            var stringStart = (int)(pieceHeaderOffset + GetLengthOfPieceIndex());
            var vertStart = stringStart + stringPool.Length;
            var trisStart = vertStart + piecesVerts.Sum(x => x.Length);

            // then actually get the bytes
            byte[] piecesHeader;
            using (var ms = new MemoryStream())
            using (var w2 = new BinaryWriter(ms))
            {
                var currVert = vertStart;
                var currTris = trisStart;
                for (int i = 0; i < Pieces.Count; i++)
                {
                    Pieces[i].WriteHeaderPart(w2, currVert, currTris);
                    currVert += piecesVerts[i].Length;
                    currTris += piecesTris[i].Length;
                }
                piecesHeader = ms.ToArray();
            }

            // and now we can finally write everything
            w.Write(skeletonOffset);
            w.Write(partsOffset);
            w.Write(locatorsOffset);
            w.Write(pieceHeaderOffset);

            w.Write(stringStart);
            w.Write(strings is null ? 0 : stringPool.Length);
            w.Write(vertStart);
            w.Write(piecesVerts.Sum(x => x.Length));
            w.Write(trisStart);
            w.Write(piecesTris.Sum(x => x.Length));

            w.Write(skeleton);
            w.Write(parts);
            w.Write(locators);
            w.Write(piecesHeader);

            w.Write(stringPool);
            foreach (var vert in piecesVerts)
            {
                w.Write(vert);
            }
            foreach (var tri in piecesTris)
            {
                w.Write(tri);
            }
        }

        private long GetLengthOfPieceIndex()
        {
            long length = 0;
            using (var ms = new MemoryStream())
            using (var w2 = new BinaryWriter(ms))
            {
                foreach (var piece in Pieces)
                {
                    piece.WriteHeaderPart(w2, 0, 0);
                }

                length += ms.Length;
            }
            return length;
        }

        private byte[] ListAsByteArray<T>(List<T> list)
        {
            byte[] array;
            using (var ms = new MemoryStream())
            using (var w2 = new BinaryWriter(ms))
            {
                w2.WriteObjectList(list);
                array = ms.ToArray();
            }
            return array;
        }
    }
}
