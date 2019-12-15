using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Interface for classes that hold map items and nodes (meaning Map and Compound).
    /// </summary>
    public interface IItemContainer
    {
        // This interface has to exist because Compounds hold items and nodes
        // themselves rather than holding references to top level objects (like
        // every other item in the game).

        Dictionary<ulong, MapItem> GetAllItems();

        List<T> GetAllItems<T>() where T : MapItem;

        Dictionary<ulong, Node> GetAllNodes();

        Node AddNode(Vector3 position, bool isRed);

        Node AddNode(Vector3 position);

        /// <summary>
        /// Adds an item to the map. This is the final step in the Add() method of an item
        /// and should not be called on its own.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="mainNode">The main node of the item. This will determine which sector
        /// contains the item.</param>
        internal void AddItem(MapItem item, Node mainNode);

        void Delete(MapItem item);

        void Delete(Node node);
    }
}
