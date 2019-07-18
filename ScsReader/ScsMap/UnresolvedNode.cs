using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Used to hold a UID reference while loading map items, before 
    /// references can be resolved. Once all nodes have been read, all 
    /// instances of this class are replaced with the actual nodes.
    /// </summary>
    internal class UnresolvedNode : Node
    {
        public UnresolvedNode(ulong uid)
        {
            Uid = uid;
        }
    }
}
