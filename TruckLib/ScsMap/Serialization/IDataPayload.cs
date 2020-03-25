using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    interface IDataPayload
    {
        void DeserializeDataPayload(BinaryReader r, MapItem item);
        void SerializeDataPayload(BinaryWriter w, MapItem item);
    }
}
