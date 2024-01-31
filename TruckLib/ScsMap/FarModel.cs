using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Defines a rectangular area which ignores the usual view distance limit of 1500 m
    /// for specific items if the camera is inside it.
    /// </summary>
    public class FarModel : MapItem, IItemReferences
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.FarModel;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Width of the area in which the models are visible.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Height of the area in which the models are visible.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Models specific to Far Model items which are only visible inside the area defined
        /// by this item.
        /// Used if <see cref="UseMapItems">UseMapItems</see> is false.
        /// </summary>
        public FarModelDataList Models { get; set; }

        /// <summary>
        /// Regular map items for which the view distance limit is ignored inside the area defined
        /// by this item.
        /// Used if <see cref="UseMapItems">UseMapItems</see> is true.
        /// </summary>
        public List<IMapItem> Children { get; set; }

        /// <summary>
        /// The map node of this item.
        /// </summary>
        public INode Node { get; set; }

        /// <summary>
        /// Gets or sets if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        /// <summary>
        /// If true, the item affects regular map items referenced in
        /// <see cref="Children">Children</see>.
        /// If false, the game will place models unique to Far Model items specified in
        /// <see cref="Models">Models</see>.
        /// </summary>
        public bool UseMapItems
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        public FarModel() : base()
        {
        }

        internal FarModel(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Models = new FarModelDataList(this);
            Children = new List<IMapItem>();
        }

        /// <summary>
        /// Adds a Far Model item to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the center node.</param>
        /// <param name="width">The width of the area.</param>
        /// <param name="height">The height of the area.</param>
        /// <returns>The newly created Far Model item.</returns>
        public static FarModel Add(IItemContainer map, Vector3 position, float width, float height)
        {
            var farModel = new FarModel();

            var node = map.AddNode(position, true);
            farModel.Node = node;
            node.ForwardItem = farModel;
            farModel.Width = width;
            farModel.Height = height;

            map.AddItem(farModel);
            return farModel;
        }

        /// <inheritdoc/>
        public override void Move(Vector3 newPos)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Translate(Vector3 translation)
        {
            Node.Move(Node.Position + translation);
            foreach (var model in Models)
            {
                model.Node?.Move(model.Node.Position + translation);
            }
        }

        /// <inheritdoc/>
        internal override IEnumerable<INode> GetItemNodes() =>
            Models.Select(x => x.Node).Prepend(Node);

        /// <inheritdoc/>
        internal override Vector3 GetCenter()
        {
            if (UseMapItems)
                return Node.Position;

            var acc = Node.Position;
            foreach (var model in Models)
            {
                acc += model.Node.Position;
            }
            return acc / (Models.Count + 1);
        }

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            Node = ResolveNodeReference(Node, allNodes);

            for (int i = 0; i < Models.Count; i++)
            {
                if (Models[i].Node is UnresolvedNode &&
                    allNodes.TryGetValue(Models[i].Node.Uid, out var resolvedModelNode))
                {
                    var farModel = Models[i];
                    farModel.Node = resolvedModelNode;
                    Models[i] = farModel;
                }
            }
        }

        /// <inheritdoc/>
        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i] is UnresolvedItem
                    && allItems.TryGetValue(Children[i].Uid, out var resolved))
                {
                    Children[i] = resolved;
                }
            }
        }
    }
}
