using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap
{
    public class TriggerAction : ActionBase
    {
        /// <summary>
        /// Unit name of the action.
        /// </summary>
        public Token Name { get; set; }

        private const int typeStart = 0;
        private const int typeLength = 4; // presumably
        public ActionType Type
        {
            get => (ActionType)ActionFlags.GetBitString(typeStart, typeLength);
            set
            {
                ActionFlags.SetBitString(typeStart, typeLength, (uint)value);
            }
        }

        public override void Deserialize(BinaryReader r)
        {
            Name = r.ReadToken();
            base.Deserialize(r);
        }

        public override void Serialize(BinaryWriter w)
        {
            w.Write(Name);
            base.Serialize(w);
        }
    }
}
