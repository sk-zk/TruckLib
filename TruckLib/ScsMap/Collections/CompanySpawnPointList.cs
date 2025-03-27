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
    /// Reperesents a list of <see cref="Company"/> spawn points.
    /// </summary>
    public class CompanySpawnPointList : IList<CompanySpawnPoint>
    {
        /// <summary>
        /// The <see cref="Company"/> item which parents these models.
        /// </summary>
        public Company Parent { get; init; }

        private readonly List<CompanySpawnPoint> list = [];

        /// <summary>
        /// Instantiates an empty list.
        /// </summary>
        /// <param name="parent">The <see cref="Company"/> item which parents these models.</param>
        public CompanySpawnPointList(Company parent)
        {
            Parent = parent;
        }

        /// <inheritdoc/>
        public CompanySpawnPoint this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        /// <inheritdoc/>
        public int Count => list.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(CompanySpawnPoint item)
        {
            list.Add(item);
        }

        /// <summary>
        /// Creates a map node at the specified position and adds a <see cref="CompanySpawnPointType"/>
        /// object with the given properties to the end of the list.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <param name="rotation">The rotation of the node.</param>
        /// <param name="type">The spawn point type.</param>
        /// <returns>The newly created spawn point.</returns>
        public CompanySpawnPoint Add(Vector3 position, Quaternion rotation, CompanySpawnPointType type)
        {
            var point = new CompanySpawnPoint(CreateNode(position), type);
            point.Node.Rotation = rotation;
            Add(point);
            return point;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (var item in list)
                GetRidOfTheNode(item);
            list.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(CompanySpawnPoint item)
        {
            return list.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(CompanySpawnPoint[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<CompanySpawnPoint> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <inheritdoc/>
        public int IndexOf(CompanySpawnPoint item)
        {
            return list.IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, CompanySpawnPoint item)
        {
            list.Insert(index, item);
        }

        /// <summary>
        /// Creates a map node at the specified position and inserts a <see cref="CompanySpawnPoint"/>
        /// object with the given properties at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the object should be inserted.</param>
        /// <param name="position">The position of the node.</param>
        /// <param name="rotation">The rotation of the node.</param>
        /// <param name="type">The spawn point type.</param>
        /// <returns>The newly created spawn point.</returns>
        public CompanySpawnPoint Insert(int index, Vector3 position, Quaternion rotation, 
            CompanySpawnPointType type)
        {
            var point = new CompanySpawnPoint(CreateNode(position), type);
            point.Node.Rotation = rotation;
            Insert(index, point);
            return point;
        }

        /// <summary>
        /// Removes the first occurrence of the specified object from the list
        /// and deletes its map node if it is not connected to anything else.
        /// </summary>
        /// <inheritdoc/>
        public bool Remove(CompanySpawnPoint item)
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

        private static void GetRidOfTheNode(CompanySpawnPoint item)
        {
            item.Node.ForwardItem = null;
            if (item.Node.IsOrphaned())
                item.Node.Parent.Delete(item.Node);
        }
    }
}
