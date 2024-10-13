using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;
using TruckLib;
using System.Diagnostics;

namespace TruckLibTests.TruckLib.ScsMap
{
    public class GateTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var gate = Gate.Add(map, new Vector3(10, 0, 10), "bar", GateType.TriggerActivated);

            Assert.True(map.MapItems.ContainsKey(gate.Uid));

            Assert.Equal("bar", gate.Model);
            Assert.Equal(GateType.TriggerActivated, gate.Type);

            Assert.Equal(new Vector3(10, 0, 10), gate.Node.Position);
            Assert.True(gate.Node.IsRed);
            Assert.Equal(gate, gate.Node.ForwardItem);
            Assert.Null(gate.Node.BackwardItem);
        }

        [Fact]
        public void Delete()
        {
            var map = new Map("foo");
            var gate = Gate.Add(map, new Vector3(10, 0, 10), "bar", GateType.TriggerActivated);
            gate.ActivationPoints.Add(new Vector3(-50, 0, -50), "aaa");
            var point = gate.ActivationPoints[0];

            map.Delete(gate);

            Assert.Empty(map.MapItems);
            Assert.Empty(map.Nodes);
        }

    }
}
