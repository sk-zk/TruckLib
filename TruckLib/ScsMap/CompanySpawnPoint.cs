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
        public CompanySpawnPointType Type { get; set; }

        public CompanySpawnPoint(INode node, CompanySpawnPointType type)
        {
            Node = node;
            Type = type;
        }
    }
}
