using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Reperesents a list of <see cref="Gate.ActivationPoints">gate activation points.</see>.
    /// Has a maximum size of 2.
    /// </summary>
    public class GateActivationPointList : IList<GateActivationPoint>
    {
        /// <summary>
        /// The <see cref="Gate">Gate</see> item which parents these models.
        /// </summary>
        public Gate Parent { get; init; }

        private readonly List<GateActivationPoint> list = new();

        /// <summary>
        /// The maximum size of the list.
        /// </summary>
        public const int MaxSize = 2;

        /// <summary>
        /// Instantiates an empty list.
        /// </summary>
        /// <param name="parent">The <see cref="Gate">Gate</see> item which parents these models.</param>
        public GateActivationPointList(Gate parent)
        {
            Parent = parent;
        }

        /// <inheritdoc/>
        public GateActivationPoint this[int index] 
        { 
            get => list[index]; 
            set => list[index] = value;
        }

        /// <inheritdoc/>
        public int Count => list.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(GateActivationPoint item)
        {
            if (list.Count >= MaxSize)
                throw new IndexOutOfRangeException();
            list.Add(item);
        }

        internal void Add(GateActivationPoint item, bool updateSectorMapItems)
        {
            if (updateSectorMapItems)
            {
                Add(item);
            } 
            else
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Creates a map node at the specified position and adds a <see cref="GateActivationPoint"/> object
        /// with the given properties to the end of the list.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <param name="trigger">The name of the trigger.</param>
        public void Add(Vector3 position, string trigger)
        {
            if (list.Count >= MaxSize)
                throw new IndexOutOfRangeException();
            Add(new GateActivationPoint(CreateNode(position), trigger));
        }

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (var item in list)
                GetRidOfTheNode(item);
            list.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(GateActivationPoint item)
        {
            return list.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(GateActivationPoint[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<GateActivationPoint> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <inheritdoc/>
        public int IndexOf(GateActivationPoint item)
        {
            return list.IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, GateActivationPoint item)
        {
            if (list.Count >= MaxSize)
                throw new IndexOutOfRangeException();
            list.Insert(index, item);
        }

        /// <summary>
        /// Creates a map node at the specified position and inserts a <see cref="GateActivationPoint"/> object
        /// with the given properties at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the object should be inserted.</param>
        /// <param name="position">The position of the node.</param>
        /// <param name="trigger">The name of the trigger.</param>
        public void Insert(int index, Vector3 position, string trigger)
        {
            if (list.Count >= MaxSize)
                throw new IndexOutOfRangeException();
            Insert(index, new GateActivationPoint(CreateNode(position), trigger));
        }

        /// <summary>
        /// Removes the first occurrence of the specified object from the list
        /// and deletes its map node if it is not connected to anything else.
        /// </summary>
        /// <inheritdoc/>
        public bool Remove(GateActivationPoint item)
        {
            var index = IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes the element at the specified index from the list
        /// and deletes its map node if it is not connected to anything else.
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

        private Node CreateNode(Vector3 position)
        {
            var node = Parent.Node.Parent.AddNode(position, false);
            node.ForwardItem = Parent;
            return node;
        }

        private void GetRidOfTheNode(GateActivationPoint point)
        {
            point.Node.ForwardItem = null;
            if (point.Node.IsOrphaned())
                point.Node.Parent.Delete(point.Node);
        }
    }
}
