//------------------------------------------------------------
// File: ScenarioIdRegistry.cs
// Purpose: Centralized registry/normalizer for scenario ids to avoid typos.
//------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    public class ScenarioIdRegistry : IScenarioIdRegistry
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ScenarioIdRegistry));

        private readonly HashSet<string> _registered = new HashSet<string>();
        
        public ScenarioIdRegistry(GlobalGameConfig globalConfig)
        {
            if (globalConfig?.GameSceneConfig != null)
            {
                foreach (var raw in globalConfig.GameSceneConfig.ListSceneRes ?? Enumerable.Empty<GameSceneResource>())
                {
                    var normalized = Normalize(raw.sceneId);
                    if (string.IsNullOrEmpty(normalized)) continue;
                    _registered.Add(normalized);
                }
            }
            else
            {
                log.Warn("[ScenarioIdRegistry] GameSceneConfig missing; using default only.");
            }
        }

        public string Normalize(string scenarioId)
        {
            if (string.IsNullOrWhiteSpace(scenarioId))
            {
                log.Error("[ScenarioIdRegistry] Normalize, scenarioId is null or empty");
                return null;
            }
            
            var result = scenarioId.Trim().Replace(" ", "_").ToLower();

            if (!IsRegistered(result))
            {
                log.ErrorFormat("[ScenarioIdRegistry] Normalize, scenarioId:{0} not registered", result);
                return null;
            }


            return result;
        }

        public bool IsRegistered(string normalizedId)
        {
            if (string.IsNullOrEmpty(normalizedId)) return false;
            if (_registered.Count == 0) return true; // permissive when registry empty.
            bool exists = _registered.Contains(normalizedId);
            if (!exists)
            {
                log.Warn($"[ScenarioIdRegistry] Unregistered scenario id '{normalizedId}' encountered.");
            }
            return exists;
        }
    }
}
