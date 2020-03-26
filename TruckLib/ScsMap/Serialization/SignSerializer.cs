using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class SignSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var sign = new Sign();
            ReadKdopItem(r, sign);

            sign.Model = r.ReadToken();
            sign.Node = new UnresolvedNode(r.ReadUInt64());

            sign.Look = r.ReadToken();
            sign.Variant = r.ReadToken();
            
            // sign_boards
            // used for legacy signs.
            var boardCount = r.ReadByte();
            if (boardCount > 0) // yes, this is correct
            {
                for (int i = 0; i < sign.SignBoards.Length; i++)
                {
                    sign.SignBoards[i].Road = r.ReadToken();
                    sign.SignBoards[i].City1 = r.ReadToken();
                    sign.SignBoards[i].City2 = r.ReadToken();
                }
            }

            // override_template
            sign.SignTemplate = r.ReadPascalString();
            // if override_template is an empty string,
            // the file does not contain the sign override array
            if (sign.SignTemplate == "") return sign;

            // sign_override
            sign.SignOverrides = ReadObjectList<SignOverride>(r);

            return sign;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var sign = item as Sign;
            WriteKdopItem(w, sign);

            w.Write(sign.Model);
            w.Write(sign.Node.Uid);
                    
            w.Write(sign.Look);
            w.Write(sign.Variant);

            w.Write((byte)sign.SignBoards.Length);
            if (sign.SignBoards.Length > 0)
            {
                foreach (var board in sign.SignBoards)
                {
                    w.Write(board.Road);
                    w.Write(board.City1);
                    w.Write(board.City2);
                }
            }

            w.WritePascalString(sign.SignTemplate);
            if (sign.SignTemplate == "") return;

            WriteObjectList(w, sign.SignOverrides);
        }
    }
}
