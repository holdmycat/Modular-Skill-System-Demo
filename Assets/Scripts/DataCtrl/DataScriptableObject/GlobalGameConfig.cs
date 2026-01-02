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
        [Tooltip("Path for loading all slg commander data assets.")]
        public string allSlgCommanderDataPath = "AllCharacterData/SlgCommanderData";
        
        [Header("Character Data")]
        [Tooltip("Path for loading all slg unit data assets.")]
        public string allSlgUnitDataPath = "AllCharacterData/SlgUnitData";
        
        [Header("Default Player Id")]
        public long defaultPlayerHeroId;

        [Header("Default ResourcePoolConfig")]
        public ResourcePoolConfig ResourcePoolConfig;

        [Header("First Scene Name")]
        public string FirstSceneName;
        
        [Header("Scene Registry")]
        [Tooltip("Registered scenario ids and defaults.")]
        public GameSceneConfig GameSceneConfig;
        
        [Header("Default Player Commander Config")] 
        [SerializeField]
        public CommanderBirthConfig CommanderBirthConfigInst;


        [Header("Character Level up add percent")]
        [Range(-2f, 2f)]
        public float characterLevelupAddPercent;

        [Header("Debug Visuals")]
        [Tooltip("Master switch for all debug visuals. If disabled, sub-toggles (Commander, Squad, Soldier) will be ignored.")]
        public bool IsDebugVisualsEnabled = true;
        public bool ShowCommanderVisual = true;
        public bool ShowSquadVisual = true;
        public bool ShowSoldierVisual = true;

        [Header("SLG Squad Config")]
        [Tooltip("The dimensions (width, depth) of a single squad in world units.")]
        public Vector2 SlgSquadDimensions = new Vector2(10f, 10f);

        [Tooltip("The space between squads on the X axis.")]
        public float SlgSquadInterval = 5f;

        [Tooltip("Max number of soldiers per row in a squad.")]
        public int SlgSquadMaxRowSize = 5;

        [Tooltip("Interval between soldiers (X, Z).")]
        public Vector2 SlgSoldierInterval = new Vector2(1f, 1f);
    }
}
