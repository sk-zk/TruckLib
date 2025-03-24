using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace TruckLib.ScsMap
{
    public interface INode : IMapObject, IBinarySerializable
    {
        /// <summary>
        /// The country of the backward item if this node is a country border.
        /// </summary>
        byte BackwardCountry { get; set; }

        /// <summary>
        /// The backward item belonging to this node. 
        /// </summary>
        IMapObject BackwardItem { get; set; }

        /// <summary>
        /// The country of the forward item if this node is a country border.
        /// </summary>
        byte ForwardCountry { get; set; }

        /// <summary>
        /// The forward item belonging to this node. 
        /// </summary>
        IMapObject ForwardItem { get; set; }

        /// <summary>
        /// Gets or sets if the game will use whichever rotation is specified without
        /// reverting to its default rotation when the node is updated.
        /// </summary>
        bool FreeRotation { get; set; }

        /// <summary>
        /// Gets or sets if this node is a country border.
        /// </summary>
        bool IsCountryBorder { get; set; }

        /// <summary>
        /// Gets or sets if this node is red or green.
        /// </summary>
        bool IsRed { get; set; }

        /// <summary>
        /// Gets or sets if this node can be moved or deleted in the editor.
        /// </summary>
        bool Locked { get; set; }

        /// <summary>
        /// Position of the node. Note that this will be serialized as fixed point values.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Rotation of the node.
        /// </summary>
        Quaternion Rotation { get; set; }

        /// <summary>
        /// The map, selection or compound which contains this node.
        /// </summary>
        IItemContainer Parent { get; set; }

        /// <summary>
        /// Gets or sets if this node represents a curve locator.
        /// </summary>
        bool IsCurveLocator { get; set; }


        /// <summary>
        /// Returns whether the node has no ForwardItem and no BackwardItem.
        /// </summary>
        /// <returns>Whether the node has no ForwardItem and no BackwardItem.</returns>
        bool IsOrphaned();

        /// <summary>
        /// Moves the node to another position.
        /// </summary>
        /// <param name="newPos">The new position of the node.</param>
        void Move(Vector3 newPos);

        /// <summary>
        /// Translates the node by the given vector.
        /// </summary>
        /// <param name="translation">The translation vector.</param>
        void Translate(Vector3 translation);

        /// <summary>
        /// Merges the node <i>n2</i> into this one, if possible, and then deletes <i>n2</i>.
        /// Note that the flags and rotation of <i>n2</i> will not be preserved.
        /// </summary>
        /// <param name="n2">The node to merge into this one.</param>
        /// <exception cref="InvalidOperationException">Thrown if the nodes can't be merged.</exception>
        void Merge(INode n2);

        /// <summary>
        /// Splits this node into one node holding the backward item and one node 
        /// holding the forward item.
        /// This method is the opposite of <see cref="Merge(INode)">Merge</see>.
        /// </summary>
        /// <returns>The newly created node. If no action was performed because
        /// this node is already only used by one item, the method returns null.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the node can't be split.</exception>
        INode Split();

        /// <inheritdoc/>
        string ToString();

        /// <summary>
        /// Searches a list of all map items for the map items referenced by UID by this node
        /// and updates the respective references.
        /// </summary>
        /// <param name="allItems">A dictionary of all items in the entire map.</param>
        void UpdateItemReferences(Dictionary<ulong, MapItem> allItems);
    }
}