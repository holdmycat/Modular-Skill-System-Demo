//------------------------------------------------------------
// File: GameSceneConfig.cs
// Purpose: Scene-level configuration placeholder (extend as needed).
//------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace Ebonor.DataCtrl
{

    [System.Serializable]
    public class GameSceneResource
    {
        [Header("Scene Id")]
        public string sceneId;
        
        [Header("Legion Config")]
        public LegionConfigDefinition LegionConfig = new LegionConfigDefinition();

        public string UIName;
        
        public bool IsDebugUIName;

        [Header("Spawn Settings")]
        [Tooltip("The world position where the Commander (and thus the Legion) will spawn.")]
        public List<FactionSpawnPoint> FactionSpawnPoints;
    }

    [System.Serializable]
    public struct FactionSpawnPoint
    {
        public FactionType Faction;
        public Vector3 SpawnPosition;
        public Vector3 SpawnRotation;
    }
    
    [CreateAssetMenu(menuName = "Ebonor/Config/Game Scene Config", fileName = "GameSceneConfig")]
    public class GameSceneConfig : ScriptableObject
    {
        [Tooltip("Registered scenario ids (normalized to lower-case, underscores).")]
        public List<GameSceneResource> ListSceneRes = new List<GameSceneResource>();
    }
}
