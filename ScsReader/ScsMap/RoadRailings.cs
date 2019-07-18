using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Railings of a road segment.
    /// </summary>
    public class RoadRailings : Railings
    {
        public bool DoubleSided;

        public bool CenterPartOnly;

        public RoadRailings Clone()
        {
            var rr = (RoadRailings)MemberwiseClone();
            rr.Models = (RoadRailing[])Models.Clone();
            return rr;
        }
    }
}
