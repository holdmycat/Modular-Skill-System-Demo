//------------------------------------------------------------
// File: ActorIdGenerator.cs
// Purpose: Global static generator for unique actor IDs (uint, starting from 0).
//------------------------------------------------------------
using System.Threading;

namespace Ebonor.DataCtrl
{
    public static class ActorIdGenerator
    {
        // Start at -1 so the first increment returns 0.
        private static int _counter = -1;

        /// <summary>Get the next unique actor ID (uint, starts at 0).</summary>
        public static uint NextId()
        {
            return unchecked((uint)Interlocked.Increment(ref _counter));
        }
    }
}
