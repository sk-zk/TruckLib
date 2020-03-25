using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    interface IDataPayload
    {
        void ReadDataPart(BinaryReader r, MapItem item);
        void WriteDataPart(BinaryWriter w, MapItem item);
    }
}
