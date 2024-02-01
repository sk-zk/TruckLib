using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class MapTest
    {
        [Fact]
        public void ThrowIfNameInvalid()
        {
            var map = new Map("foo");
            Assert.Throws<ArgumentNullException>(() => map.Name = null);
            Assert.Throws<ArgumentNullException>(() => map.Name = " ");
            Assert.Throws<ArgumentException>(() => map.Name = "a/b");
        }
    }
}
