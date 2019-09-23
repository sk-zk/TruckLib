using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A sign override attribute, used to define which attributes of an
    /// object in a sign template have been changed.
    /// </summary>
    public class SignOverrideAttribute<T> : ISignOverrideAttribute
    {
        /// <summary>
        /// The index of the attribute in the attribute list of the object.
        /// </summary>
        public uint Index { get; set; }

        public T Value { get; set; }

        Type ISignOverrideAttribute.Type { get => typeof(T); }

        void ISignOverrideAttribute.SetValue(object value)
        {
            Value = (T)value;
        }

        object ISignOverrideAttribute.GetValue()
        {
            return Value;
        }
    }
}
