//------------------------------------------------------------
// File: LegionIdGenerator.cs
// Purpose: Generate Commander/Legion ids deterministically.
//------------------------------------------------------------
using System.Collections.Concurrent;

namespace Ebonor.DataCtrl
{
    public class LegionIdGenerator : ILegionIdGenerator
    {
        // Global counter for all commanders.
        // Start from ROOM_MANAGER_NET_ID (1) so the first generated ID is 2.
        private static int _globalCommanderIdCounter = (int)NetworkConstants.ROOM_MANAGER_NET_ID;
     
        private readonly ConcurrentDictionary<uint, uint> _counters = new ConcurrentDictionary<uint, uint>();
        
        public ulong Next(uint commanderNetId)
        {
            // Increment per commander and compose the LegionId.
            var nextIndex = _counters.AddOrUpdate(commanderNetId, 1, (_, current) => unchecked(current + 1));
            return Compose(commanderNetId, nextIndex);
        }

        public ulong Compose(uint commanderNetId, uint localLegionIndex)
        {
            return ((ulong)commanderNetId << 32) | localLegionIndex;
        }

        public (uint commanderNetId, uint localLegionIndex) Decompose(ulong legionId)
        {
            uint commanderId = (uint)(legionId >> 32);
            uint localIndex = (uint)(legionId & 0xFFFFFFFF);
            return (commanderId, localIndex);
        }


    }
}
