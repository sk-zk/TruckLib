using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Item type IDs in .base/.aux/.snd files.
    /// </summary>
    public enum ItemType
    {
        Terrain = 0x01,
        Buildings = 0x02,
        Road = 0x03,
        Prefab = 0x04,
        Model = 0x05,
        Company = 0x06,
        Service = 0x07,
        CutPlane = 0x08,
        Mover = 0x09,
        NoWeatherArea = 0x0B,
        CityArea = 0x0C,
        Hinge = 0x0D,
        AnimatedModel = 0x0F,
        MapOverlay = 0x12,
        Ferry = 0x13,
        Sound = 0x15,
        Garage = 0x16,
        CameraPoint = 0x17,
        Walker = 0x1C,
        Trigger = 0x22,
        FuelPump = 0x23,
        Sign = 0x24,
        BusStop = 0x25,
        TrafficArea = 0x26,
        BezierPatch = 0x27,
        Compound = 0x28,
        Trajectory = 0x29,
        MapArea = 0x2A,
        FarModel = 0x2B,
        Curve = 0x2C,
        CameraPath = 0x2D,
        Cutscene = 0x2E,
    };

    /// <summary>
    /// Sector files that can hold an item.
    /// </summary>
    public enum ItemFile
    {
        Base,
        Aux,
        Snd
    }

    /// <summary>
    /// Allowed terrain noise values for terrain.
    /// </summary>
    public enum TerrainNoise
    {
        Percent100 = 0,
        Percent50 = 1,
        Percent0 = 2
    }

    /// <summary>
    /// Allowed transition values for terrain.
    /// </summary>
    public enum TerrainTransition
    {
        _4 = 2,
        _8 = 1,
        _16 = 0,
        _32 = 3
    }

    /// <summary>
    /// Allowed values for sidewalk size (in the legacy pre-edge system).
    /// </summary>
    public enum SidewalkSize
    {
        Meters0 = 3,
        Meters2 = 2,
        Meters4 = 1,
        Meters8 = 0
    }

    /// <summary>
    /// Vegetation modes for terrain quads.
    /// </summary>
    public enum QuadVegetation
    {
        Normal = 0,
        NoVegetation = 1,
        LowPolyVegetation = 2,
        HighPolyVegetation = 3,
    }

    /// <summary>
    /// Vegetation modes for vegetation spheres.
    /// </summary>
    public enum VegetationSphereType
    {
        NoVegetation = 1,
        LowPolyVegetation = 2,
        HighPolyVegetation = 3
    }

    /// <summary>
    /// Scale of vegetation models.
    /// </summary>
    public enum VegetationScale
    {
        Percent80to120 = 0,
        Percent60to100 = 1,
        Percent50to80 = 2,
        Percent100to140 = 3
    }

    /// <summary>
    /// Quad resolution of a standalone terrain. 
    /// </summary>
    public enum StepSize
    {
        Meters2 = 3,
        Meters4 = 0,
        Meters12 = 2,
        Meters16 = 1
    }

    /// <summary>
    /// The resolution / step size of a road.
    /// </summary>
    public enum RoadResolution
    {
        /// <summary>
        /// 15m
        /// </summary>
        Normal, 

        /// <summary>
        /// 5m
        /// </summary>
        HighPoly,

        /// <summary>
        /// 1m (non-template only)
        /// </summary>
        Superfine
    }

    /// <summary>
    /// The type of a MapOverlay.
    /// </summary>
    public enum OverlayType
    {
        /// <summary>
        /// The item will display an image specified in Look.
        /// </summary>
        RoadName = 0,

        /// <summary>
        /// The item will display a rest area symbol.
        /// </summary>
        Parking = 1,

        /// <summary>
        /// The item will display the name of a city like a City item.
        /// </summary>
        CityName = 2,

        // TODO: What does this do?
        UserText = 4
    }

    /// <summary>
    /// The type of a MapArea.
    /// </summary>
    public enum MapAreaType
    {
        Visual = 0,
        Navigation = 1
    }

    /// <summary>
    /// The color of a MapArea.
    /// </summary>
    public enum MapAreaColor
    {
        Road = 0,
        Light = 1,
        Dark = 2,
        Green = 3
    }

    public enum ServiceType
    {
        GasStation = 0,
        ServiceStation = 1,
        TruckDealer = 2,
        Parking = 4,
        Recruitment = 5,
        WeighStation = 7,
        WeighStationCat = 8,
    }

    public enum CutsceneType
    {
        Viewpoint = 0,
        Event = 1
    }

    public enum ModelLod
    {
        Dynamic = 0,
        LodFrom100m = 1,
        LodFrom200m = 2
    }

    public enum SoundType
    {
        ThreeDSound = 0,
        AmbientArea = 1,
        ReverbArea = 2
    }

}
