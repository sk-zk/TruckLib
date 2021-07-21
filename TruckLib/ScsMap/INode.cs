using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace TruckLib.ScsMap
{
    public interface INode : IMapObject
    {
        byte BackwardCountry { get; set; }
        IMapObject BackwardItem { get; set; }
        byte ForwardCountry { get; set; }
        IMapObject ForwardItem { get; set; }
        bool FreeRotation { get; set; }
        bool IsCountryBorder { get; set; }
        bool IsRed { get; set; }
        bool Locked { get; set; }
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        Sector[] Sectors { get; set; }

        bool IsOrphaned();
        void Move(Vector3 newPos);
        void Deserialize(BinaryReader r);
        string ToString();
        void UpdateItemReferences(Dictionary<ulong, MapItem> allItems);
        void Serialize(BinaryWriter w);
    }
}