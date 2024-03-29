﻿using System.Numerics;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Properties of a model specific to <see cref="FarModel">Far Model</see> items.
    /// </summary>
    public struct FarModelData
    {
        /// <summary>
        /// The map node which defines the position and rotation of the model.
        /// </summary>
        public INode Node { get; set; }

        /// <summary>
        /// Unit name of the model, as defined in <c>/def/world/far_model.sii</c>.
        /// </summary>
        public Token Model { get; set; }

        /// <summary>
        /// Relative scale per axis.
        /// </summary>
        public Vector3 Scale { get; set; }

        public FarModelData(INode node, Token model, Vector3 scale)
        {
            Node = node;
            Model = model;
            Scale = scale;
        }
    }
}
