using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TruckLib.ScsMap.Collections;

namespace TruckLib.ScsMap.Serialization
{
    class TriggerSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var trigger = new Trigger(false);
            ReadKdopItem(r, trigger);

            trigger.Tags = ReadObjectList<Token>(r);
            trigger.Nodes = new PolygonNodeList(trigger);
            trigger.Nodes.AddRange(ReadNodeRefList(r));
            trigger.Actions = ReadObjectList<TriggerAction>(r);
            if (trigger.Nodes.Count == 1)
            {
                trigger.Range = r.ReadSingle();
            }

            return trigger;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var trigger = item as Trigger;
            WriteKdopItem(w, trigger);

            WriteObjectList(w, trigger.Tags);
            WriteNodeRefList(w, trigger.Nodes);
            WriteObjectList(w, trigger.Actions);
            if (trigger.Nodes.Count == 1)
            {
                w.Write(trigger.Range);
            }
        }
    }
}
