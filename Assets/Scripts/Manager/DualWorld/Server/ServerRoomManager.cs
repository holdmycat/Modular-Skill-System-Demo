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
        private readonly IPlayerDataProvider _playerDataProvider;
        private readonly Dictionary<FactionType, ServerPlayer> _players = new Dictionary<FactionType, ServerPlayer>();

        public ServerRoomManager(INetworkBus networkBus, IPlayerDataProvider playerDataProvider)
        {
            _networkBus = networkBus;
            _playerDataProvider = playerDataProvider;
            log.Debug("[ServerRoomManager] Constructed.");
        }
        
        public async UniTask InitAsync()
        {
            log.Info("[ServerRoomManager] InitAsync");

            foreach (var playerInfo in _playerDataProvider.GetPlayers())
            {
                if (_players.ContainsKey(playerInfo.FactionId))
                {
                    log.Warn($"[ServerRoomManager] Duplicate player faction {playerInfo.FactionId} ignored.");
                    continue;
                }

                var player = new ServerPlayer(playerInfo, _networkBus);
                _players.Add(playerInfo.FactionId, player);

                // Notify clients about the faction
                _networkBus.SendRpc(new RpcCreateFaction { FactionId = playerInfo.FactionId });

                // Bootstrap teams for this player
                player.InitializeTeams();
            }
        }

        public void Tick(int tick)
        {
            foreach (var player in _players.Values)
            {
                player.Tick(tick);
            }
        }

        public async UniTask ShutdownAsync()
        {
            log.Info("[ServerRoomManager] ShutdownAsync");
            
            foreach (var player in _players.Values)
            {
                await player.ShutdownAsync();
            }
            _players.Clear();
        }
    }
}
