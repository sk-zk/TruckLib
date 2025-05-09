﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RBush;

namespace TruckLib.ScsMap.Collections
{
    /// <summary>
    /// A collection for <see cref="Node">map nodes</see> which combines a dictionary and R-tree.
    /// </summary>
    public class NodeDictionary : IDictionary<ulong, INode>
    {
        private readonly Dictionary<ulong, INode> dictionary = [];
        internal RBush<Node> Tree { get; } = new();

        public INode this[ulong key]
        {
            get => dictionary[key];
            set
            {
                var previous = (Node)dictionary[key];
                Tree.Delete(previous);
                Tree.Insert((Node)value);
                dictionary[key] = value;
            }
        }

        public ICollection<ulong> Keys => dictionary.Keys;

        public ICollection<INode> Values => dictionary.Values;

        public int Count => dictionary.Count;

        public bool IsReadOnly => false;

        public void Add(ulong key, INode value)
        {
            Tree.Insert((Node)value);
            dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<ulong, INode> item)
        {
            Tree.Insert((Node)item.Value);
            dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            dictionary.Clear();
            Tree.Clear();
        }

        public bool Contains(KeyValuePair<ulong, INode> item) => 
            dictionary.Contains(item);

        public bool ContainsKey(ulong key) => 
            dictionary.ContainsKey(key);

        public bool ContainsValue(INode value) => 
            dictionary.ContainsValue(value);

        public void CopyTo(KeyValuePair<ulong, INode>[] array, int arrayIndex) => 
            (dictionary as IDictionary).CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<ulong, INode>> GetEnumerator() => 
            dictionary.GetEnumerator();

        public bool Remove(ulong key)
        {
            if (dictionary.TryGetValue(key, out var node))
                Tree.Delete((Node)node);

            return dictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<ulong, INode> item)
        {
            if (dictionary.TryGetValue(item.Key, out var node))
                Tree.Delete((Node)node);

            return dictionary.Remove(item.Key);
        }

        public bool TryGetValue(ulong key, [MaybeNullWhen(false)] out INode value) => 
            dictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();

        /// <summary>
        /// Retrieves nodes within the given bounding box. 
        /// </summary>
        /// <param name="minX">The minimum X coordinate.</param>
        /// <param name="minZ">The minimum Z coordinate.</param>
        /// <param name="maxX">The maximum X coordinate.</param>
        /// <param name="maxZ">The maximum Z coordinate.></param>
        /// <returns>A list of nodes contained within this bounding box.</returns>
        public IReadOnlyList<Node> Within(double minX, double minZ, double maxX, double maxZ)
        {
            return Tree.Search(new RBush.Envelope(minX, minZ, maxX, maxZ));
        }
    }
}
