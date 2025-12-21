using System;
using System.Collections.Generic;

namespace Ebonor.DataCtrl
{
    [Serializable]
    public struct TeamSpawnPayload
    {
        public FactionType Faction;
        public long TeamId;
        public uint OwnerNetId; // NetId of the owner Actor
        public List<long> SquadList;
    }

    [Serializable]
    public struct SquadSpawnPayload
    {
        public long TeamId;
        // Add more squad-specific init data here
    }
}
