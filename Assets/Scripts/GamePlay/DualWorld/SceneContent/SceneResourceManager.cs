using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class SceneResourceManager : ISceneResourceManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SceneResourceManager));
        private CommanderBootstrapInfo _commander;
        private CommanderBootstrapInfo _playerCommander;
        private readonly string _sceneId;
        private readonly GlobalGameConfig _globalGameConfig;
        private readonly GameSceneResource _gameSceneResource;
        public SceneResourceManager(string sceneId /* 其他依赖 */, GlobalGameConfig config, IScenarioIdRegistry idRegistry) {
            _sceneId = sceneId;
            _globalGameConfig = config;

            var list = _globalGameConfig?.GameSceneConfig?.ListSceneRes;
            if (list == null)
            {
                log.Error("[SceneResourceManager] Construct failed: GameSceneConfig.ListSceneRes is null.");
                throw new System.InvalidOperationException("[SceneResourceManager] No scene resources configured.");
            }
            
            var targetSceneId = idRegistry.Normalize(_sceneId);
            if (string.IsNullOrEmpty(targetSceneId))
            {
                log.Error("[SceneResourceManager] Construct failed: targetSceneId is empty.");
                throw new System.InvalidOperationException("[SceneResourceManager] SceneId not provided or not registered.");
            }
            var isFound = false;
            
            foreach (var variable in list)
            {
                var tmp = variable?.sceneId;
                
                var normalized = idRegistry.Normalize(tmp);

                if (!string.IsNullOrEmpty(normalized) && normalized.Equals(targetSceneId))
                {
                    isFound = true;
                    _gameSceneResource = variable;
                    break;
                }
            }

            if (!isFound)
            {
                log.ErrorFormat("[SceneResourceManager] Construct, targetSceneId:{0} not found", targetSceneId);
                throw new System.InvalidOperationException($"[SceneResourceManager] SceneId '{targetSceneId}' not found in config.");
            }

            var legionConfig = _gameSceneResource.LegionConfig;
            
            _commander = new CommanderBootstrapInfo(legionConfig.Seed.Faction.ToString() + legionConfig.Seed.Slot, legionConfig.Seed.Faction, legionConfig);

            var playerLegionConfig = _globalGameConfig?.CommanderBirthConfigInst?.LegionConfig;
            
            if (playerLegionConfig != null)
            {
                _playerCommander = new CommanderBootstrapInfo(playerLegionConfig.Seed.Faction.ToString() + playerLegionConfig.Seed.Slot, playerLegionConfig.Seed.Faction, playerLegionConfig);
            }
            else
            {
                log.Error("[SceneResourceManager] Player commander config missing.");
            }
            
            log.Info($"[SceneResourceManager] Construct");
        }

        public GameSceneResource GetSceneResource()
        {
            return _gameSceneResource;
        }

        public CommanderBootstrapInfo GetCommanderBootstrapInfo()
        {
            return _commander;
        }

        public CommanderBootstrapInfo GetPlayerCommanderBootstrapInfo()
        {
            return _playerCommander;
        }
        
    }
}
