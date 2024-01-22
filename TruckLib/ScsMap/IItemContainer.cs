using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Interface for classes that hold map items and nodes: <see cref="Map"/>, <see cref="Compound"/>
    /// and <see cref="Selection"/>.
    /// </summary>
    public interface IItemContainer
    {
        // This interface has to exist because Compounds hold items and nodes
        // themselves rather than holding references to top level objects (like
        // every other item in the game).

        /// <summary>
        /// Returns a dictionary containing all map items in the container.
        /// </summary>
        /// <returns>All map items in the container.</returns>
        Dictionary<ulong, MapItem> GetAllItems();

        /// <summary>
        /// Returns a dictionary containing all items of type T in the container.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <returns>All items of this type in the container.</returns>
        Dictionary<ulong, T> GetAllItems<T>() where T : MapItem;

        /// <summary>
        /// Returns a dictionary containing all nodes in the container.
        /// </summary>
        /// <returns>All nodes in the container.</returns>
        Dictionary<ulong, INode> GetAllNodes();

        /// <summary>
        /// Adds a node to the container.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <param name="isRed">Whether the node is red.</param>
        /// <returns>The new node.</returns>
        Node AddNode(Vector3 position, bool isRed);

        /// <summary>
        /// Adds a node to the container.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <returns>The new node.</returns>
        Node AddNode(Vector3 position);

        /// <summary>
        /// Adds an item to the map. This is the final step in the Add() method of an item
        /// and should not be called on its own.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void AddItem(MapItem item);

        /// <summary>
        /// Deletes an item from the container.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(MapItem item);

        /// <summary>
        /// Deletes a node from the container.
        /// </summary>
        /// <param name="node">The node.</param>
        void Delete(INode node);
    }
}
