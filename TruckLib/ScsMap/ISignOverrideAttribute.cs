using System;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// An interface for storing <see cref="SignOverrideAttribute">SignOverrideAttributes</see>
    /// of any type in a collection.
    /// </summary>
    public interface ISignOverrideAttribute
    {
        Type Type { get; }

        uint Index { get; set; }

        void SetValue(object value);

        object GetValue();
    }
}