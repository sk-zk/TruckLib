﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TruckLib.ScsMap.Collections;

namespace TruckLib.ScsMap.Serialization
{
    class CameraPathSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var path = new CameraPath(false);
            ReadKdopItem(r, path);

            path.Tags = ReadObjectList<Token>(r);
            path.Nodes = new PathNodeList(path);
            path.Nodes.AddRange(ReadNodeRefList(r), false);
            path.TrackPoints = ReadNodeRefList(r);
            path.ControlNodes = ReadNodeRefList(r);
            path.Keyframes = ReadObjectList<Keyframe>(r);
            path.CameraSpeed = r.ReadSingle();

            return path;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var path = item as CameraPath;
            WriteKdopItem(w, path);

            WriteObjectList(w, path.Tags);
            WriteNodeRefList(w, path.Nodes);
            WriteNodeRefList(w, path.TrackPoints);
            WriteNodeRefList(w, path.ControlNodes);
            WriteObjectList(w, path.Keyframes);
            w.Write(path.CameraSpeed);
        }
    }
}
