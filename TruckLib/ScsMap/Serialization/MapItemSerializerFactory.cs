using System;
using System.Collections.Generic;
using System.Text;

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
            switch (type)
            {
                case ItemType.AnimatedModel:
                    return new AnimatedModelSerializer();
                case ItemType.BezierPatch:
                    return new BezierPatchSerializer();
                case ItemType.Building:
                    return new BuildingSerializer();
                case ItemType.BusStop:
                    return new BusStopSerializer();
                case ItemType.CameraPath:
                    return new CameraPathSerializer();
                case ItemType.CameraPoint:
                    return new CameraPointSerializer();
                case ItemType.CityArea:
                    return new CityAreaSerializer();
                case ItemType.Company:
                    return new CompanySerializer();
                case ItemType.Compound:
                    return new CompoundSerializer();
                case ItemType.Curve:
                    return new CurveSerializer();
                case ItemType.CutPlane:
                    return new CutPlaneSerializer();
                case ItemType.Cutscene:
                    return new CutsceneSerializer();
                case ItemType.FarModel:
                    return new FarModelSerializer();
                case ItemType.Ferry:
                    return new FerrySerializer();
                case ItemType.FuelPump:
                    return new ServiceSerializer();
                case ItemType.Garage:
                    return new GarageSerializer();
                case ItemType.Hinge:
                    return new HingeSerializer();
                case ItemType.MapArea:
                    return new MapAreaSerializer();
                case ItemType.MapOverlay:
                    return new MapOverlaySerializer();
                case ItemType.Model:
                    return new ModelSerializer();
                case ItemType.Mover:
                    return new MoverSerializer();
                case ItemType.NoWeatherArea:
                    return new NoWeatherAreaSerializer();
                case ItemType.Prefab:
                    return new PrefabSerializer();
                case ItemType.Road:
                    return new RoadSerializer();
                case ItemType.Service:
                    return new ServiceSerializer();
                case ItemType.Sign:
                    return new SignSerializer();
                case ItemType.Sound:
                    return new SoundSerializer();
                case ItemType.Terrain:
                    return new TerrainSerializer();
                case ItemType.TrafficArea:
                    return new TrafficAreaSerializer();
                case ItemType.Trajectory:
                    return new TrajectorySerializer();
                case ItemType.Trigger:
                    return new TriggerSerializer();
                case ItemType.Walker:
                    return new WalkerSerializer();
                default:
                    throw new NotImplementedException($"Item type {type} is not supported.");
            }

        }
    }
}
