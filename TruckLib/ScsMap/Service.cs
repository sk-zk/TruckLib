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
    /// Additional data for service prefabs (gas stations, weigh stations etc).
    /// </summary>
    public class Service : PrefabSlaveItem
    {
        public override ItemType ItemType => ItemType.Service;

        public List<INode> Nodes { get; set; }

        public ServiceType ServiceType
        {
            get => (ServiceType)Kdop.Flags.GetByte(0);
            set => Kdop.Flags.SetByte(0, (byte)value);
        }

        public Service() : base() { }

        internal Service(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Nodes = new List<INode>();
        }

        public static Service Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<Service>(map, parent, position);
        }

        public override void Move(Vector3 newPos)
        {
            throw new NotImplementedException();
        }

        public override void Translate(Vector3 translation)
        {
            base.Translate(translation);

            foreach (var node in Nodes)
            {
                node.Move(node.Position + translation);
            }
        }

        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            base.UpdateNodeReferences(allNodes);
            ResolveNodeReferences(Nodes, allNodes);
        }

    }
}
