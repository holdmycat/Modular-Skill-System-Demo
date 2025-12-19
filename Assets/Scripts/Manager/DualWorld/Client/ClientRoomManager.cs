using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.Manager
{
    public class ClientRoomManager : MonoBehaviour, IRoomManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientRoomManager));
        
        private INetworkBus _networkBus;
        private readonly Dictionary<FactionType, Ebonor.GamePlay.ClientFaction> _factions = new Dictionary<FactionType, Ebonor.GamePlay.ClientFaction>();

        [Inject]
        public void Construct(INetworkBus networkBus)
        {
            _networkBus = networkBus;
        }

        public async UniTask InitAsync()
        {
            log.Info("[ClientRoomManager] InitAsync - Listening for Server Events");
            // PASSIVE: Do NOT create factions yourself. Wait for Server.
            
            _networkBus.OnRpcReceived += OnRpcReceived;
            _networkBus.OnTickSync += Tick;
        }

        private void OnRpcReceived(IRpc rpc)
        {
            if (rpc is RpcCreateFaction createFactionPacket)
            {
                CreateFaction(createFactionPacket.FactionId);
            }
            else if (rpc is RpcCreateTeam createTeamPacket)
            {
                if (_factions.TryGetValue(createTeamPacket.FactionId, out var faction))
                {
                    faction.CreateTeam(createTeamPacket.TeamId);
                }
            }
        }

        private void CreateFaction(FactionType factionId)
        {
            // Now ClientFaction is a pure C# class that creates its own GameObject
            var faction = new Ebonor.GamePlay.ClientFaction(factionId, transform);
            _factions.Add(factionId, faction);
        }

        public void OnUpdate()
        {
            foreach (var faction in _factions.Values)
            {
                faction.OnUpdate();
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
            log.Info("[ClientRoomManager] ShutdownAsync");
            
            if (_networkBus != null)
                _networkBus.OnRpcReceived -= OnRpcReceived;

            foreach(var faction in _factions.Values)
            {
                faction.Destroy();
            }
            _factions.Clear();
        }
    }
}
