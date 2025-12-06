//------------------------------------------------------------
// File: HeroAttributesNodeData.cs
// Created: 2025-12-01
// Purpose: Hero attribute node data definitions and editor utilities.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Ebonor.DataCtrl
{
    
    [System.Serializable]
    [BsonSerializer(typeof(AttributesDataSerializer<UnitAttributesNodeDataBase>))]
    public class HeroAttributesNodeData: UnitAttributesNodeDataBase
    {

        public  override void OnInit()
        {
            base.OnInit();
            DicPlayerStackAnimConfig ??= new Dictionary<eBuffBindAnimStackState, PlayAnimInfo>();
        }
        
        #region Hero Base Attributes


        
        [Tooltip("Pickup radius in meters.")]
        [BsonElement("PickupRadius")]
        public float PickupRadius;
        
        [Tooltip("Number of equip slots for skills.")]
        [BsonElement("EquipSkillNum")]
        public float EquipSkillNum;
        
        [Tooltip("Number of dash charges granted on spawn.")]
        [BsonElement("BirthDashNum")]
        public int BirthDashNum;
        
        [Tooltip("Luck multiplier.")]
        public float LuckyRate;
        
        [Tooltip("Harvest multiplier.")]
        public float HarvestRate;
        

        [Tooltip("Whether the hero is unlocked by default.")]
        public bool IsDefaultUnlock;

        
        #endregion
        
        #region Weapon Slot Attributes

        [Header("Weapon Slot Attributes")]
        [Tooltip("Maximum number of weapon slots.")]
        [BsonElement("MaxWeaponSpot")]
        public float MaxWeaponSpot;
        
        [Tooltip("Initial number of weapon slots when spawning.")]
        [BsonElement("BirthWeaponSpot")]
        public float BirthWeaponSpot;

      
        #endregion

      
        
        #region Stack Animation Config

        [Header("Stack Animation Config")]
        [Tooltip("Animation clips bound to buff stack states.")]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<eBuffBindAnimStackState, PlayAnimInfo> DicPlayerStackAnimConfig = new Dictionary<eBuffBindAnimStackState, PlayAnimInfo>();
        #endregion
    
        
        
    }
}
