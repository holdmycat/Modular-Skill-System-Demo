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
    }
    
    [CreateAssetMenu(menuName = "Ebonor/Config/Game Scene Config", fileName = "GameSceneConfig")]
    public class GameSceneConfig : ScriptableObject
    {
        [Tooltip("Registered scenario ids (normalized to lower-case, underscores).")]
        public List<GameSceneResource> ListSceneRes = new List<GameSceneResource>();
    }
}
