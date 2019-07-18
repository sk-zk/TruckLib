using System;

namespace ScsReader.ScsMap
{
    public class TrajectoryCheckpoint
    {
        public Token Checkpoint;

        public Token Route;

        public override string ToString()
        {
            return $"{Route} - {Checkpoint}";
        }
    }
}
