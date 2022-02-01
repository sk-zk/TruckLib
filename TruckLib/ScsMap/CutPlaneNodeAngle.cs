using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.ScsMap
{
    public struct CutPlaneNodeAngle
    {
        private const int factor = 3;
        private const int maxValue = 180;
        private const int minValue = -180;

        internal sbyte InternalValue { get; private set; }

        internal CutPlaneNodeAngle(sbyte value)
        {
            InternalValue = value;
        }

        public static implicit operator CutPlaneNodeAngle(int v)
        {
            if (v < minValue || v > maxValue)
                throw new ArgumentOutOfRangeException(nameof(v), $"Value must be between {minValue} and {maxValue}.");

            if (v % factor != 0)
                throw new ArgumentOutOfRangeException(nameof(v), $"Value must be divisible by {factor}.");

            return new CutPlaneNodeAngle((sbyte)(v / factor));
        }

        public static implicit operator sbyte(CutPlaneNodeAngle c)
            => c.InternalValue;

        public override string ToString() 
            => (InternalValue * factor).ToString();

        public override int GetHashCode() =>
            InternalValue.GetHashCode();

        public override bool Equals(object obj) =>
            obj is CutPlaneNodeAngle angle && angle.InternalValue == this.InternalValue;

        public static bool operator ==(CutPlaneNodeAngle left, CutPlaneNodeAngle right) => 
            left.Equals(right);

        public static bool operator !=(CutPlaneNodeAngle left, CutPlaneNodeAngle right) => 
            !(left == right);

    }
}
