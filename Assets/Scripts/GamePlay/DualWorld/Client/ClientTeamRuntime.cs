using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ClientTeamRuntime : BaseTeamRuntime
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientTeamRuntime));

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
        
    }
}
