using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScsReader.Model.Pmg
{
    public class Part : IBinarySerializable
    {
        public Token Name { get; set; }

        public uint PieceCount { get; set; }

        public uint PiecesIndex { get; set; }

        public uint LocatorCount { get; set; }

        public uint LocatorsIndex { get; set; }

        public override string ToString()
        {
            return Name.String;
        }

        public void ReadFromStream(BinaryReader r)
        {
            Name = r.ReadToken();
            PieceCount = r.ReadUInt32();
            PiecesIndex = r.ReadUInt32();
            LocatorCount = r.ReadUInt32();
            LocatorsIndex = r.ReadUInt32();
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(Name);
            w.Write(PieceCount);
            w.Write(PiecesIndex);
            w.Write(LocatorCount);
            w.Write(LocatorsIndex);
        }
    }
}
