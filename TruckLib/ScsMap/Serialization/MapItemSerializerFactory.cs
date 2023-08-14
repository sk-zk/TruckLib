using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    internal class MapItemSerializerFactory
    {
        private static readonly Dictionary<ItemType, MapItemSerializer> cache
            = new Dictionary<ItemType, MapItemSerializer>();

        public static MapItemSerializer Get(ItemType type)
        {
            if (cache.TryGetValue(type, out var cached))
            {
                return cached;
            }
            else
            {
                var serializer = Create(type);
                cache.Add(type, serializer);
                return serializer;
            }
        }

        private static MapItemSerializer Create(ItemType type)
        {
            return type switch
            {
                ItemType.AnimatedModel => new AnimatedModelSerializer(),
                ItemType.BezierPatch => new BezierPatchSerializer(),
                ItemType.Buildings => new BuildingsSerializer(),
                ItemType.BusStop => new BusStopSerializer(),
                ItemType.CameraPath => new CameraPathSerializer(),
                ItemType.CameraPoint => new CameraPointSerializer(),
                ItemType.CityArea => new CityAreaSerializer(),
                ItemType.Company => new CompanySerializer(),
                ItemType.Compound => new CompoundSerializer(),
                ItemType.Curve => new CurveSerializer(),
                ItemType.CutPlane => new CutPlaneSerializer(),
                ItemType.Cutscene => new CutsceneSerializer(),
                ItemType.FarModel => new FarModelSerializer(),
                ItemType.Ferry => new FerrySerializer(),
                ItemType.FuelPump => new ServiceSerializer(),
                ItemType.Garage => new GarageSerializer(),
                ItemType.Gate => new GateSerializer(),
                ItemType.Hinge => new HingeSerializer(),
                ItemType.Hookup => new HookupSerializer(),
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
                ItemType.VisibilityArea => new VisibilityAreaSerializer(),
                ItemType.Walker => new WalkerSerializer(),
                _ => throw new NotImplementedException($"Item type {type} is not supported."),
            };
        }
   
    }
}
