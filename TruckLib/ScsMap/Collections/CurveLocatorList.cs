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
    /// Reperesents a list of <see cref="Curve.Locators">curve locator points</see>.
    /// Has a maximum size of 2.
    /// </summary>
    public class CurveLocatorList : IList<INode>
    {
        /// <summary>
        /// The <see cref="Curve">Curve</see> item which parents these nodes.
        /// </summary>
        public Curve Parent { get; init; }

        private readonly List<INode> list = [];

        /// <summary>
        /// The maximum size of the list.
        /// </summary>
        public const int MaxSize = 2;

        /// <summary>
        /// Instantiates an empty list.
        /// </summary>
        /// <param name="parent">The <see cref="Gate">Curve</see> item which parents these models.</param>
        public CurveLocatorList(Curve parent)
        {
            Parent = parent;
        }

        /// <inheritdoc/>
        public INode this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        /// <inheritdoc/>
        public int Count => list.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <exception cref="IndexOutOfRangeException">Thrown if the list is full.</exception>
        /// <inheritdoc/>
        public void Add(INode item)
        {
            if (list.Count >= MaxSize)
                throw new IndexOutOfRangeException();
            list.Add(item);
        }

        /// <summary>
        /// Creates a locator node at the specified position and adds it to the end of the list.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <param name="rotation">The rotation of the node.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if the list is full.</exception>
        public void Add(Vector3 position, Quaternion rotation)
        {
            if (list.Count >= MaxSize)
                throw new IndexOutOfRangeException();
            Add(CreateNode(position, rotation));
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

        /// <exception cref="IndexOutOfRangeException">Thrown if the list is full.</exception>
        /// <inheritdoc/>
        public void Insert(int index, INode item)
        {
            if (list.Count >= MaxSize)
                throw new IndexOutOfRangeException();
            list.Insert(index, item);
        }

        /// <summary>
        /// Creates a locator node at the specified position and inserts it
        /// with the given properties at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the object should be inserted.</param>
        /// <param name="position">The position of the node.</param>
        /// <param name="rotation">The rotation of the node.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if the list is full.</exception>
        public void Insert(int index, Vector3 position, Quaternion rotation)
        {
            if (list.Count >= MaxSize)
                throw new IndexOutOfRangeException();
            Insert(index, CreateNode(position, rotation));
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
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        private Node CreateNode(Vector3 position, Quaternion rotation)
        {
            var node = Parent.Node.Parent.AddNode(position, false);
            node.Rotation = rotation;
            node.IsCurveLocator = true;
            node.BackwardItem = Parent;
            return node;
        }

        private void GetRidOfTheNode(INode node)
        {
            node.BackwardItem = null;
            if (node.IsOrphaned())
                node.Parent.Delete(node);
        }
    }
}
