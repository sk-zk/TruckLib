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
    /// Reperesents a list of <see cref="FarModel.Models">Far Model models</see>.
    /// </summary>
    public class FarModelDataList : IList<FarModelData>
    {
        /// <summary>
        /// The <see cref="FarModel">Far Model</see> item which parents these models.
        /// </summary>
        public FarModel Parent { get; init; }

        private readonly List<FarModelData> list = [];

        /// <summary>
        /// Instantiates an empty list.
        /// </summary>
        /// <param name="parent">The <see cref="FarModel">Far Model</see> item which parents these models.</param>
        public FarModelDataList(FarModel parent)
        {
            Parent = parent;
        }

        /// <inheritdoc/>
        public FarModelData this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        /// <inheritdoc/>
        public int Count => list.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(FarModelData item)
        {
            list.Add(item);
        }

        /// <summary>
        /// Creates a map node at the specified position and adds a <see cref="FarModelData"/> object
        /// with the given properties to the end of the list.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <param name="model">The unit name of the model.</param>
        /// <param name="scale">The scale of the model.</param>
        /// <returns>The newly created object.</returns>
        public FarModelData Add(Vector3 position, Token model, Vector3 scale)
        {
            var obj = new FarModelData(CreateNode(position), model, scale);
            Add(obj);
            return obj;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (var item in list)
                GetRidOfTheNode(item);
            list.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(FarModelData item)
        {
            return list.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(FarModelData[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<FarModelData> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <inheritdoc/>
        public int IndexOf(FarModelData item)
        {
            return list.IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, FarModelData item)
        {
            list.Insert(index, item);
        }

        /// <summary>
        /// Creates a map node at the specified position and inserts a <see cref="FarModelData"/> object
        /// with the given properties at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the object should be inserted.</param>
        /// <param name="position">The position of the node.</param>
        /// <param name="model">The unit name of the model.</param>
        /// <param name="scale">The scale of the model.</param>
        /// <returns>The newly created object.</returns>
        public FarModelData Insert(int index, Vector3 position, Token model, Vector3 scale)
        {
            var obj = new FarModelData(CreateNode(position), model, scale);
            Insert(index, obj);
            return obj;
        }

        /// <summary>
        /// Removes the first occurrence of the specified object from the list
        /// and deletes its map node if it is not connected to anything else.
        /// </summary>
        /// <inheritdoc/>
        public bool Remove(FarModelData item)
        {
            var success = list.Remove(item);
            if (success)
                GetRidOfTheNode(item);
            return success;
        }

        /// <summary>
        /// Removes the element at the specified index from the list
        /// and deletes its map node if it is not connected to anything else.
        /// </summary>
        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            GetRidOfTheNode(list[index]);
            list.RemoveAt(index);
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

        private static void GetRidOfTheNode(FarModelData item)
        {
            item.Node.ForwardItem = null;
            if (item.Node.IsOrphaned())
                item.Node.Parent.Delete(item.Node);
        }
    }
}
