using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// The activation point for a ferry.
    /// </summary>
    public class Ferry : SingleNodeItem, IItemReferences
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Ferry;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Base;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Unit name of the port.
        /// </summary>
        public Token Port { get; set; }

        public Vector3 UnloadOffset { get; set; }

        /// <summary>
        /// The prefab this ferry is linked to, if applicable.
        /// </summary>
        public IMapItem Prefab { get; set; }

        /// <summary>
        /// Gets or sets if the item is actually a train transport.
        /// </summary>
        public bool TrainTransport
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public bool Cutscene
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        public Ferry() : base() { }

        internal Ferry(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Adds a ferry to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the node.</param>
        /// <param name="port">Unit name of the port.</param>
        /// <returns>The newly created ferry.</returns>
        public static Ferry Add(IItemContainer map, Vector3 position, Token port)
        {
            var ferry = Add<Ferry>(map, position);
            ferry.Port = port;
            return ferry;
        }

        /// <inheritdoc/>
        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems)
        {
            if (Prefab is UnresolvedItem 
                && allItems.TryGetValue(Prefab.Uid, out var resolvedPrefab))
            {
                Prefab = resolvedPrefab;
            }
        }
    }
}
