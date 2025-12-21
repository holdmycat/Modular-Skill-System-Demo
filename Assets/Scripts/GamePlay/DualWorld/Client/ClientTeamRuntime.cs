// ---------------------------------------------
// Script: ClientTeamRuntime.cs
// Purpose: Client representation of a team, tracking squad runtimes by NetId and squad identifiers.
// ---------------------------------------------
using System.Collections.Generic;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ClientTeamRuntime : BaseTeamRuntime
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientTeamRuntime));
        private readonly Dictionary<uint, ClientSquadRuntime> _squadRuntimesByNetId = new Dictionary<uint, ClientSquadRuntime>();
        private readonly Dictionary<long, ClientSquadRuntime> _squadRuntimesBySquadId = new Dictionary<long, ClientSquadRuntime>();

        public ClientTeamRuntime(uint netId)
        {
            log.Debug("[ClientTeamRuntime] Constructed.");
            BindId(netId);//client team
        }
        
        public override void InitTeamRuntime(TeamSpawnPayload payload)
        {
            
            log.Debug("[ClientTeamRuntime] InitTeamRuntime.");
            
            base.InitTeamRuntime(payload);
            
            //construct squad
            ConstructSquads();
        }
        
        
        protected override void ConstructSquads()
        {
            log.Debug("[ClientTeamRuntime] ConstructSquads.");
        }

        public void RegisterSquadRuntime(ClientSquadRuntime squadRuntime, SquadSpawnPayload payload)
        {
            if (squadRuntime == null)
            {
                return;
            }
            
            _squadRuntimesByNetId[squadRuntime.NetId] = squadRuntime;
            _squadRuntimesBySquadId[payload.SquadId] = squadRuntime;
            
            log.Debug($"[ClientTeamRuntime] Registered Squad NetId:{squadRuntime.NetId} SquadId:{payload.SquadId}");
        }
        
    }
}
