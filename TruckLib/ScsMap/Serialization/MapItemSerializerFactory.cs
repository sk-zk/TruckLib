using System;
using System.Collections.Generic;
using System.Text;
using TruckLib.ScsMap.Serialization;

namespace TruckLib.ScsMap.Serialization
{
    internal class MapItemSerializerFactory
    {
        // don't create a new object every single time
        // because it's not necessary
        private static Dictionary<ItemType, MapItemSerializer> cached 
            = new Dictionary<ItemType, MapItemSerializer>();

        public static MapItemSerializer Get(ItemType type)
        {
            if (cached.TryGetValue(type, out var obj))
            {
                return obj;
            }
            else
            {
                var serializer = Create(type);
                cached.Add(type, serializer);
                return serializer;
            }
        }

        private static MapItemSerializer Create(ItemType type)
        {
            return type switch
            {
                ItemType.AnimatedModel => new AnimatedModelSerializer(),
                ItemType.BezierPatch => new BezierPatchSerializer(),
                ItemType.Building => new BuildingSerializer(),
                ItemType.BusStop => new BusStopSerializer(),
                ItemType.CameraPoint => new CameraPointSerializer(),
                ItemType.CityArea => new CityAreaSerializer(),
                ItemType.Company => new CompanySerializer(),
                ItemType.Compound => new CompoundSerializer(),
                ItemType.Curve => new CurveSerializer(),
                ItemType.CutPlane => new CutPlaneSerializer(),
                ItemType.FarModel => new FarModelSerializer(),
                ItemType.Ferry => new FerrySerializer(),
                ItemType.FuelPump => new ServiceSerializer(),
                ItemType.Garage => new GarageSerializer(),
                ItemType.Hinge => new HingeSerializer(),
                ItemType.MapArea => new MapAreaSerializer(),
                ItemType.MapOverlay => new MapOverlaySerializer(),
                ItemType.Model => new ModelSerializer(),
                ItemType.Mover => new MoverSerializer(),
                ItemType.NoWeatherArea => new NoWeatherAreaSerializer(),
                ItemType.Prefab => new PrefabSerializer(),
                ItemType.Road => new RoadSerializer(),
                ItemType.Service => new ServiceSerializer(),
                ItemType.Sign => new SignSerializer(),
                ItemType.Sound => new SoundSerializer(),
                ItemType.Terrain => new TerrainSerializer(),
                ItemType.TrafficArea => new TrafficAreaSerializer(),
                ItemType.Trajectory => new TrajectorySerializer(),
                ItemType.Trigger => new TriggerSerializer(),
                ItemType.Walker => new WalkerSerializer(),
                _ => throw new NotImplementedException($"Item type {type} is not supported."),
            };
        }
   
    }
}
