using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ServerFaction : FactionBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerFaction));
        
        private readonly INetworkBus _networkBus;
        private readonly ITeamIdGenerator _teamIdGenerator;
        private readonly TeamIdComponents _teamIdComponents;
        private readonly long _teamId;
        
        public ServerFaction(FactionType factionId, INetworkBus networkBus, ITeamIdGenerator teamIdGenerator, TeamIdComponents teamIdComponents) : base(factionId)
        {
            _networkBus = networkBus;
            _teamIdGenerator = teamIdGenerator;
            _teamIdComponents = teamIdComponents;
            _teamId = _teamIdGenerator.GenerateTeamId(teamIdComponents);
        }
        
        public void CreateTeam()
        {
            log.Info($"[ServerFaction] {FactionId} Creating Team {_teamId}");
            // var team = new ServerTeam(_teamId);
            // Teams.Add(team);
            
            // DRIVE THE CLIENT
            _networkBus.SendRpc(new RpcCreateTeam 
            { 
                FactionId = FactionId, 
                TeamId = (int)_teamId 
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
