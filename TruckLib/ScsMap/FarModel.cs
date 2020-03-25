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
        /// Determines if the item is reflected in water.
        /// </summary>
        public bool WaterReflection
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        public FarModel()
        {
            const int maxFarModels = 4;
            Models = new FarModelData[maxFarModels].Select(h => new FarModelData()).ToArray();
        }

        public static FarModel Add(IItemContainer map, Token model, float width, float height, Vector3 position, Vector3 perspPosition)
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
            if (Node is UnresolvedNode && allNodes.ContainsKey(Node.Uid))
            {
                Node = allNodes[Node.Uid];
            }

            for(int i = 0; i < Models.Length; i++)
            {
                if (Models[i].PerspectiveNode is UnresolvedNode 
                    && allNodes.ContainsKey(Models[i].PerspectiveNode.Uid))
                {
                    Models[i].PerspectiveNode = allNodes[Models[i].PerspectiveNode.Uid];
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
