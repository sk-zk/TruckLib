using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Models.Ppd
{
    public enum AllowedVehicles
    {
        PlayerOnly = 0,
        SmallVehicles = 1,
        LargeVehicles = 2,
        AllVehicles = 3
    }

    public enum BlinkerType
    {
        NoBlinker = 0,
        NoBlinkerForced = 0b001,
        RightBlinker = 0b010,
        LeftBlinker = 0b100
    }

    /// <summary>
    /// Type of a prefab semaphore.
    /// </summary>
    public enum SemaphoreType
    {
        UseProfile = 0,
        ModelOnly = 1,
        TrafficLight = 2,
        TrafficLightMinor = 3,
        TrafficLightMajor = 4,
        BarrierManualTimed = 5,
        BarrierDistanceActivated = 6,
        TrafficLightBlockable = 7,
        BarrierGas = 8,
        TrafficLightVirtual = 9,
        BarrierAutomatic = 10,
    }

    /// <summary>
    /// Spawn type of a SpawnPoint prefab locator.
    /// </summary>
    public enum SpawnPointType
    {
        BusStation = 18,
        /// <summary>
        /// The point where a garage can be bought.
        /// </summary>
        BuyPoint = 15,
        CameraPoint = 17,
        CompanyPoint = 13,
        CompanyUnload = 23,
        Custom = 9,
        GaragePoint = 14,
        GasStation = 3,
        Hotel = 8,
        LongTrailer = 25,
        Meet = 12,
        None = 0,
        Parking = 10,
        Recruitment = 16,
        ServiceStation = 4,
        Task = 11,
        Trailer = 1,
        /// <summary>
        ///  The point where an owned trailer is spawned at a garage.
        /// </summary>
        TrailerSpawn = 24,
        TruckDealer = 7,
        TruckStop = 5,
        UnloadEasy = 2,
        UnloadHard = 20,
        UnloadMedium = 19,
        UnloadRigid = 21,
        WeighStationCat = 22,
        WeighStation = 6,
    }

    /// <summary>
    /// The type of road to draw as the UI map representation of a prefab road.
    /// </summary>
    public enum RoadSize
    {
        OneWay = 0,
        OneLane = 1,
        TwoLanes = 2,
        ThreeLanes = 3,
        FourLanes = 4,
        TwoLaneSplit = 5,
        ThreeLaneSplit = 6,
        FourLaneSplit = 7,
        ThreeLanesOneWay = 8,
        Polygon = 13,
        Auto = 14,
    }

    /// <summary>
    /// Distance between road lanes in the UI map representation of a prefab road.
    /// </summary>
    public enum RoadOffset
    {
        Meters0 = 0,
        Meters1 = 1,
        Meters2 = 2,
        Meters5 = 3,
        Meters10 = 4,
        Meters15 = 5,
        Meters20 = 6,
        Meters25 = 7,
    }

    /// <summary>
    /// Color of prefab MapPoint polygons.
    /// </summary>
    public enum CustomColor
    {
        None = 0,

        /// <summary>
        /// Used for accessible prefab areas.
        /// </summary>
        Light = 1,

        /// <summary>
        /// Used for buildings.
        /// </summary>
        Dark = 2,

        /// <summary>
        /// Used for grass and inaccessible prefab areas.
        /// </summary>
        Green = 4
    }
}
