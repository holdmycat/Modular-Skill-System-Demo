//------------------------------------------------------------
// File: GlobalGameConfig.cs
// Purpose: Global game configuration (loading mode, rules).
//------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    [CreateAssetMenu(menuName = "Ebonor/Config/Global Game Config", fileName = "GlobalGameConfig")]
    public class GlobalGameConfig : ScriptableObject
    {
        [Header("Resource Loading")]
        public ResourceLoadMode loadMode = ResourceLoadMode.Resources;

        [Header("Input")]
        [Tooltip("Path used by ResourceLoader to load the PlayerInput prefab.")]
        public string playerInputPrefabPath = "UI/PlayerInput";

        [Header("Character Data")]
        [Tooltip("Path for loading all character data assets.")]
        public string allCharacterDataPath = "AllCharacterData/HeroData";
        
        [Header("Character Data")]
        [Tooltip("Path for loading all slg squad data assets.")]
        public string allSlgSquadDataPath = "AllCharacterData/SlgSquadData";
        
        [Header("Character Data")]
        [Tooltip("Path for loading all slg unit data assets.")]
        public string allSlgUnitDataPath = "AllCharacterData/SlgUnitData";
        
        [Header("Default Player Id")]
        public long defaultPlayerHeroId;

        [Header("Default ResourcePoolConfig")]
        public ResourcePoolConfig ResourcePoolConfig;

        [Header("First Scene Name")]
        public string FirstSceneName;
        
        [Header("Default Player Team Config")] 
        [SerializeField]
        public PlayerBirthTeamConfig PlayerBirthTeamConfigInst;
    }
}
