using System.Numerics;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Properties of a model specific to <see cref="FarModel">Far Model</see> items.
    /// </summary>
    public struct FarModelData
    {
        /// <summary>
        /// Unit name of the model.
        /// </summary>
        public Token Model;

        /// <summary>
        /// Relative scale per axis.
        /// </summary>
        public Vector3 Scale;

        /// <summary>
        /// This node defines the position and rotation of the model.
        /// </summary>
        public INode Node;
    }
}
