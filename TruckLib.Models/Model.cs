using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace TruckLib.Models
{
    // hi r/badcode

    public class Model
    {
        private const int PmdVersion = 4;
        private const string PmdExtension = "pmd";

        private const byte PmgVersion = 0x15;
        private readonly char[] PmgSignature = ['g', 'm', 'P'];
        private const string PmgExtension = "pmg";

        public string Name { get; set; } = "untitled";

        public List<Look> Looks { get; set; } = [];

        public List<Variant> Variants { get; set; } = [];

        public AxisAlignedBox BoundingBox { get; set; } = new();

        public Vector3 BoundingBoxCenter { get; set; } = Vector3.Zero;

        public float BoundingBoxDiagonalSize { get; set; }

        public List<Bone> Skeleton { get; set; } = [];

        public List<Part> Parts { get; set; } = [];

        private List<string> strings = [];

        public Model()
        {
            Looks.Add(new Look("default"));
            Variants.Add(new Variant("default"));
        }

        public Model(string name) : this()
        {
            Name = name;
        }

        public static Model Open(string pmdPath)
        {
            var name = Path.GetFileNameWithoutExtension(pmdPath);
            var model = new Model(name);

            using (var r = new BinaryReader(new FileStream(pmdPath, FileMode.Open)))
            {
                model.ReadPmd(r);
            }

            var pmgPath = Path.ChangeExtension(pmdPath, PmgExtension);
            using (var r = new BinaryReader(new FileStream(pmgPath, FileMode.Open)))
            {
                model.ReadPmg(r);
            }

            return model;
        }

        public void Save(string directory)
        {
            var pmdPath = Path.Combine(directory, Name + "." + PmdExtension);
            using (var w = new BinaryWriter(new FileStream(pmdPath, FileMode.Create)))
            {
                WritePmd(w);
            }

            var pmgPath = Path.ChangeExtension(pmdPath, PmgExtension);
            using (var w = new BinaryWriter(new FileStream(pmgPath, FileMode.Create)))
            {
                WritePmg(w);
            }
        }

        private void ReadPmd(BinaryReader r)
        {
            // specifically check if the user passed in a pmg file by accident
            r.BaseStream.Position = 1;
            var pmgSigCheck = r.ReadChars(3);
            if (Enumerable.SequenceEqual(pmgSigCheck, PmgSignature))
            {
                throw new ArgumentException("This is a .pmg file, not a .pmd file.");
            }
            r.BaseStream.Position = 0;

            var version = r.ReadUInt32();
            if (version != PmdVersion)
            {
                throw new UnsupportedVersionException($".pmd version {version} is not supported.");
            }

            var materialCount = r.ReadUInt32();
            var lookCount = r.ReadUInt32();
            var pieceCount = r.ReadUInt32();
            var variantCount = r.ReadUInt32();
            var partCount = r.ReadUInt32();
            var attribsCount = r.ReadUInt32();

            var attribsValuesSize = r.ReadUInt32();
            var materialBlockSize = r.ReadUInt32();

            var lookOffset = r.ReadUInt32();
            var variantOffset = r.ReadUInt32();
            var partAttribsOffset = r.ReadUInt32();
            var attribsValuesOffset = r.ReadUInt32();
            var attribsHeaderOffset = r.ReadUInt32();
            var materialOffsetsOffset = r.ReadUInt32();
            var materialDataOffset = r.ReadUInt32();

            // look names
            Looks.Clear();
            for (int i = 0; i < lookCount; i++)
            {
                Looks.Add(new Look(r.ReadToken()));
            }

            // variant names
            Variants.Clear();
            for (int i = 0; i < variantCount; i++)
            {
                Variants.Add(new Variant(r.ReadToken()));
            }

            // "partAttribs"
            // TODO: what is this?
            for (int i = 0; i < partCount; i++)
            {
                var from = r.ReadInt32();
                var to = r.ReadInt32();
            }

            // attribs header
            // each variant has the same attribs
            for (int i = 0; i < attribsCount; i++)
            {
                var name = r.ReadToken();
                var type = r.ReadInt32();
                var offset = r.ReadInt32();
                foreach (var variant in Variants)
                {
                    var attrib = new PartAttribute();
                    variant.Attributes.Add(attrib);
                    attrib.Tag = name;
                    attrib.Type = type;
                }
            }

            // attribs values
            // TODO: Find out if there are any files where a part has 
            // more than one attrib or if "visible" is actually the only attrib
            // that exists
            for (int i = 0; i < variantCount; i++)
            {
                for (int j = 0; j < attribsCount; j++)
                {
                    Variants[i].Attributes[j].Value = r.ReadUInt32();
                }
            }

            // material offsets; I think we can get away with ignoring this?
            var materialsOffset = new List<uint>();
            for (int i = 0; i < lookCount * materialCount; i++)
            {
                materialsOffset.Add(r.ReadUInt32());
            }

            // look material paths
            var materialsData = r.ReadBytes((int)materialBlockSize);
            var materials = StringUtils.CStringBytesToList(materialsData);
            for(int i = 0; i < Looks.Count; i++)
            {
                Looks[i].Materials.AddRange(
                    materials.GetRange(i * (int)materialCount, (int)materialCount)
                    );
            }
        }

        private void ReadPmg(BinaryReader r)
        {
            var version = r.ReadByte();
            if (version != PmgVersion)
            {
                throw new UnsupportedVersionException($".pmg version {version} is not supported.");
            }

            var signature = r.ReadChars(3);
            if (!Enumerable.SequenceEqual(signature, PmgSignature))
            {
                throw new InvalidDataException($"Probably not a pmg file");
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
            BoundingBox.Deserialize(r);

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

            // jump ahead and read all locators & pieces first
            r.BaseStream.Position = locatorsOffset;
            var locators = r.ReadObjectList<Locator>(locatorCount);
            var pieces = r.ReadObjectList<Piece>(pieceCount);

            // then return to parts and assign the locators and pieces right away
            r.BaseStream.Position = partsOffset;
            for (int i = 0; i < partCount; i++)
            {
                var part = new Part();
                part.Name = r.ReadToken();

                var piecesCount = r.ReadUInt32();
                var piecesIndex = r.ReadUInt32();
                part.Pieces = pieces.GetRange((int)piecesIndex, (int)piecesCount);

                var locatorsCount = r.ReadUInt32();
                var locatorsIndex = r.ReadUInt32();
                part.Locators = locators.GetRange((int)locatorsIndex, (int)locatorsCount);

                Parts.Add(part);
            }

            // TODO: what is this?
            r.BaseStream.Position = stringPoolOffset;
            if (stringPoolSize > 0)
            {
                r.BaseStream.Position = stringPoolOffset;
                var stringsBytes = r.ReadBytes((int)stringPoolSize);
                strings = StringUtils.CStringBytesToList(stringsBytes);
            }
        }

        private void WritePmd(BinaryWriter w)
        {
            w.Write(PmdVersion);

            w.Write(Looks[0].Materials.Count);
            w.Write(Looks.Count);
            w.Write(Parts.Sum(x => x.Pieces.Count)); // TODO: Why is the pmd piece count different from the pmg piece count?
            w.Write(Variants.Count);
            w.Write(Parts.Count);
            w.Write(Parts.Count); // attribs count

            byte[] lookNames = WriteToByteArray((_w) =>
            {
                foreach (var look in Looks)
                {
                    _w.Write(look.Name);
                }
            });

            byte[] variantNames = WriteToByteArray((_w) =>
            {
                foreach (var variant in Variants)
                {
                    _w.Write(variant.Name);
                }
            });

            // placeholder code
            byte[] partAttribs = WriteToByteArray((_w) =>
            {
                for (int i = 0; i < Parts.Count; i++)
                {
                    _w.Write(i);
                    _w.Write(i + 1);
                }
            });

            byte[] attribsHeader = WriteToByteArray((_w) =>
            {
                var offset = 0;
                foreach (var attrib in Variants[0].Attributes)
                {
                    _w.Write(attrib.Tag);
                    _w.Write(attrib.Type);
                    _w.Write(offset);
                    offset += 4;
                }
            });

            byte[] attribsValues = WriteToByteArray((_w) =>
            {
                foreach (var variant in Variants)
                {
                    foreach (var attrib in variant.Attributes)
                    {
                        _w.Write(attrib.Value);
                    }
                }
            });

            // materials offsets
            var materialOffsetsLength = Looks[0].Materials.Count * sizeof(int);

            var materialStrs = Looks.Select(x => x.Materials).SelectMany(x => x).ToList();
            var materials = StringUtils.ListToCStringByteList(materialStrs);

            w.Write(attribsValues.Length / Variants.Count);
            w.Write(materials.Sum(x => x.Length));

            var offsetPartLength = 7 * sizeof(int);

            int lookNamesOffset = (int)w.BaseStream.Position + offsetPartLength;
            w.Write(lookNamesOffset);

            int variantNamesOffset = lookNamesOffset + lookNames.Length;
            w.Write(variantNamesOffset);

            int partAttribsOffset = variantNamesOffset + variantNames.Length;
            w.Write(partAttribsOffset);

            int attribsHeaderOffset = partAttribsOffset + partAttribs.Length;
            int attribsValuesOffset = attribsHeaderOffset + attribsHeader.Length;
            w.Write(attribsValuesOffset);
            w.Write(attribsHeaderOffset);

            int materialOffsetsOffset = attribsValuesOffset + attribsValues.Length;
            w.Write(materialOffsetsOffset);

            int materialDataOffset = materialOffsetsOffset + materialOffsetsLength;
            w.Write(materialDataOffset);

            w.Write(lookNames);
            w.Write(variantNames);
            w.Write(partAttribs);
            w.Write(attribsHeader);
            w.Write(attribsValues);

            var materialOffset = (int)w.BaseStream.Position + (materials.Count * sizeof(int));
            for (int i = 0; i < materials.Count; i++)
            {
                w.Write(materialOffset);
                materialOffset += materials[i].Length;
            }

            foreach (var str in materials)
            {
                w.Write(str);
            }
        }

        private void WritePmg(BinaryWriter w)
        {
            w.Write(PmgVersion);

            w.Write(Encoding.ASCII.GetBytes(PmgSignature).Reverse().ToArray());

            w.Write(Parts.Sum(x => x.Pieces.Count));
            w.Write(Parts.Count);
            w.Write(Skeleton.Count);
            w.Write(0); // TODO: What is "weight width"?
            w.Write(Parts.Sum(x => x.Locators.Count));
            w.Write(Skeleton.Count == 0 ? 0UL : 0UL); // TODO: How is the "skeleton hash" calculated?

            w.Write(BoundingBoxCenter);
            w.Write(BoundingBoxDiagonalSize);
            BoundingBox.Serialize(w);

            // from this point onward we need to deal with offset vals
            // which pretty much breaks my workflow but I can't be bothered
            // to do it properly

            // start from the bottom with the triangles,
            // write everything to byte[]s, get the offsets,
            // then output everything in the correct order

            var offsetSectionLength = sizeof(int) * 10;

            byte[] skeleton = ListAsByteArray(Skeleton);

            byte[] parts = WriteToByteArray((_w) =>
            {
                int pieceIndex = 0;
                int locatorIndex = 0;
                foreach (var part in Parts)
                {
                    _w.Write(part.Name);
                    _w.Write(part.Pieces.Count);
                    _w.Write(pieceIndex);
                    pieceIndex += part.Pieces.Count;
                    _w.Write(part.Locators.Count);
                    _w.Write(locatorIndex);
                    locatorIndex += part.Locators.Count;
                }
            });

            byte[] locators = ListAsByteArray(
                Parts.Select(x => x.Locators).SelectMany(x => x).ToList()
                );

            var pieces = Parts.Select(x => x.Pieces).SelectMany(x => x).ToList();

            // index pool
            var piecesTris = new List<byte[]>();
            foreach (var piece in pieces)
            {
                piecesTris.Add(
                    WriteToByteArray((_w) =>
                    {
                        piece.WriteTriangles(_w);
                    })
                );
            }

            // vert pool
            var piecesVerts = new List<byte[]>();
            foreach (var piece in pieces)
            {
                piecesVerts.Add(
                    WriteToByteArray((_w) =>
                    {
                        piece.WriteVertPart(_w);
                    })
                 );
            }

            // string pool
            var stringPool = StringUtils.ListToCStringByteList(strings);           

            // pieces header
            // first, get the byte length of the pieces segment
            // because we need it to calculate the vert*Offset values.
            var skeletonOffset = (int)w.BaseStream.Position + offsetSectionLength;
            var partsOffset = skeletonOffset + skeleton.Length;
            var locatorsOffset = partsOffset + parts.Length;
            var pieceHeaderOffset = locatorsOffset + locators.Length;
            var stringStart = (int)(pieceHeaderOffset + GetLengthOfPieceIndex(pieces));
            var vertStart = stringStart + stringPool.Sum(x => x.Length);
            var trisStart = vertStart + piecesVerts.Sum(x => x.Length);

            // then actually get the bytes
            byte[] piecesHeader = WriteToByteArray((_w) =>
            {
                var currVert = vertStart;
                var currTris = trisStart;
                for (int i = 0; i < pieces.Count; i++)
                {
                    pieces[i].WriteHeaderPart(_w, currVert, currTris);
                    currVert += piecesVerts[i].Length;
                    currTris += piecesTris[i].Length;
                }
            });

            // and now we can finally write everything
            w.Write(skeletonOffset);
            w.Write(partsOffset);
            w.Write(locatorsOffset);
            w.Write(pieceHeaderOffset);

            w.Write(stringStart);
            w.Write(strings is null ? 0 : stringPool.Sum(x => x.Length));
            w.Write(vertStart);
            w.Write(piecesVerts.Sum(x => x.Length));
            w.Write(trisStart);
            w.Write(piecesTris.Sum(x => x.Length));

            w.Write(skeleton);
            w.Write(parts);
            w.Write(locators);
            w.Write(piecesHeader);

            foreach (var str in stringPool)
            {
                w.Write(str);
            }
            foreach (var vert in piecesVerts)
            {
                w.Write(vert);
            }
            foreach (var tri in piecesTris)
            {
                w.Write(tri);
            }
        }

        private byte[] WriteToByteArray(Action<BinaryWriter> action)
        {
            byte[] arr;
            using (var ms = new MemoryStream())
            using (var w = new BinaryWriter(ms))
            {
                action(w);
                arr = ms.ToArray();
            }
            return arr;
        }

        private long GetLengthOfPieceIndex(List<Piece> pieces)
        {
            long length = 0;
            using (var ms = new MemoryStream())
            using (var w2 = new BinaryWriter(ms))
            {
                foreach (var piece in pieces)
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
