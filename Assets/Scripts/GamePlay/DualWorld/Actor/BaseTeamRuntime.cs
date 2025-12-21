using System.Collections.Generic;
using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    public abstract class BaseTeamRuntime : NetworkBehaviour
    {
       
       
        protected long _teamId;
        protected FactionType _factionType;
        
        protected List<long> _squadList;
        
        public virtual void InitTeamRuntime(TeamSpawnPayload payload)
        {
            _teamId = payload.TeamId;
            _squadList = payload.SquadList;
            _factionType = payload.Faction;
        }

        protected abstract void ConstructSquads();

    }
}
