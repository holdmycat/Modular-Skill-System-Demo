using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public abstract class BaseTeamRuntime : NetworkBehaviour
    {
       
        private static readonly ILog log = LogManager.GetLogger(typeof(BaseTeamRuntime));
        protected long _teamId;
        protected long _ownerNetId;
        protected FactionType _factionType;
        
        protected List<long> _squadList;
        
        public virtual void InitTeamRuntime(TeamSpawnPayload payload)
        {
            _teamId = payload.TeamId;
            _squadList = payload.SquadList;
            _factionType = payload.Faction;
            _ownerNetId = payload.OwnerNetId;
            
            log.Debug("[BaseTeamRuntime] InitTeamRuntime.");
        }

        protected abstract void ConstructSquads();

        public override async UniTask ShutdownAsync()
        {
            await base.ShutdownAsync();
            if (null != _squadList)
            {
                _squadList.Clear();
            }
            
            log.Debug("[BaseTeamRuntime] ShutdownAsync.");
        }
        
    }
}
