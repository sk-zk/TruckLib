using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public struct CompanySpawnPoint
    {
        public INode Node { get; set; }

        public FlagField Flags { get; set; }

        public CompanySpawnPointType Type {
            get => (CompanySpawnPointType)Flags.GetBitString(0, 4);
            set => Flags.SetBitString(0, 4, (byte)value);
        }

        public CompanySpawnPoint(INode node, uint flags)
        {
            Node = node;
            Flags = new FlagField(flags);
        }
    }
}
