//------------------------------------------------------------
// File: NP_ClientResDataManager.cs
// Created: 2025-12-05
// Purpose: Client resource data manager for NP behaviour tree canvas assets.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Bson.Serialization;
using Ebonor.Framework;
using UnityEngine;
using UObject = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ebonor.DataCtrl
{
#if UNITY_EDITOR

    [System.Serializable]
    public class CLocalization
    {
        public string Key;
        public string Value;
    }
    
    [System.Serializable]
    public class LocalizationArrayWrapper
    {
        public CLocalization[] Localizations;
    }
    
    [System.Serializable]
    public class CLocalizationList
    {
        public CLocalization[] Localizations;
    }
    
    // TODO: Consider centralizing data here to separate edit-time logic and enable Excel/node-editor import/export.
    /// <summary>
    /// Canvas data manager for blackboard and common resource strings.
    /// </summary>
    public class NP_ClientResDataManager
    {
        [Header("Client Resources")]
        [Tooltip("Battle-related audio clip identifiers.")]
        public List<string> ListAllBattleAudio = new List<string>();
        
        [Tooltip("Common audio clip identifiers.")]
        public List<string> ListAllCommonAudio = new List<string>();
        
        [Tooltip("Effect asset identifiers.")]
        public List<string> ListAllEffects = new List<string>();
        
        [Tooltip("Animation clip identifiers for NT animations.")]
        public List<string> ListAllNTAnims = new List<string>();

        [Tooltip("Skill animation clip identifiers.")]
        public List<string> ListAllSkillAnims = new List<string>();
        
        [Tooltip("Baked skill animation identifiers.")]
        public List<string> ListAllBakeSkillAnims = new List<string>();
        
        [Tooltip("Shader asset identifiers.")]
        public List<string> ListAllShaders = new List<string>();

        [Tooltip("UI asset identifiers.")]
        public List<string> ListAllUIName = new List<string>();
        
#if UNITY_EDITOR
        /// <summary>
        /// Set when the GraphEditor Blackboard button is clicked.
        /// </summary>
        public static NP_ClientResDataManager CurrentEditedNP_ClientResDataManager;
        
        public void LoadAllClientRes()
        {
            //LoadAllBattleAudios();
            //LoadAllDropItemsFromAllDropAttributes();
            // LoadAllEffects();
            // LoadAllNTAnims();
            // LoadAllBakeAnims();
            // LoadAllShaders();
            // LoadAllSkillNames();
            // LoadAllSkillsByAllSkillAttributeGraph();
            // LoadAllBulletsByAllBulletAttributeGraph();
            // LoadAllSummonosByAllSummonoAttributeGraph();
            // LoadAllSkillDescription();
            // LoadAllSkillIcons();
            // LoadAllUIScenesPrefabs();
            // LoadAllWeaponNames();
            // LoadAllWeaponIds();
            // LoadAllWeaponSkillName();
            // LoadAllGameData();
            //DataCtrlHelper.LoadBuffDataMappping();
        }
        
       
        
        

#endif
    }
#endif
}
