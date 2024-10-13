using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.Models.Ppd;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class PrefabFixture
    {
        public PrefabDescriptor CrossingPpd { get; } =
            PrefabDescriptor.Open("Data/PrefabTest/blkw_r1_x_r1_narrow_tmpl.ppd");

        public PrefabDescriptor CompanyPpd { get; } =
            PrefabDescriptor.Open("Data/PrefabTest/car_dealer_01_fr.ppd");
    }
}
