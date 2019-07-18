using System;

namespace ScsReader.ScsMap
{
    public class MapItemFactory
    {
        /// <summary>
        /// Creates a new instance of the item with the given ItemType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MapItem Create(ItemType type)
        {
            switch (type)
            {
                case ItemType.AnimatedModel: return new AnimatedModel();
                case ItemType.BezierPatch: return new BezierPatch();
                case ItemType.Building: return new Building();
                case ItemType.BusStop: return new BusStop();
                case ItemType.CameraPoint: return new CameraPoint();
                case ItemType.City: return new CityArea();
                case ItemType.Company: return new Company();
                case ItemType.Compound: return new Compound();
                case ItemType.Curve: return new Curve();
                case ItemType.CutPlane: return new CutPlane();
                case ItemType.FarModel: return new FarModel();
                case ItemType.Ferry: return new Ferry();
                case ItemType.FuelPump: return new FuelPump();
                case ItemType.Garage: return new Garage();
                case ItemType.Hinge: return new Hinge();
                case ItemType.MapArea: return new MapArea();
                case ItemType.MapOverlay: return new MapOverlay();
                case ItemType.Model: return new Model();
                case ItemType.Mover: return new Mover();
                case ItemType.NoWeatherArea: return new NoWeatherArea();
                case ItemType.Prefab: return new Prefab();
                case ItemType.Road: return new Road();
                case ItemType.Service: return new Service();
                case ItemType.Sign: return new Sign();
                case ItemType.Sound: return new Sound();
                case ItemType.Terrain: return new Terrain();
                case ItemType.TrafficArea: return new TrafficArea();
                case ItemType.Trajectory: return new Trajectory();
                case ItemType.Trigger: return new Trigger();
                case ItemType.Walker: return new Walker();

                default:
                    throw new NotImplementedException($"Item type {type} is not supported.");
            }
        }

    }
}
