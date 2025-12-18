using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.Manager
{
    public class ServerRoomManager : IRoomManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerRoomManager));
        
        private readonly INetworkBus _networkBus;
        private readonly Dictionary<int, Ebonor.GamePlay.ServerFaction> _factions = new Dictionary<int, Ebonor.GamePlay.ServerFaction>();

        public ServerRoomManager(INetworkBus networkBus)
        {
            _networkBus = networkBus;
            log.Debug("[ServerRoomManager] Constructed.");
        }
        
        public async UniTask InitAsync()
        {
            log.Info("[ServerRoomManager] InitAsync");
            
            // 1. Create Logical Factions
            CreateFactionInternal(1);
            CreateFactionInternal(2);
            
            // 2. Notify Clients (Drive the View)
            // In a real server, we might wait for clients to connect, but here they are local.
            _networkBus.SendRpc(new RpcCreateFaction { FactionId = 1 });
            _networkBus.SendRpc(new RpcCreateFaction { FactionId = 2 });
        }

        private void CreateFactionInternal(int factionId)
        {
             _factions.Add(factionId, new Ebonor.GamePlay.ServerFaction(factionId, _networkBus));
             
             // Setup initial state (e.g. create a team immediately for testing?)
             if(factionId == 1)
             {
                 _factions[factionId].CreateTeam(101);
             }
        }

        public void Tick(int tick)
        {
            foreach (var faction in _factions.Values)
            {
                faction.Tick(tick);
            }
        }

        public async UniTask ShutdownAsync()
        {
            log.Info("[ServerRoomManager] ShutdownAsync");
            _factions.Clear();
        }
    }
}
