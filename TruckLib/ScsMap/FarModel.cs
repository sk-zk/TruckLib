using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public class FarModel : MapItem
    {
        public override ItemType ItemType => ItemType.FarModel;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Width of the area in which the model is visible.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Height of the area in which the model is visible.
        /// </summary>
        public float Height { get; set; }

        public FarModelData[] Models { get; set; }

        public Node Node { get; set; }

        /// <summary>
        /// Determines if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public FarModel() : base()
        {
            Models = new FarModelData[4].Select(
                h => new FarModelData()).ToArray();
        }

        internal FarModel(bool initFields) : base(initFields)
        {
            if (initFields) Init();
            Models = new FarModelData[4].Select(
                h => new FarModelData()).ToArray();
        }

        protected override void Init()
        {
            base.Init();
        }

        public static FarModel Add(IItemContainer map, Token model, float width, float height,
            Vector3 position, Vector3 perspPosition)
        {
            var farModel = new FarModel();

            var node = map.AddNode(position, true);
            farModel.Node = node;
            node.ForwardItem = farModel;

            var perspNode = map.AddNode(perspPosition, true);
            perspNode.ForwardItem = farModel;
            farModel.Models[0].PerspectiveNode = perspNode;

            farModel.Models[0].Model = model;
            farModel.Width = width;
            farModel.Height = height;

            map.AddItem(farModel, node);
            return farModel;
        }

        internal override IEnumerable<Node> GetItemNodes()
        {
            return Models.Select(x => x.PerspectiveNode).Prepend(Node);
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            if (Node is UnresolvedNode && 
                allNodes.TryGetValue(Node.Uid, out var resolvedNode))
            {
                Node = resolvedNode;
            }

            for(int i = 0; i < Models.Length; i++)
            {
                if (Models[i].PerspectiveNode is UnresolvedNode 
                    && allNodes.TryGetValue(Models[i].PerspectiveNode.Uid, 
                    out var resolvedPerspNode))
                {
                    Models[i].PerspectiveNode = resolvedPerspNode;
                }
            }
        } 
    }

    public struct FarModelData
    {
        /// <summary>
        /// The model.
        /// </summary>
        public Token Model;

        /// <summary>
        /// The scale.
        /// </summary>
        public Vector3 Scale;

        /// <summary>
        /// This node defines where the model will be rendered (I think).
        /// </summary>
        public Node PerspectiveNode;
    }
}
