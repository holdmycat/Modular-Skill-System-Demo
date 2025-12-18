using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ServerFaction : FactionBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerFaction));
        
        private readonly INetworkBus _networkBus;
        //public List<ServerTeam> Teams { get; private set; } = new List<ServerTeam>();

        public ServerFaction(FactionType factionId, INetworkBus networkBus) : base(factionId)
        {
            _networkBus = networkBus;
        }
        
        public void CreateTeam(int teamId)
        {
            log.Info($"[ServerFaction] {FactionId} Creating Team {teamId}");
            // var team = new ServerTeam(teamId);
            // Teams.Add(team);
            
            // DRIVE THE CLIENT
            _networkBus.SendRpc(new RpcCreateTeam 
            { 
                FactionId = FactionId, 
                TeamId = teamId 
            });
        }

        public override void Tick(int tick)
        {
            // foreach (var team in Teams)
            // {
            //     team.Tick();
            // }
        }
        
    }
}
