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
    /// Defines a plane behind which nothing will be rendered.
    /// </summary>
    public class CutPlane : MapItem
    {
        public override ItemType ItemType => ItemType.CutPlane;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public List<INode> Nodes { get; set; }

        /// <summary>
        /// Determines if the cut plane is active in one direction only.
        /// </summary>
        public bool OneSideOnly
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public bool RotatedLimits
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        public CutPlane() : base() { }

        internal CutPlane(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Nodes = new List<INode>(2);
        }

        /// <summary>
        /// Adds a cut plane to the map.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="nodePositions"></param>
        /// <param name="oneSideOnly"></param>
        /// <returns></returns>
        public static CutPlane Add(IItemContainer map, IList<Vector3> nodePositions,
            bool oneSideOnly = false)
        {
            var cutPlane = new CutPlane();
            cutPlane.OneSideOnly = oneSideOnly;

            // the nodes have a forward node except for the last one
            // which just links to itself, so I'm creating the nodes in reverse
            // to create the references
            var posReverse = nodePositions.Reverse().ToArray(); 
            var nodes = new List<INode>(nodePositions.Count());
            for(int i = 0; i < posReverse.Count(); i++)
            {
                var node = map.AddNode(posReverse[i]);
                node.ForwardItem = cutPlane;
                if(i == 0)
                {
                    node.ForwardItem = node;
                    node.IsRed = true;
                }
                else
                {
                    node.ForwardItem = nodes[i - 1];
                }
                nodes.Add(node);
            }

            nodes.Reverse();
            cutPlane.Nodes = nodes.ToList();

            // the item is added to the sector the first node is in.
            // if you store it somewhere else, it will still work, but 
            // it can't be deleted until you update it
            map.AddItem(cutPlane, nodes.Last());
            return cutPlane;
        }

        /// <summary>
        /// Appends a new node to the cut plane.
        /// </summary>
        /// <param name="position"></param>
        public void Append(Vector3 position)
        {
            var node = Nodes[0].Sectors[0].Map.AddNode(position);
            node.ForwardItem = this;
            node.ForwardItem = node;
            Nodes.Last().ForwardItem = node;
            Nodes.Add(node);
        }

        /// <summary>
        /// Moves the item to a different location, where Nodes[0] 
        /// will be set to the given position.
        /// </summary>
        /// <param name="newPos"></param>
        public override void Move(Vector3 newPos)
        {
            var translation = newPos - Nodes[0].Position;
            Translate(translation);
        }


        public override void Translate(Vector3 translation)
        {
            foreach (var node in Nodes)
            {
                node.Move(node.Position + translation);
            }
        }

        internal override IEnumerable<INode> GetItemNodes()
        {
            return new List<INode>(Nodes);
        }

        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            ResolveNodeReferences(Nodes, allNodes);
        }
    }
}
