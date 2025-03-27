using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap.Collections
{
    public class GateActivationPointListTest
    {
        [Fact]
        public void Add()
        {
            var map = new Map("foo");
            var gate = Gate.Add(map, new Vector3(10, 0, 10), "bar", GateType.TriggerActivated);

            gate.ActivationPoints.Add(new Vector3(-50, 0, -50), "aaa");

            Assert.Single(gate.ActivationPoints);

            Assert.Equal("aaa", gate.ActivationPoints[0].Trigger);

            Assert.Equal(new Vector3(-50, 0, -50), gate.ActivationPoints[0].Node.Position);
            Assert.True(map.Nodes.ContainsKey(gate.ActivationPoints[0].Node.Uid));
            Assert.False(gate.ActivationPoints[0].Node.IsRed);
            Assert.Equal(gate, gate.ActivationPoints[0].Node.ForwardItem);
        }

        [Fact]
        public void Insert()
        {
            var map = new Map("foo");
            var gate = Gate.Add(map, new Vector3(10, 0, 10), "bar", GateType.TriggerActivated);

            gate.ActivationPoints.Add(new Vector3(-50, 0, -50), "aaa");
            gate.ActivationPoints.Insert(0, new Vector3(-30, 0, -20), "bbb");

            Assert.Equal(2, gate.ActivationPoints.Count);
            Assert.Equal(new Vector3(-30, 0, -20), gate.ActivationPoints[0].Node.Position);
        }

        [Fact]
        public void RemoveAt()
        {
            var map = new Map("foo");
            var gate = Gate.Add(map, new Vector3(10, 0, 10), "bar", GateType.TriggerActivated);

            var point = gate.ActivationPoints.Add(new Vector3(-50, 0, -50), "aaa");
            gate.ActivationPoints.RemoveAt(0);

            Assert.Empty(gate.ActivationPoints);
            Assert.False(map.Nodes.ContainsKey(point.Node.Uid));
        }

        [Fact]
        public void Remove()
        {
            var map = new Map("foo");
            var gate = Gate.Add(map, new Vector3(10, 0, 10), "bar", GateType.TriggerActivated);

            var point = gate.ActivationPoints.Add(new Vector3(-50, 0, -50), "aaa");
            var success = gate.ActivationPoints.Remove(point);

            Assert.True(success);
            Assert.Empty(gate.ActivationPoints);
            Assert.False(map.Nodes.ContainsKey(point.Node.Uid));
        }

        [Fact]
        public void Clear()
        {
            var map = new Map("foo");
            var gate = Gate.Add(map, new Vector3(10, 0, 10), "bar", GateType.TriggerActivated);

            var point1 = gate.ActivationPoints.Add(new Vector3(-50, 0, -50), "aaa");
            var point2 = gate.ActivationPoints.Insert(0, new Vector3(-30, 0, -20), "bbb");
            gate.ActivationPoints.Clear();

            Assert.Empty(gate.ActivationPoints);
            Assert.False(map.Nodes.ContainsKey(point1.Node.Uid));
            Assert.False(map.Nodes.ContainsKey(point2.Node.Uid));
        }

        [Fact]
        public void AddThrowsIfFull()
        {
            var map = new Map("foo");
            var gate = Gate.Add(map, new Vector3(50, 0, 50), "bar", GateType.TriggerActivated);

            gate.ActivationPoints.Add(new Vector3(69, 42, 0), "aaa");
            gate.ActivationPoints.Add(new Vector3(69, 42, 0), "aaa");
            Assert.Throws<IndexOutOfRangeException>(() => gate.ActivationPoints.Add(new Vector3(69, 42, 0), "aaa"));
        }
    }
}
