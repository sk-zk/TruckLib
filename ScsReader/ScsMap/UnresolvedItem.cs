using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Used to hold a UID reference while loading map items, before 
    /// references can be resolved. Once all items have been read, all 
    /// instances of this class are replaced with the actual items.
    /// </summary>
    internal class UnresolvedItem : MapItem
    {
        readonly string exceptionMessage = "This item is unresolved.";

        public override ItemType ItemType => throw new InvalidOperationException(exceptionMessage);
        public override ItemFile DefaultItemFile => throw new InvalidOperationException(exceptionMessage);
        protected override ushort DefaultViewDistance => throw new InvalidOperationException(exceptionMessage);

        public UnresolvedItem()
        {
        }

        public UnresolvedItem(ulong uid)
        {
            Uid = uid;
        }

        public override void ReadFromStream(BinaryReader r)
        {
            Uid = r.ReadUInt64();
        }

        public override void WriteToStream(BinaryWriter w)
        {
            throw new InvalidOperationException(exceptionMessage);
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            throw new InvalidOperationException(exceptionMessage);
        }

    }
}
