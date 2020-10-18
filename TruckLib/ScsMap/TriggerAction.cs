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
