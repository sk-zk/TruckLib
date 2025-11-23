using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Holds a UID reference while loading map items (before 
    /// references can be resolved). Once all nodes have been read, all 
    /// instances of this class are replaced with the actual nodes 
    /// (if they exist).
    /// </summary>
    internal struct UnresolvedNode : INode
    {
        public ulong Uid { get; set; }
        public UnresolvedNode(ulong uid)
        {
            Uid = uid;
        }

        #region Nothing to see here
        public byte BackwardCountry { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }
        public IMapObject BackwardItem { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }
        public byte ForwardCountry { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }
        public IMapObject ForwardItem { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }
        public bool FreeRotation { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }
        public bool IsCountryBorder { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }
        public bool IsRed { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }
        public bool Locked { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }
        public Vector3 Position { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }
        public Quaternion Rotation { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }
        public IItemContainer Parent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsCurveLocator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool PlayerVehicleTypeChange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool FwdTruck { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool FwdBus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool FwdCar { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool BwdTruck { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool BwdBus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool BwdCar { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsOrphaned() => throw new InvalidOperationException();
        public void Move(Vector3 newPos) => throw new InvalidOperationException();
        public void Merge(INode n2) => throw new NotImplementedException();
        public INode Split() => throw new NotImplementedException();

        public void Translate(Vector3 translation) => throw new InvalidOperationException();
        public void Deserialize(BinaryReader r, uint? version = null) => throw new InvalidOperationException();
        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems) => throw new InvalidOperationException();
        public void Serialize(BinaryWriter w) => throw new InvalidOperationException();

        #endregion
    }
}
