using Ebonor.DataCtrl;
using UnityEngine;

namespace Ebonor.GamePlay
{
    public abstract class BaseSquadRuntime : NetworkBehaviour
    {
        protected long _squadId;
        protected long _ownerNetId;
        protected long _teamNetId;
        protected FactionType _factionType;

        public virtual void InitSquadRuntime(SquadSpawnPayload payload)
        {
            _squadId = payload.SquadId;
            _ownerNetId = payload.OwnerNetId;
            _teamNetId = payload.TeamNetId;
            _factionType = payload.Faction;
        }

        // Logic for managing units will be added here
    }
}
