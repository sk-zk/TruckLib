using System.Numerics;

namespace TruckLib.ScsMap
{
    public interface IMapItem : IMapObject
    {
        ItemFile DefaultItemFile { get; }
        ItemFile ItemFile { get; }
        ItemType ItemType { get; }

        void Move(Vector3 newPos);
        void Translate(Vector3 translation);
    }
}