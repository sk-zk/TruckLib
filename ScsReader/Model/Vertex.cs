using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace ScsReader.Model
{
    public class Vertex
    {
        public Vector3 Position { get; set; }

        public Vector3 Normal { get; set; }

        public Vector4? Tangent { get; set; }

        public Color Color { get; set; }

        public Color? SecondaryColor { get; set; }

        public List<Vector2> TextureCoordinates { get; set; }

        public byte[] BoneIndexes { get; set; }

        public byte[] BoneWeights { get; set; }
    }
}
