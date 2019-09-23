using System;
using System.Collections.Generic;
using System.Linq;
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
    internal sealed class UnresolvedNode : Node
    {
        public UnresolvedNode(ulong uid)
        {
            Uid = uid;
        }
    }
}
