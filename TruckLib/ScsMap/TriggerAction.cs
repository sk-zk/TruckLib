using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Action properties of a <see cref="Trigger"/>.
    /// </summary>
    public class TriggerAction : ActionBase
    {
        /// <summary>
        /// Unit name of the action.
        /// </summary>
        public Token Name { get; set; }

        private const int typeStart = 0;
        private const int typeLength = 4; // presumably
        /// <summary>
        /// The type of the action.
        /// </summary>
        public ActionType Type
        {
            get => (ActionType)ActionFlags.GetBitString(typeStart, typeLength);
            set
            {
                ActionFlags.SetBitString(typeStart, typeLength, (uint)value);
            }
        }

        /// <inheritdoc/>
        public override void Deserialize(BinaryReader r, uint? version = null)
        {
            Name = r.ReadToken();
            base.Deserialize(r);
        }

        /// <inheritdoc/>
        public override void Serialize(BinaryWriter w)
        {
            w.Write(Name);
            base.Serialize(w);
        }
    }
}
