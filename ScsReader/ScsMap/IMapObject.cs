using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{

    /// <summary>
    /// An interface implemented by both types of objects in a map, 
    /// items (MapItem) and nodes (Node).
    /// 
    /// <para>This interface exists because cut plane nodes
    /// just HAD to be weird and reference other nodes 
    /// rather than items.</para>
    /// </summary>
    public interface IMapObject
    {
        ulong Uid { get; set; }
    }
}
