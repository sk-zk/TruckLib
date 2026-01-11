using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib.Tests.TruckLib.ScsMap
{
    public class PolylineTest
    {
        [Fact]
        public void InterpolateCurveDist()
        {
            var map = new Map("foo");
            var r1 = Road.Add(map, new(19, 0, 19.5f), new(65, 0, 23), "ger1");
            var r2 = r1.Append(new(98, 0, 43.5f));
            var r3 = r2.Append(new(146.5f, 0, 25));

            var actual = r2.InterpolateCurveDist(4.2f);

            Assert.NotNull(actual);
            AssertEx.Equal(new(68.90782f, 0, 24.699467f), actual.Value.Position, 0.0001f);
            AssertEx.Equal(new(0, -0.85898876f, 0, 0.5119944f), actual.Value.Rotation, 0.0001f);
        }

        [Fact]
        public void InterpolateCurveDistNullIfTooFar()
        {
            var map = new Map("foo");
            var r1 = Road.Add(map, new(19, 0, 19.5f), new(65, 0, 23), "ger1");
            var r2 = r1.Append(new(98, 0, 43.5f));

            var actual = r2.InterpolateCurveDist(999f);
            Assert.Null(actual);
        }

        [Fact]
        public void FindFirstItem()
        {
            var map = new Map("foo");
            var r1 = Road.Add(map, new(42, 0, 0), new(60, 0, 15), "ger1");
            var r2 = r1.Append(new(42, 0, 30));
            var r3 = r2.Append(new(25, 0, 18));
            var r4 = r3.Append(new(42, 0, 0));

            Assert.Equal(r1, r4.FindFirstItem());
            Assert.Equal(r1, r3.FindFirstItem());
            Assert.Equal(r1, r2.FindFirstItem());
            Assert.Equal(r1, r1.FindFirstItem());
        }

        [Fact]
        public void FindLastItem()
        {
            var map = new Map("foo");
            var r1 = Road.Add(map, new(42, 0, 0), new(60, 0, 15), "ger1");
            var r2 = r1.Append(new(42, 0, 30));
            var r3 = r2.Append(new(25, 0, 18));
            var r4 = r3.Append(new(42, 0, 0));

            Assert.Equal(r4, r4.FindLastItem());
            Assert.Equal(r4, r3.FindLastItem());
            Assert.Equal(r4, r2.FindLastItem());
            Assert.Equal(r4, r1.FindLastItem());
        }

        [Fact]
        public void FindFirstItemWithLoop()
        {
            var map = new Map("foo");
            var r1 = Road.Add(map, new(42, 0, 0), new(60, 0, 15), "ger1");
            var r2 = r1.Append(new(42, 0, 30));
            var r3 = r2.Append(new(25, 0, 18));
            var r4 = r3.Append(new(42, 0, 0));
            r1.Node.Merge(r4.ForwardNode);

            Assert.Equal(r1, r1.FindFirstItem());
            Assert.Equal(r2, r2.FindFirstItem());
            Assert.Equal(r3, r3.FindFirstItem());
            Assert.Equal(r4, r4.FindFirstItem());
        }

        [Fact]
        public void FindLastItemWithLoop()
        {
            var map = new Map("foo");
            var r1 = Road.Add(map, new(42, 0, 0), new(60, 0, 15), "ger1");
            var r2 = r1.Append(new(42, 0, 30));
            var r3 = r2.Append(new(25, 0, 18));
            var r4 = r3.Append(new(42, 0, 0));
            r1.Node.Merge(r4.ForwardNode);

            Assert.Equal(r1, r1.FindLastItem());
            Assert.Equal(r2, r2.FindLastItem());
            Assert.Equal(r3, r3.FindLastItem());
            Assert.Equal(r4, r4.FindLastItem());
        }

        [Fact]
        public void ClosedLoopHasCorrectNodeRotation()
        {
            var map = new Map("foo");
            var r1 = Road.Add(map, new(42, 0, 0), new(60, 0, 15), "ger1");
            var r2 = r1.Append(new(42, 0, 30));
            var r3 = r2.Append(new(25, 0, 18));
            var r4 = r3.Append(new(42, 0, 0));
            r1.Node.Merge(r4.ForwardNode);

            AssertEx.Equal(new(0, 0.72112f, 0, 0.69281f), r3.Node.Rotation, 0.001f);
            AssertEx.Equal(new(0, 0.0498041f, 0, 0.998759f), r4.Node.Rotation, 0.001f);
        }

        [Fact]
        public void RecalculateWithFreeRotationNodeInTheMiddle()
        {
            var map = new Map("foo");

            var r1 = Road.Add(map, new(16,0,23), new(66,0,8), "ger1");
            var r2 = r1.Append(new(125,0,17));
            var r3 = r2.Append(new(123,0,74));
            var r4 = r3.Append(new(74,0,103));

            var freeRot = Quaternion.CreateFromYawPitchRoll(MathEx.Rad(-170), 0, 0);
            r3.Node.Rotation = freeRot;
            r3.Node.FreeRotation = true;

            r1.Recalculate();

            AssertEx.Equal(new(0, 0.596931f, 0, -0.802293f), r1.Node.Rotation);
            AssertEx.Equal(new(0, 0.681915f, 0, -0.731431f), r2.Node.Rotation);
            AssertEx.Equal(freeRot, r3.Node.Rotation);
            AssertEx.Equal(new(0, 0.964337f, 0, 0.264677f), r4.Node.Rotation);
            AssertEx.Equal(new(0, 0.868712f, 0, 0.495318f), r4.ForwardNode.Rotation);
        }

        [Fact]
        public void RecalculateWithFreeRotationNodeAtTheStart()
        {
            var map = new Map("foo");

            var r1 = Road.Add(map, new(16, 0, 23), new(66, 0, 8), "ger1");
            var r2 = r1.Append(new(125, 0, 17));
            var r3 = r2.Append(new(123, 0, 74));
            var r4 = r3.Append(new(74, 0, 103));

            var freeRot = Quaternion.CreateFromYawPitchRoll(MathEx.Rad(-120), 0, 0);
            r1.Node.Rotation = freeRot;
            r1.Node.FreeRotation = true;

            r1.Recalculate();

            AssertEx.Equal(freeRot, r1.Node.Rotation);
            AssertEx.Equal(new(0, 0.681915f, 0, -0.731431f), r2.Node.Rotation);
            AssertEx.Equal(new(0, 0.940707f, 0, -0.339219f), r3.Node.Rotation);
            AssertEx.Equal(new(0, 0.964337f, 0, 0.264677f), r4.Node.Rotation);
            AssertEx.Equal(new(0, 0.868712f, 0, 0.495318f), r4.ForwardNode.Rotation);
        }

        [Fact]
        public void RecalculateWithFreeRotationNodeAtTheEnd()
        {
            var map = new Map("foo");

            var r1 = Road.Add(map, new(16, 0, 23), new(66, 0, 8), "ger1");
            var r2 = r1.Append(new(125, 0, 17));
            var r3 = r2.Append(new(123, 0, 74));
            var r4 = r3.Append(new(74, 0, 103));

            var freeRot = Quaternion.CreateFromYawPitchRoll(MathEx.Rad(90), 0, 0);
            r4.ForwardNode.Rotation = freeRot;
            r4.ForwardNode.FreeRotation = true;

            r1.Recalculate();

            AssertEx.Equal(new(0, 0.596931f, 0, -0.802293f), r1.Node.Rotation);
            AssertEx.Equal(new(0, 0.681915f, 0, -0.731431f), r2.Node.Rotation);
            AssertEx.Equal(new(0, 0.940707f, 0, -0.339219f), r3.Node.Rotation);
            AssertEx.Equal(new(0, 0.964337f, 0, 0.264677f), r4.Node.Rotation);
            AssertEx.Equal(freeRot, r4.ForwardNode.Rotation);
        }

    }
}
