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
        EnvironmentArea = 0x0B,
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
        Hookup = 0x2F,
        VisibilityArea = 0x30,
        Gate = 0x31,
    };

    /// <summary>
    /// Sector files that can hold <see cref="MapItem">map items</see>.
    /// </summary>
    public enum ItemFile
    {
        Base,
        Aux,
        Snd
    }

    /// <summary>
    /// Allowed terrain noise values in percent.
    /// </summary>
    public enum TerrainNoise
    {
        Percent100 = 0,
        Percent50 = 1,
        Percent0 = 2
    }

    /// <summary>
    /// Allowed transition values for terrain in number of quads.
    /// </summary>
    public enum TerrainTransition
    {
        Quads4 = 2,
        Quads8 = 1,
        Quads16 = 0,
        Quads32 = 3
    }

    /// <summary>
    /// Allowed values for sidewalk size in meters in the legacy pre-edge system.
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
    /// Defines the behavior of a vegetation sphere.
    /// </summary>
    public enum VegetationSphereType
    {
        NoVegetation = 1,
        LowPolyVegetation = 2,
        HighPolyVegetation = 3
    }

    /// <summary>
    /// Scale of vegetation models in percent.
    /// </summary>
    public enum VegetationScale
    {
        Percent80to120 = 0,
        Percent60to100 = 1,
        Percent50to80 = 2,
        Percent100to140 = 3,
        Percent35to50 = 4,
        Percent100 = 5,
        Percent80 = 6,
        Percent60 = 7,
    }

    /// <summary>
    /// Quad resolution of a <see cref="Terrain"/> item.
    /// </summary>
    public enum StepSize
    {
        Meters2 = 3,
        Meters4 = 0,
        Meters12 = 2,
        Meters16 = 1
    }

    /// <summary>
    /// The resolution / step size of a <see cref="Road">road</see>.
    /// </summary>
    public enum RoadResolution
    {
        /// <summary>
        /// 15 m
        /// </summary>
        Normal, 

        /// <summary>
        /// 5 m
        /// </summary>
        HighPoly,

        /// <summary>
        /// 1 m (non-template only)
        /// </summary>
        Superfine
    }

    /// <summary>
    /// The type of a <see cref="MapOverlay">Map Overlay</see>.
    /// </summary>
    public enum OverlayType
    {
        /// <summary>
        /// The item will display an image specified in <see cref="MapOverlay.Look">Look</see>.
        /// </summary>
        RoadName = 0,

        /// <summary>
        /// The item will display a rest area symbol.
        /// </summary>
        Parking = 1,

        /// <summary>
        /// The item will display the name of a city specified in <see cref="MapOverlay.Look">Look</see>.
        /// </summary>
        CityName = 2,

        /// <summary>
        /// TODO: What does this do?
        /// </summary>
        UserText = 3,

        /// <summary>
        /// TODO: What does this do?
        /// </summary>
        Landmark = 4,
    }

    /// <summary>
    /// The type of a <see cref="MapArea">Map Area</see>.
    /// </summary>
    public enum MapAreaType
    {
        /// <summary>
        /// The map area will be drawn onto the UI map as a polygon.
        /// </summary>
        Visual = 0,

        /// <summary>
        /// TODO: What on earth does this do?
        /// </summary>
        Navigation = 1
    }

    /// <summary>
    /// The spawn point type of a <see cref="Service"/> item.
    /// </summary>
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

    /// <summary>
    /// The type of a <see cref="Cutscene"/> item.
    /// </summary>
    public enum CutsceneType
    {
        Viewpoint = 0,
        Event = 1,
        MapScene = 2,
    }

    /// <summary>
    /// The type of a <see cref="Sound"/> item.
    /// </summary>
    public enum SoundType
    {
        ThreeDSound = 0,
        AmbientArea = 1,
        ReverbArea = 2
    }

    /// <summary>
    /// The rendering behavior of a <see cref="VisibilityArea">visibility area</see>.
    /// </summary>
    public enum VisibilityAreaBehavior
    {
        /// <summary>
        /// Ignores cut planes for child objects.
        /// </summary>
        ShowHiddenObjects = 0,

        /// <summary>
        /// Child objects are invisible inside the area.
        /// </summary>
        HideObjects = 1,

        /// <summary>
        /// Child objects are only visible inside the area.
        /// </summary>
        ShowObjects = 2
    }

    /// <summary>
    /// Easing functions used by <see cref="Keyframe">keyframes</see>.
    /// </summary>
    public enum EasingFunction
    {
        Custom = 0,

        EaseInCubic = 9,
        EaseInExpo = 11,
        EaseInQuad = 8,
        EaseInQuart = 10,
        EaseInSine = 7,

        EaseInOutCubic = 4,
        EaseInOutExpo = 6,
        EaseInOutQuad = 3,
        EaseInOutQuart = 5,
        EaseInOutSine = 2,

        EaseOutCubic = 14,
        EaseOutExpo = 16,
        EaseOutQuad = 13,
        EaseOutQuart = 15,
        EaseOutSine = 12,

        Linear = 1
    }

    /// <summary>
    /// Fog behavior in a <see cref="EnvironmentArea">Environment Area</see>.
    /// </summary>
    public enum FogMask
    {
        None = 0,

        /// <summary>
        /// No fog falloff.
        /// </summary>
        Indoor = 1,

        /// <summary>
        /// Short directional falloff.
        /// </summary>
        Tunnel = 2,

        /// <summary>
        /// Long directional falloff.
        /// </summary>
        Overpass = 3,

        /// <summary>
        /// Long bidirectional falloff.
        /// </summary>
        OpenArea = 4,
    }

    /// <summary>
    /// Blinker type for <see cref="Trajectory"/> items.
    /// </summary>
    public enum BlinkerType
    {
        NoBlinker = 0,
        NoBlinkerForced = 1,
        RightBlinker = 2,
        LeftBlinker = 4,
    }

    /// <summary>
    /// The behavior of a <see cref="Gate"/>.
    /// </summary>
    public enum GateType
    {
        TriggerActivated = 0,
        AlwaysOpen = 1,
        AlwaysClosed = 2,
    }

    /// <summary>
    /// The node alignment of a <see cref="Hookup"/>.
    /// </summary>
    public enum HookupNodeAlignment
    {
        /// <summary>
        /// The model is placed in its default position.
        /// </summary>
        None = 0,
        /// <summary>
        /// The hookup node is the position of the front end of the model.
        /// </summary>
        Front = 1,
        /// <summary>
        /// The hookup node is the position of the rear end of the model.
        /// </summary>
        Rear = 2,
    }

    /// <summary>
    /// The likelihood in percent that a <see cref="Hookup"/> will spawn a model.
    /// </summary>
    public enum HookupSpawnProbability
    {
        Default = 0,
        Percent25 = 1,
        Percent50 = 2,
        Percent75 = 3,
    }

    /// <summary>
    /// The LOD of the spawned model of a <see cref="Hookup"/>.
    /// </summary>
    public enum HookupModelDetail
    {
        Default = 0,
        Average = 1,
        Low = 2,
        VeryLow = 3,
    }

    /// <summary>
    /// The spawn point type of a node belonging to a <see cref="Company"/> item.
    /// </summary>
    public enum CompanySpawnPointType
    {
        /// <summary>
        /// Easy difficulty (15 XP) parking spot.
        /// </summary>
        UnloadEasy = 1,
        /// <summary>
        /// Medium difficulty (40 XP) parking spot.
        /// </summary>
        UnloadMedium = 2,
        /// <summary>
        /// Hard difficulty (90 XP) parking spot.
        /// </summary>
        UnloadHard = 3,
        /// <summary>
        /// Trailer spawn point.
        /// </summary>
        Trailer = 4,
    }

    /// <summary>
    /// Action type for <see cref="Trigger">triggers</see>.
    /// </summary>
    public enum ActionType
    {
        Default = 0,
        Condition = 1,
        Fallback = 2,
        Mandatory = 3,
        ConditionRetry = 4
    }

}
