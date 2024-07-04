using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.HashFs.Dds
{
    internal class SubresourceData
    {
        public int DataIdx { get; set; }
        public int RowPitch { get; set; }
        public int SlicePitch { get; set; }
    }
}
