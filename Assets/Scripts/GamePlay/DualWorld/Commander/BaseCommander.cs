using System.Collections.Generic;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.GamePlay
{
    public abstract class BaseCommander : SlgBattleEntity
    {
        protected CommanderSeed _seed;
        protected CommanderBootstrapInfo _bootstrapInfo;
        protected static readonly ILog log = LogManager.GetLogger(typeof(BaseCommander));
        protected List<BaseSquad> _spawnedSquads = new List<BaseSquad>();
        
        [Inject] protected GlobalGameConfig _globalGameConfig;

        /// <summary>
        /// Inject commander bootstrap data and bind net id (call once after creation).
        /// </summary>
        public void Configure(CommanderBootstrapInfo bootstrapInfo,  bool isServer = true, uint tmpnetId = 0)
        {
            _bootstrapInfo = bootstrapInfo;

            Faction = bootstrapInfo.LegionConfig.Seed.Faction;
            
            // Set Position from Config
            if (_globalGameConfig != null && _globalGameConfig.GameSceneConfig != null)
            {
                var sceneName = bootstrapInfo.LegionConfig.Seed.ScenarioId;
                var sceneRes = _globalGameConfig.GameSceneConfig.ListSceneRes.Find(x => x.sceneId == sceneName);
                
                if (sceneRes != null && sceneRes.FactionSpawnPoints != null)
                {
                    foreach (var spawnPoint in sceneRes.FactionSpawnPoints)
                    {
                        if (spawnPoint.Faction == Faction)
                        {
                            Position = spawnPoint.SpawnPosition;
                            Rotation = Quaternion.Euler(spawnPoint.SpawnRotation);
                            log.Info($"[BaseCommander] Configured Spawn for Faction {Faction}: Pos {Position}, Rot {Rotation.eulerAngles}");
                            break;
                        }
                    }
                }
                else
                {
                   log.Warn($"[BaseCommander] Could not find SceneResource for {sceneName} or FactionSpawnPoints is empty.");
                }
            }
            
            // base.Configure(bootstrapInfo); // MOVED TO END
            
            // Manual set for local logic usage if needed, though we use param 'bootstrapInfo'
            _bootstrapInfo = bootstrapInfo; 
            
            if (_bootstrapInfo == null || _bootstrapInfo.LegionConfig == null)
            {
                log.Error("[ServerCommander] Configure failed: bootstrap info missing.");
                throw new System.InvalidOperationException("[ServerCommander] Configure failed: bootstrap info missing.");
            }

            if (_bootstrapInfo.LegionConfig.Seed == null)
            {
                log.Error("[ServerCommander] Configure failed: LegionConfig.Seed is null. This is a critical data error.");
                throw new System.InvalidOperationException("[ServerCommander] Configure failed: LegionConfig.Seed is null.");
            }

            uint netId = tmpnetId;
            if (isServer)
            {
                _seed = _bootstrapInfo.LegionConfig.Seed;
                netId = _dataLoaderService.NextId();
            }
            
            // Populate Context Data (Write Once)
            _contextData.SetContext(true, _bootstrapInfo);
            
            BindId(netId);
            
            // Now call base to trigger Numeric Init (which depends on Context)
            InitializeNumeric();
            
            _networkBus.RegisterSpawns(NetId, this, isServer);
            
        }

        public IReadOnlyList<BaseSquad> GetSpawnedSquads()
        {
            return _spawnedSquads;
        }

        protected void RecalculateSquadPositions()
        {
            if (_spawnedSquads == null || _spawnedSquads.Count == 0) return;
            if (_globalGameConfig == null) return;

            // Separate into Melee (0) and Ranged (1)
            var meleeSquads = new List<BaseSquad>();
            var rangedSquads = new List<BaseSquad>();

            foreach (var squad in _spawnedSquads)
            {
                var posType = squad.GetCombatPositionType();
                if (posType == CombatPositionType.Melee)
                    meleeSquads.Add(squad);
                else
                    rangedSquads.Add(squad);
            }

            LayoutSquads(meleeSquads, 0);
            LayoutSquads(rangedSquads, 1);
        }

        private void LayoutSquads(List<BaseSquad> squads, int rankIndex)
        {
            if (squads.Count == 0) return;

            var squadDim = _globalGameConfig.SlgSquadDimensions;
            // var squadInterval = _globalGameConfig.SlgSquadInterval; // Used below
            
            float zPos = -rankIndex * (squadDim.y + _globalGameConfig.SlgSquadInterval);

            for (int i = 0; i < squads.Count; i++)
            {
                int rowIndex = i / 3;
                int colIndex = i % 3; // 0, 1, 2

                float xOffset = 0;
                if (colIndex == 1) xOffset = -(squadDim.x + _globalGameConfig.SlgSquadInterval);
                else if (colIndex == 2) xOffset = (squadDim.x + _globalGameConfig.SlgSquadInterval);

                float finalZ = zPos - rowIndex * (squadDim.y + _globalGameConfig.SlgSquadInterval);

                var localPos = new Vector3(xOffset, 0, finalZ);
                
                // Set Logical Position
                squads[i].Position = this.Position + this.Rotation * localPos;
                squads[i].Rotation = this.Rotation; // Same rotation for now
            }
        }

       

        
    }
}
