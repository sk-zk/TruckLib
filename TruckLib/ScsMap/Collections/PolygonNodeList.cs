using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap.Collections
{
    /// <summary>
    /// Contains the nodes of a <see cref="PolygonItem">polygon item</see>.
    /// </summary>
    public class PolygonNodeList : IList<INode>
    {
        /// <summary>
        /// The <see cref="PolygonItem">polygon item</see> which parents these nodes.
        /// </summary>
        public PolygonItem Parent { get; init; }

        private IItemContainer container;

        private readonly List<INode> list = [];

        /// <summary>
        /// Instantiates an empty list.
        /// </summary>
        /// <param name="parent">The <see cref="PolygonItem">polygon item</see>
        /// which parents these models.</param>
        public PolygonNodeList(PolygonItem parent)
        {
            Parent = parent;
        }

        /// <inheritdoc/>
        public INode this[int index]
        {
            get => list[index];
            set
            {
                list[index] = value;
                if (index == 0)
                {
                    if (list[0] is not UnresolvedNode)
                    {
                        container = list[0].Parent;
                        list[0].IsRed = true;
                    }
                    if (Count > 1 && list[1] is not UnresolvedNode)
                    {
                        list[1].IsRed = false;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public int Count => list.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(INode item)
        {
            list.Add(item);
            if (Count == 1 && item is not UnresolvedNode)
            {
                container = list[0].Parent;
                list[0].IsRed = true;
            }
        }

        /// <summary>
        /// Adds nodes to the end of list.
        /// </summary>
        /// <param name="items">The nodes to add to the list.</param>
        public void AddRange(IEnumerable<INode> items)
        {
            list.AddRange(items);
            if (Count == items.Count() && list[0] is not UnresolvedNode)
                list[0].IsRed = true;
        }

        /// <summary>
        /// Creates a map node at the specified position and adds it to the end of the list.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <returns>The newly created node.</returns>
        public INode Add(Vector3 position)
        {
            var node = CreateNode(position);
            Add(node);
            return node;
        }

        /// <summary>
        /// Creates map nodes at the specified positions and adds them to the end of the list.
        /// </summary>
        /// <param name="positions">The positions of the nodes.</param>
        public void AddRange(IEnumerable<Vector3> positions)
        {
            list.EnsureCapacity(Count + positions.Count());
            foreach (var position in positions)
                Add(position);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (var item in list)
                GetRidOfTheNode(item);
            list.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(INode item)
        {
            return list.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(INode[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<INode> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <inheritdoc/>
        public int IndexOf(INode item)
        {
            return list.IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, INode item)
        {
            list.Insert(index, item);
            if (index == 0)
                list[1].IsRed = false;
        }

        /// <summary>
        /// Creates a map node at the specified position and inserts it
        /// with the given properties at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the object should be inserted.</param>
        /// <param name="position">The position of the node.</param>
        /// <returns>The newly created node.</returns>
        public INode Insert(int index, Vector3 position)
        {
            var node = CreateNode(position, index == 0);
            Insert(index, node);
            return node;
        }

        /// <summary>
        /// Removes the first occurrence of the specified node from the list
        /// and deletes it from the map if it is not connected to anything else.
        /// </summary>
        /// <inheritdoc/>
        public bool Remove(INode item)
        {
            var index = IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes the node at the specified index from the list
        /// and deletes it if it is not connected to anything else.
        /// </summary>
        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            var point = list[index];
            list.RemoveAt(index);
            GetRidOfTheNode(point);
            if (index == 0)
                list[0].IsRed = true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        private Node CreateNode(Vector3 position, bool isIndex0 = false)
        {
            container ??= Parent.Parent;
            var node = container.AddNode(position, isIndex0);
            node.ForwardItem = Parent;
            return node;
        }

        private void GetRidOfTheNode(INode node)
        {
            node.ForwardItem = null;
            if (node.IsOrphaned())
                node.Parent.Delete(node);
        }
    }
}
