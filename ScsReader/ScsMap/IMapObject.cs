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
    /// </summary>
    public interface IMapObject
    {
        ulong Uid { get; set; }
    }
}
