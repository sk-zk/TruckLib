using System;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Used for deserialization and collections of SignOverrideAttribute objects.
    /// </summary>
    public interface ISignOverrideAttribute
    {
        Type Type { get; }

        uint Index { get; set; }

        void SetValue(object value);

        object GetValue();
    }
}