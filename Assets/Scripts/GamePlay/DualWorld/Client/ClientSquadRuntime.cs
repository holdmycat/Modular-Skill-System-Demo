using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ClientSquadRuntime : BaseSquadRuntime
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientSquadRuntime));

        public ClientSquadRuntime(uint netId)
        {
            BindId(netId);
            log.Debug($"[ClientSquadRuntime] Constructed. NetId: {NetId}");
        }

        public override void InitSquadRuntime(SquadSpawnPayload payload)
        {
            base.InitSquadRuntime(payload);
            log.Debug($"[ClientSquadRuntime] InitSquadRuntime. SquadId: {_squadId}, TeamNetId: {_teamNetId}");
        }
    }
}
