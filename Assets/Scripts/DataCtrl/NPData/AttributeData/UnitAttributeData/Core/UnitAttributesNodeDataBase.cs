//------------------------------------------------------------
// File: UnitAttributesNodeDataBase.cs
// Created: 2025-11-29
// Purpose: Base class for unit attribute node data and shared metadata.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Ebonor.Framework;

using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace Ebonor.DataCtrl
{
   
    /// <summary>
    /// Unit base attribute data (optimized version)
    /// </summary>
    [System.Serializable]
    [BsonSerializer(typeof(AttributesDataSerializer<UnitAttributesNodeDataBase>))]
    public abstract class UnitAttributesNodeDataBase : ICommonAttributeBase
    {
        #region ConstructionAndInitialization
        
        public UnitAttributesNodeDataBase()
        {
            OnInit();
        }
        
        protected StringBuilder mStrBld = new StringBuilder();

        public virtual void OnInit()
        {
           
        }
        
        #endregion
        
        #region UtilityProperties
        
        [BsonElement("typeStr")]
        [HideInInspector]
        public string typeStr = "";

        #endregion
        
        #region ========== PrimaryConfigurationArea ==========
       
        // ===== Tab 1: Base Attributes =====
        [BsonElement("UnitDataNodeId")]
        public uint UnitDataNodeId;
        
        [BsonElement("ActorModelType")]
        public eActorModelType ActorModelType;
        
        [BsonElement("ActorSide")]
        public eSide ActorSide;
        //
        // // Hero only
        [BsonElement("HeroProfession")]
        public eHeroProfession HeroProfession;
        //
        // public eHeroType HeroType;
        //
        // public eCoreAttributeType CharacterCoreAttributeType;
        //
        // // NPC only
        // [BsonElement("NpcProfession")]
        // public eNpcProfession NpcProfession;
        //
        // Basic info
        [BsonElement("UnitName")]
        public string UnitName;

        [BsonElement("UnitSprite")]
        public string UnitSprite;
        
        [BsonElement("UnitAvatar")]
        public string UnitAvatar;
        
        [BsonElement("UnitMaxLV")]
        public uint UnitMaxLV;
        
        [BsonElement("ActiveSkillNum")]
        public uint ActiveSkillNum;
        
        [BsonElement("CanChase")]
        public bool CanChase;
        
        // Physical dimensions
        [BsonElement("Height")]
        public float Height;
        
        [BsonElement("Radius")]
        public float Radius;
        
        [BsonElement("Scale")]
        public float Scale;
        
        
        [BsonElement("SummonActorIndex")]
        public int SummonActorIndex;
        
        public int WeaponTypeIndex;
        
        [BsonElement("HeroActorIndex")]
        public int HeroActorIndex;
        
        [BsonElement("NpcActorIndex")]
        public int NpcActorIndex;
        
        // ===== Tab 2: Core Attributes =====
        [BsonElement("Power")]
        public float Power;
        
        [BsonElement("Agility")]
        public float Agility;
        
        [BsonElement("Vitality")]
        public float Vitality;
        
        public float PowerGrowthRate;
        
        public float AgilityGrowthRate;
        
        
        public float VitalityGrowthRate;

        // ===== Tab 3: Combat Attributes =====
        public float AttackDamge;
        
        public float LifeStealRate;
        
        [HideInInspector]
        public float BaseAttackTime = 1.7f;
        
        [Range(100f, 150f)]
        public float BaseAttackSpeed;
        
        [Range(0f, 200f)]
        public float MagicAttackDamageBonus;
        
        public float Armour;
        
        public float ElementsResistance;
        
        public float DodgeRate;

        [BsonElement("MovementSpeed")]
        public float MovementSpeed;

        [BsonElement("RotationSpeed")]
        public float RotationSpeed;

        
        // ===== Tab 5: Component Configuration =====
        [BsonElement("HostInstallNavMeshAgent")]
        public bool HostInstallNavMeshAgent;
        
        [BsonElement("HostPInstallCapsuleCollider")]
        public bool HostPInstallCapsuleCollider;
        
        [BsonElement("HostInstallRigidBody")]
        public bool HostInstallRigidBody;
        
        [BsonElement("CliLPInstallNavMeshAgent")]
        public bool CliLPInstallNavMeshAgent;
        
        [BsonElement("CliLPInstallCapsuleCollider")]
        public bool CliLPInstallCapsuleCollider;
        
        [BsonElement("CliLPInstallRigidBody")]
        public bool CliLPInstallRigidBody;
        
        [BsonElement("CliOnlyInstallNavMeshAgent")]
        public bool CliOnlyInstallNavMeshAgent;
        
        [BsonElement("CliOnlyInstallCapsuleCollider")]
        public bool CliOnlyInstallCapsuleCollider;
        
        [BsonElement("CliOnlyInstallRigidBody")]
        public bool CliOnlyInstallRigidBody;
        
        [BsonElement("ServerInstallNavMeshAgent")]
        public bool ServerInstallNavMeshAgent;
        
        [BsonElement("ServerInstallCapsuleCollider")]
        public bool ServerInstallCapsuleCollider;
        
        [BsonElement("ServerInstallRigidBody")]
        public bool ServerInstallRigidBody;

        // ===== Tab 6: Behavior Tree Configuration =====
        [BsonElement("HasBehaviourTree")]
        public bool HasBehaviourTree;
        
        [BsonElement("HasNormalAttackTree")]
        public bool HasNormalAttackTree;
        
        [BsonElement("HasSkillTree")]
        public bool HasSkillTree;
        
     
        
       

        // ===== Tab 7: Visual Effects =====
        [BsonElement("ActorBirthEffectName")]
        public string ActorBirthEffectName;

        [BsonElement("ActorFootStepName")]
        public string ActorFootStepName;
        
      
        
        // Spawn configuration
        [BsonElement("BirthDuration")]
        public float BirthDuration;
        
        [BsonElement("HasBirthAnimation")]
        public bool HasBirthAnimation;
        
        [BsonElement("BirthCanbeLocked")]
        public bool BirthCanbeLocked;
        
        [BsonElement("BirthCanbeSeen")]
        public bool BirthCanbeSeen;
        
        // Death configuration
        [BsonElement("DeathDelayDuration")]
        public float DeathDelayDuration;
        
        [BsonElement("DeathRepulseDistance")]
        public float DeathRepulseDistance;
        
        [BsonElement("DeathRepulseDuration")]
        public float DeathRepulseDuration;

       
        
        #endregion
    }
}
