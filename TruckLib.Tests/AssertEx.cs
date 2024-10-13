using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLibTests
{
    public static class AssertEx
    {
        public static void Equal(Vector3 expected, Vector3 actual, float tolerance = 0.000001f)
        {
            Assert.Equal(expected.X, actual.X, tolerance);
            Assert.Equal(expected.Y, actual.Y, tolerance);
            Assert.Equal(expected.Z, actual.Z, tolerance);
        }

        public static void Equal(Quaternion expected, Quaternion actual, float tolerance = 0.000001f)
        {
            Assert.True(QuaternionsEqual(expected,actual,tolerance));
        }

        private static bool QuaternionsEqual(Quaternion q1, Quaternion q2, float tolerance)
        {
            return ((Math.Abs(q2.X - q1.X) < tolerance)
                   && (Math.Abs(q2.Y - q1.Y) < tolerance)
                   && (Math.Abs(q2.Z - q1.Z) < tolerance)
                   && (Math.Abs(q2.W - q1.W) < tolerance)) 
                || ((Math.Abs(-q2.X - q1.X) < tolerance)
                   && (Math.Abs(-q2.Y - q1.Y) < tolerance)
                   && (Math.Abs(-q2.Z - q1.Z) < tolerance)
                   && (Math.Abs(-q2.W - q1.W) < tolerance));
        }
    }
}
