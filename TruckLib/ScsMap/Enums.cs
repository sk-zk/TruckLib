using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Item type IDs in .base files.
    /// </summary>
    public enum ItemType
    {
        Terrain = 0x01,
        Building = 0x02,
        Road = 0x03,
        Prefab = 0x04,
        Model = 0x05,
        Company = 0x06,
        Service = 0x07,
        CutPlane = 0x08,
        Mover = 0x09,
        NoWeatherArea = 0x0B,
        City = 0x0C,
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
        Curve = 0x2C
    };

    /// <summary>
    /// Sector files that can hold an item.
    /// </summary>
    public enum ItemFile
    {
        Base,
        Aux
    }

    /// <summary>
    /// Allowed terrain noise values for road terrain.
    /// </summary>
    public enum TerrainNoise
    {
        Percent100 = 0,
        Percent50 = 1,
        Percent0 = 2
    }

    /// <summary>
    /// Allowed transition values for road terrain.
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
        NoDetailVegetation = 1,
        NoVegetation = 2
    }

    /// <summary>
    /// Vegetation modes for veg. spheres.
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

    // Some prefab setting. TODO: Find out what this does
    public enum StaticLod
    {
        Default = 0,
        LodFrom50m = 1,
        LodFrom100m = 2
    }

    /// <summary>
    /// Quad resolution of a standalone terrain. 
    /// Roads would do this with hi-poly and superfine flags.
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

        // TODO: What does this do?
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

}
