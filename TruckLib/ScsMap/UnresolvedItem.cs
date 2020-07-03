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
    /// references can be resolved). Once all items have been read, all 
    /// instances of this class are replaced with the actual items 
    /// (if they exist).
    /// </summary>
    internal struct UnresolvedItem : IMapItem
    {
        public ulong Uid { get; set; }

        public UnresolvedItem(ulong uid)
        {
            Uid = uid;
        }

        public ItemFile DefaultItemFile => throw new InvalidOperationException();
        public ItemFile ItemFile => throw new InvalidOperationException();
        public ItemType ItemType => throw new InvalidOperationException();

        public void Move(Vector3 newPos) => throw new InvalidOperationException();
        public void Translate(Vector3 translation) => throw new InvalidOperationException();
    }
}
