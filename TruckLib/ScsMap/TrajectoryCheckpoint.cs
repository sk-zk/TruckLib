﻿using System;

namespace TruckLib.ScsMap
{
    public class TrajectoryCheckpoint
    {
        public Token Checkpoint { get; set; }

        public Token Route { get; set; }

        public override string ToString()
        {
            return $"{Route} - {Checkpoint}";
        }
    }
}
