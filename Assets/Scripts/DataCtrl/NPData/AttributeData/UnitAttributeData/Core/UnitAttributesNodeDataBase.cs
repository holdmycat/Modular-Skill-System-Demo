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
    /// Unit基本属性数据（优化版）
    /// </summary>
    [BsonSerializer(typeof(AttributesDataSerializer<UnitAttributesNodeDataBase>))]
    public abstract class UnitAttributesNodeDataBase : ICommonAttributeBase
    {
        #region 构造和初始化
        
        public UnitAttributesNodeDataBase()
        {
            OnInit();
        }
        
        protected StringBuilder mStrBld = new StringBuilder();

        public virtual void OnInit()
        {
           
        }
        
        #endregion
        
        #region 工具属性
        
        [BsonElement("typeStr")]
        [HideInInspector]
        public string typeStr = "";

        #endregion
        
        #region ========== 主要配置区域 ==========
       
        // ===== Tab 1: 基础属性 =====
        [BsonElement("UnitDataNodeId")]
        public uint UnitDataNodeId;
        
        // [BsonElement("ActorModelType")]
        // public eActorModelType ActorModelType;
        //
        // [BsonElement("ActorSide")]
        // public eSide ActorSide;
        //
        // // 英雄专用
        // [BsonElement("HeroProfession")]
        // public eHeroProfession HeroProfession;
        //
        // public eHeroType HeroType;
        //
        // public eCoreAttributeType CharacterCoreAttributeType;
        //
        // // NPC专用
        // [BsonElement("NpcProfession")]
        // public eNpcProfession NpcProfession;
        //
        // 基本信息
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
        
        // 物理尺寸
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
        
        // ===== Tab 2: 核心属性 =====
        [BsonElement("Power")]
        public float Power;
        
        [BsonElement("Agility")]
        public float Agility;
        
        [BsonElement("Vitality")]
        public float Vitality;
        
        public float PowerGrowthRate;
        
        public float AgilityGrowthRate;
        
        
        public float VitalityGrowthRate;

        // ===== Tab 3: 战斗属性 =====
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

        
        // ===== Tab 5: 组件配置 =====
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

        // ===== Tab 6: 行为树配置 =====
        [BsonElement("HasBehaviourTree")]
        public bool HasBehaviourTree;
        
        [BsonElement("HasNormalAttackTree")]
        public bool HasNormalAttackTree;
        
        [BsonElement("HasSkillTree")]
        public bool HasSkillTree;
        
     
        
       

        // ===== Tab 7: 视觉特效 =====
        [BsonElement("ActorBirthEffectName")]
        public string ActorBirthEffectName;

        [BsonElement("ActorFootStepName")]
        public string ActorFootStepName;
        
      
        
        // 出生配置
        [BsonElement("BirthDuration")]
        public float BirthDuration;
        
        [BsonElement("HasBirthAnimation")]
        public bool HasBirthAnimation;
        
        [BsonElement("BirthCanbeLocked")]
        public bool BirthCanbeLocked;
        
        [BsonElement("BirthCanbeSeen")]
        public bool BirthCanbeSeen;
        
        // 死亡配置
        [BsonElement("DeathDelayDuration")]
        public float DeathDelayDuration;
        
        [BsonElement("DeathRepulseDistance")]
        public float DeathRepulseDistance;
        
        [BsonElement("DeathRepulseDuration")]
        public float DeathRepulseDuration;

       
        
        #endregion
    }
}
