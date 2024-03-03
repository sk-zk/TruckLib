using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    internal interface IPathItemWithCachedLengths
    {
        List<float> Lengths { get; }
        bool UseCurvedPath { get; set; }
    }
}
