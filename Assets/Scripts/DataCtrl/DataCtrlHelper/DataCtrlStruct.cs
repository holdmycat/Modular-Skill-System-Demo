//------------------------------------------------------------
// File: DataCtrlStruct.cs
// Created: 2025-12-01
// Purpose: Shared data-control enums and animation configuration structures.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    #region Enums
    
    public enum ePoolObjectType
    {
        eEffect = 0,
        eModel = 1,
        eFloatingText = 2,
        ePoolSize = 3,
        
    }
    
    
    /// <summary>
    /// Buff animation stack states and their priorities (higher bit = higher priority).
    /// </summary>
    [Flags]
    public enum eBuffBindAnimStackState
    {
        /// <summary>
        /// Death
        /// </summary>
        [InspectorName("Death")]
        Die = 1 << 20, // Highest priority

        [InspectorName("Victory")]
        Victory = 1 << 19, // Second highest priority
        
        /// <summary>
        /// Invincible
        /// </summary>
        [InspectorName("Invincible")]
        Invincible = 1 << 16, // High priority

        /// <summary>
        /// Waiting for buff execution; do nothing else. Highest priority.
        /// </summary>
        //WaitForBuff = 1<<15,
        
        // [InspectorName("Teleport")]
        // Teleport = 1<<14, // Directly teleport the target to a position

        [InspectorName("Frozen")]
        Frozen = 1 << 13, // Frozen
        
        /// <summary>
        /// Knock up
        /// </summary>
        [InspectorName("Knock Up")]
        KnockUp = 1 << 12, // High priority

        /// <summary>
        /// Repulse
        /// </summary>
        [InspectorName("Repulse")]
        Repulse = 1 << 11, // Medium priority

        // [InspectorName("Drag")]
        // Drag = 1<<10, // Drag target along a specific trajectory
        
        [InspectorName("Pull")]
        Pull = 1 << 9, // Pull target to caster position
        
        /// <summary>
        /// Stun
        /// </summary>
        [InspectorName("Stunned")]
        Stunned = 1 << 8, // Medium priority

        /// <summary>
        /// Hurt
        /// </summary>
        [InspectorName("Hurt")]
        GetHurt = 1 << 7, // Medium priority

        /// <summary>
        /// Disarm
        /// </summary>
        [InspectorName("Disarmed")]
        Disarmed = 1 << 6, // Medium priority
        
        // [InspectorName("Request Cast Skill")]
        // RequestCastSkill = 1<<5,  // Low priority
        
        /// <summary>
        /// Cast skill
        /// </summary>
        [InspectorName("Cast Skill")]
        CastSkill = 1 << 4, // Low priority

        /// <summary>
        /// Normal attack
        /// </summary>
        [InspectorName("Normal Attack")]
        NormalAttack = 1 << 3, // Low priority

        /// <summary>
        /// Move
        /// </summary>
        [InspectorName("Move")]
        Chasing = 1 << 2, // Low priority

        /// <summary>
        /// Idle: can move and play movement animation
        /// </summary>
        [InspectorName("Idle")]
        Idle = 1 << 1, // Low priority

        /// <summary>
        /// Birth
        /// </summary>
        [InspectorName("Birth")]
        Birth = 1 << 0, // Lowest priority
        
        NullStateID = 0,
    }
    
    /// <summary>
    /// Battle side identifiers.
    /// </summary>
    public enum eSide {
        Null = 0,
        /// <summary>Player side.</summary>
        Player = 1,
        /// <summary>Enemy side.</summary>
        Enemy = 2,
        /// <summary>Neutral side, cannot be attacked.</summary>
        Neutral = 3,
        /// <summary>Aggressive toward both player and enemy.</summary>
        Terrorist = 4,
    }
    
    /// <summary>
    /// Actor model categories used for runtime configuration and IDs.
    /// </summary>
    //[Flags]
    public enum eActorModelType: byte
    {
        [InspectorName("Unknown Model Type")]
        eNULL = 0,
        [InspectorName("Hero")]
        eHero = 1,
        [InspectorName("NPC")]
        eNpc = 2,
        [InspectorName("Skill Manager")]
        eSkillActor = 3,
        eSize = 4,
    }

    /// <summary>
    /// Hero professions (combinable flags).
    /// </summary>
    [Flags]
    public enum eHeroProfession
    {
        [InspectorName("Berserker")]
        Berserker = 1 << 0,

        [InspectorName("Duelist")]
        Duelist = 1 << 1,

        [InspectorName("Paladin")]
        Paladin = 1 << 2,
    
        [InspectorName("Assassin")]
        Assassin = 1 << 3,
    
        [InspectorName("Ranger")]
        Ranger = 1 << 4,
    
        [InspectorName("Blade Dancer")]
        BladeDancer = 1 << 5,

        [InspectorName("Elementalist")]
        Elementalist = 1 << 6,

        [InspectorName("Arcanist")]
        Arcanist = 1 << 7,

        [InspectorName("Necromancer")]
        Necromancer = 1 << 8,
        
        [InspectorName("Omni")]
        OMNI = 1 << 9,
    }

    
    [Flags]
    public enum eNpcProfession
    {
        EnemyNull = 1 << 0,
        
        Mow = 1 << 1,
        
        Elite = 1 << 2,
        
        Boss = 1 << 3,
        
        SummonDragon = 1 << 4,
        
        PlayerSummonNpc = 1 << 5,
    }
    
    /// <summary>
    /// Input control flag for enabling/disabling input groups.
    /// </summary>
    [Flags]
    public enum eInputControlFlag : byte
    {
        None = 0,
        Movement = 1 << 0,
        Skills = 1 << 1,
        Ui = 1 << 2,
        All = Movement | Skills | Ui
    }

    /// <summary>
    /// Asset category used by the resource loader to build paths/keys.
    /// </summary>
    public enum ResourceAssetType
    {
        /// <summary>ScriptableObjects, typically under Resources/ScriptableObject/</summary>
        ScriptableObject,
        /// <summary>UI prefabs under Resources/UI/</summary>
        UiPrefab,
        /// <summary>Hero model prefabs under Resources/Models/Hero/</summary>
        HeroModelPrefab,
        /// <summary>All character data under Resources/AllCharacterData/</summary>
        AllCharacterData,
        SceneStateManager,
    }
    
    /// <summary>
    /// Skill event hooks used to trigger behaviour tree logic.
    /// </summary>
    [Flags]
    public enum eSkillEventNode {
        [InspectorName("None")]
        ESkillEventNull = 0,
        
        [InspectorName("Channel Start")]
        ESkillEventChannel = 1,
        
        [InspectorName("Channel Hit")]
        ESkillEventChannelHit = 2,
        
        [InspectorName("Instant Hit")]
        ESkillEventHit = 4,
        
        [InspectorName("Animation Event")]
        ESkillAnimEvent = 8,
        
        [InspectorName("Animation Start")]
        ESkillAnimStart = 16,
        
        [InspectorName("Animation End")]
        ESkillAnimEnd = 32,
        
        [InspectorName("Size Sentinel")]
        Size = 64,
    }
    
    #endregion
    
    #region Data Classes

    /// <summary>
    /// Animation playback configuration for skills and actors.
    /// </summary>
    [BsonDeserializerRegister]
    public class PlayAnimInfo
    {
        public PlayAnimInfo()
        {
            AnimCurve = new AnimationCurve();
        }

        [Tooltip("Is this a passive weapon animation?")]
        public bool IsPassiveWeaponAnim;
        
        [Tooltip("Whether to play a baked animation.")]
        public bool IsPlayBakAnim;
        
        [Tooltip("Baked animation name to play.")]
        public string ClipBakeAnimName;
        
        [Tooltip("Transition asset name for the animation clip.")]
        public string ClipTransAssetName;

        [Tooltip("Force refresh animation data.")]
        public bool IsForceRefreshAnimData;
        
        [Tooltip("Normalized start time for the animation (0-1).")]
        [Range(0f, 1f)]
        public float AnimationStartNormalizedTime;
        
        [Tooltip("Use single collision data.")]
        public bool IsOnceCollideData;
        
        [Tooltip("Animation collision start time (normalized 0-1).")]
        [Range(0f, 1f)]
        public float AnimationCollideStartTime;
        
        [Tooltip("Animation collision end time (normalized 0-1).")]
        [Range(0f, 1f)]
        public float AnimationCollideEndTime;

        [Tooltip("Animation clip length (read-only).")]
        public float AnimationClipLength;

        [Tooltip("Configure animation event list.")]
        public bool IsRegisterAnimEventNode;
        
        [Tooltip("Animation event list.")]
        public List<System.Numerics.Vector3> ListAnimEvents;
        
        [Tooltip("Use animation event hit list.")]
        public bool IsUseAnimHitEvents;
        
        [Tooltip("Animation event hit list.")]
        public List<System.Numerics.Vector2> ListAnimHitEvents;
        
        [HideInInspector]
        public float AnimationRunTimeClipLength;

        [HideInInspector] 
        public Vector3 RepulseDir;

        [HideInInspector] 
        public AnimationCurve AnimCurve;
        
        [HideInInspector] 
        public Vector3 DestinationPosition;
        [HideInInspector] 
        public Vector3 InitialPosition;
        
        [HideInInspector]
        public float RepulseDuration;
        [HideInInspector]
        public float RepulseDistance;

        [Tooltip("Animation end time (normalized 0-1).")]
        [Range(0f, 1f)]
        public float AnimationEndTime;
        
        [HideInInspector]
        public short StackLevel;
        
        /// <summary>
        /// Transition time.
        /// </summary>
        [Tooltip("Time to transition from other animations into this one.")]
        public float FadeOutTime;
    }
    
    #endregion
    
    #region Buff
    public class CSupportBuffInfo
    {
        
        // public eSkillEventNode SkillEventNode;
        // public int ParagraphIndex;
        // public NP_SupportSkillDataSupportor SupportSkillDataSupportor;
        // public SupportSkillNumericComponent SupportSkillNumeric;
        // public SkillNumericComponent ActiveSkillNumeric;
        // public CSupportBuffInfo(){}
        // public CSupportBuffInfo(eSkillEventNode node, int index, SkillNumericComponent active,  NP_SupportSkillDataSupportor supportor, SupportSkillNumericComponent numeric)
        // {
        //     OnInitSupportBuffInfo(node, index, active, supportor, numeric);
        // }
        //
        // public void OnInitSupportBuffInfo(eSkillEventNode node, int index, SkillNumericComponent active,  NP_SupportSkillDataSupportor supportor, SupportSkillNumericComponent numeric)
        // {
        //     SkillEventNode = node;
        //     ParagraphIndex = index;
        //     SupportSkillDataSupportor = supportor;
        //     SupportSkillNumeric = numeric;
        //     ActiveSkillNumeric = active;
        // }
        
    }
    
    public class CDynamicBuffMgr
    {

        private eSkillEventNode mSkillEventNode;
        public eSkillEventNode SkillEventNodeInst => mSkillEventNode;
        
        private DynamicParallel mDynamicParallelInst;
        public DynamicParallel DynamicParallelInst => mDynamicParallelInst;
        
        // [Obsolete]
        // private Dictionary<int, List<VTD_BuffInfo>> mDicBuffInfo;
        // [Obsolete]
        // public Dictionary<int, List<VTD_BuffInfo>> DicBuffInfo => mDicBuffInfo;
        
        private Dictionary<int, List<CSupportBuffInfo>> mDicSupportSkillId;
        
        public Dictionary<int, List<CSupportBuffInfo>> DicSupportSkillId => mDicSupportSkillId;
        
        
        public CDynamicBuffMgr(DynamicParallel parallel, eSkillEventNode eventNode)
        {
            mDynamicParallelInst = parallel;

            mSkillEventNode = eventNode;
            
            //mDicBuffInfo = new Dictionary<int, List<VTD_BuffInfo>>();

            mDicSupportSkillId = new Dictionary<int, List<CSupportBuffInfo>>();
        }
    }
    
    // public class CDynamicSupportSkillNumeric
    // {
    //     public Dictionary<int, List<SupportSkillNumericComponent>> DicParaGraphSupportSkill;
    //     
    //     
    //     
    //     private eSkillEventNode mSkillEventNode;
    //     public eSkillEventNode SkillEventNodeInst => mSkillEventNode;
    //
    //     private int mMaxParagrapNum;
    //     
    //     public int MaxParagrapNum => mMaxParagrapNum;
    //     
    //     private int mMaxSupportNum;
    //     
    //     public int MaxSupportNum => mMaxSupportNum;
    //     
    //     public CDynamicSupportSkillNumeric(eSkillEventNode eventNode, int maxParagrapNum, int maxSupportNum)
    //     {
    //         mSkillEventNode = eventNode;
    //
    //         mMaxParagrapNum = maxParagrapNum;
    //
    //         mMaxSupportNum = maxSupportNum;
    //         
    //         DicParaGraphSupportSkill = new Dictionary<int, List<SupportSkillNumericComponent>>();
    //     }
    // }

    #endregion
    
    #region Character Numeric Type

    
    public struct CharacterRuntimeData
    {
        public long _numericId;//character type id
        public uint _netId;
        public bool _canChase;
        public CharacterRuntimeData(uint netId, long numericId, bool canChase = true)
        {
            _numericId = numericId;
            _canChase = canChase;
            _netId = netId;
        }
    }
    
    public struct UnitNumericChange
    {
        public ActorNumericComponentBase UnitAttrComp;
        public eNumericType NumericType;
        public float NumericResult;

        public UnitNumericChange(ActorNumericComponentBase comp, eNumericType type, float result)
        {
            UnitAttrComp = comp;
            NumericType = type;
            NumericResult = result;
        }
    }
    
    public struct BossUnitNumericChange
    {
        public ActorNumericComponentBase UnitAttrComp;
        public eNumericType NumericType;
        public float NumericResult;

        public BossUnitNumericChange(ActorNumericComponentBase comp, eNumericType type, float result)
        {
            UnitAttrComp = comp;
            NumericType = type;
            NumericResult = result;
        }
    }
    
    #endregion
    
    #region Pool Manager
    [Serializable]
    public class ResourcePoolConfig
    {
        public int MaxSimultaneousEffects;

        public int MaxSimultaneousSounds;

        public int MaxFloatingTextCount;
    }
    
    #endregion
    
}
