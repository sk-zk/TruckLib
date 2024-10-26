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
        /// <summary>
        /// The nodes parented by this container.
        /// </summary>
        IDictionary<ulong, INode> Nodes { get; }

        /// <summary>
        /// The map items parented by this container.
        /// </summary>
        Dictionary<ulong, MapItem> MapItems { get; }

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
