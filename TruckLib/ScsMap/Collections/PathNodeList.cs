using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap.Collections
{
    /// <summary>
    /// Contains the nodes of a <see cref="PathItem">path item</see>.
    /// </summary>
    public class PathNodeList : IList<INode>
    {
        /// <summary>
        /// The <see cref="PathItem">path item</see> which parents these nodes.
        /// </summary>
        public PathItem Parent { get; init; }

        private IItemContainer container;

        private readonly List<INode> nodes = [];

        /// <summary>
        /// Cached lengths of the path segments, if the parent is a <see cref="Mover"/> or <see cref="Walker"/>.
        /// </summary>
        public List<float> Lengths { get; internal set; }

        /// <summary>
        /// Instantiates an empty list.
        /// </summary>
        /// <param name="parent">The <see cref="PathItem">path item</see>
        /// which parents these models.</param>
        public PathNodeList(PathItem parent)
        {
            Parent = parent;
            if (parent is IPathItemWithCachedLengths)
                Lengths = [];
        }

        /// <inheritdoc/>
        public INode this[int index]
        {
            get => nodes[index];
            set
            {
                nodes[index] = value;

                if (index == 0)
                {
                    container = nodes[0].Parent;
                    if (nodes[0] is not UnresolvedNode)
                        nodes[0].IsRed = true;
                    if (Count > 1 && nodes[1] is not UnresolvedNode)
                        nodes[1].IsRed = false;
                }
                Parent.RecalculateAdjacent(index);

                if (Parent is IPathItemWithCachedLengths cl)
                    RecalculateAdjacentLengths(index, cl.UseCurvedPath);
            }
        }

        internal void SetWithoutRecalculating(int index, INode value)
        {
            nodes[index] = value;
        }

        private void RecalculateAdjacentLengths(int i, bool useCurvedPath)
        {
            if (i > 0)
                Lengths[i - 1] = CalculatePathLength(i - 1, useCurvedPath);
            if (i < Lengths.Count)
                Lengths[i] = CalculatePathLength(i - 1, useCurvedPath);
        }

        /// <inheritdoc/>
        public int Count => nodes.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(INode item)
        {
            nodes.Add(item);
            if (Count == 1 && item is not UnresolvedNode)
            {
                container = nodes[0].Parent;
                nodes[0].IsRed = true;
            }
            Parent.RecalculateAdjacent(Count - 1);

            if (Count > 1 && Parent is IPathItemWithCachedLengths cl)
                Lengths.Add(CalculatePathLength(Count - 2, cl.UseCurvedPath));
        }

        /// <summary>
        /// Adds nodes to the end of list.
        /// </summary>
        /// <param name="items">The nodes to add to the list.</param>
        public void AddRange(IEnumerable<INode> items)
        {
            AddRange(items, true);
        }

        internal void AddRange(IEnumerable<INode> items, bool recalculate)
        {
            nodes.AddRange(items);
            if (recalculate)
            {
                if (Count == items.Count() && nodes[0] is not UnresolvedNode)
                    nodes[0].IsRed = true;
                Parent.Recalculate();

                if (Parent is IPathItemWithCachedLengths cl)
                    CalculatePathLengths(cl.UseCurvedPath);
            }
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
            nodes.EnsureCapacity(Count + positions.Count());
            foreach (var position in positions)
                Add(position);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (var item in nodes)
                GetRidOfTheNode(item);
            nodes.Clear();
            Lengths?.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(INode item)
        {
            return nodes.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(INode[] array, int arrayIndex)
        {
            nodes.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<INode> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        /// <inheritdoc/>
        public int IndexOf(INode item)
        {
            return nodes.IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, INode item)
        {
            nodes.Insert(index, item);
            if (index == 0)
                nodes[1].IsRed = false;
            Parent.RecalculateAdjacent(index);

            if (Parent is IPathItemWithCachedLengths cl)
            {
                Lengths.Insert(index, 0f);
                RecalculateAdjacentLengths(index, cl.UseCurvedPath);
            }
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
            var point = nodes[index];
            nodes.RemoveAt(index);
            GetRidOfTheNode(point);
            if (index == 0)
                nodes[0].IsRed = true;

            Parent.RecalculateAdjacent(index - 1);
            if (index < Count)
                Parent.RecalculateAdjacent(index);

            if (Parent is IPathItemWithCachedLengths cl)
            {
                var indexToUpdate = index == Lengths.Count ? index - 1 : index;
                Lengths.RemoveAt(indexToUpdate);
                RecalculateAdjacentLengths(indexToUpdate, cl.UseCurvedPath);             
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return nodes.GetEnumerator();
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

        /// <summary>
        /// Calculates the lengths of the sections of a path defined by a <see cref="Mover"/> or <see cref="Walker"/>.
        /// </summary>
        /// <param name="nodes">The nodes of the item.</param>
        /// <param name="useCurvedPath">Whether the path is a Catmull-Rom spline rather than linear.</param>
        /// <returns>The calculated lengths.</returns>
        private void CalculatePathLengths(bool useCurvedPath)
        {
            var lengths = new List<float>(nodes.Count - 1);
            if (nodes.Count > 2)
            {
                for (int i = 0; i < nodes.Count - 1; i++)
                    lengths.Add(CalculatePathLength(i - 1, useCurvedPath));
            }
            Lengths = lengths;
        }

        private float CalculatePathLength(int i, bool useCurvedPath)
        {
            float length;
            if (useCurvedPath)
            {
                var p0 = nodes[Math.Max(0, i - 1)].Position;
                var p1 = nodes[i].Position;
                var p2 = nodes[Math.Min(nodes.Count - 1, i + 1)].Position;
                var p3 = nodes[Math.Min(nodes.Count - 1, i + 2)].Position;
                length = CatmullRomSpline.ApproximateLength(p0, p1, p2, p3);
            }
            else
            {
                length = (nodes[i + 1].Position - nodes[i].Position).Length();
            }
            return length;
        }
    }
}
