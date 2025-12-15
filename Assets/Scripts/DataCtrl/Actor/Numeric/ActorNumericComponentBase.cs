//------------------------------------------------------------
// File: ActorNumericComponent.cs
// Purpose: Base implementation storing numeric attribute metadata.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.DataCtrl
{

    //property operations
    public abstract partial class ActorNumericComponentBase : MonoBehaviour, IActorNumericComponent
    {

        [Inject]
        private IDataEventBus _dataEventBus;

        protected StringBuilder mStrBld;
        
        protected Dictionary<int, float> mNumericDic;

        protected Dictionary<int, float> mOriNumericDic;

        protected List<eNumericType> _displayNumericTypes;
        public IReadOnlyList<eNumericType> DisplayNumericTypes => _displayNumericTypes;
        
        protected Dictionary<int, float> NumericDic => mNumericDic;

        protected Dictionary<int, float> OriNumericDic => mOriNumericDic;
        
        private float GetByKey(int key)
        {
            NumericDic.TryGetValue(key, out float value);
            
            return value;
        }
        
        protected void SetValueForOrig(eNumericType type, float value)
        {
            eNumericType baseType =(eNumericType)((int)type * 10 + 1);
            eNumericType addType =(eNumericType)((int)type * 10 + 2);
            if (Enum.IsDefined(typeof(eNumericType), baseType) && Enum.IsDefined(typeof(eNumericType), addType))
            {
                SetValueForOrigPrivate(baseType, value);
                SetValueForOrigPrivate(addType, 0);
            }
            SetValueForOrigPrivate(type, value);
            
            
            void SetValueForOrigPrivate(eNumericType numericType, float value)
            {
                OriNumericDic[(int) numericType] = value;
                NumericDic[(int) numericType] = value;
            }
        }
        
        /// <summary>
        /// UpdateNumeric_SetOperation: handle numeric value updates.
        /// </summary>
        /// <param name="numericType"></param>
        /// <param name="value"></param>
        /// <param name="isForceRefresh">Allow forcing a refresh even when the value is unchanged.</param>
        /// <param name="isAllowAdjustBase">Allow adjusting the Base numeric value directly.</param>
        private void UpdateNumeric_SetOperation(eNumericType numericType,float value, bool isForceRefresh = false, bool isAllowAdjustBase = false)
        {
            float cur = this.GetByKey((int)numericType);
            if (numericType < eNumericType.Min)
            {
                // Skip if the value is unchanged unless a forced refresh is requested.
                if (Math.Abs(cur - value) <= 0.00001f && !isForceRefresh)
                {
                    return;
                }
                
                float baseValue = this.GetByKey((int)numericType * 10 + 1);
                    
                NumericDic[(int) numericType] = value;
                NumericDic[(int)numericType * 10 + 2] = value - baseValue;
                UpdateNumeric(numericType);
                

            }
            else if (numericType > eNumericType.Min) // base / add
            {
                int final = (int) numericType / 10;
                int bas = final * 10 + 1;
                int add = final * 10 + 2;
                    
                // External callers should not modify Base values directly unless explicitly allowed.
                if ((int)numericType == bas && !isAllowAdjustBase)
                {
                    log.ErrorFormat("You should only modify cur and  add values directly. this time you want to change {0}", numericType);
                    return;
                }
                
                NumericDic[(int) numericType] = value;
                
                if ((int)numericType == add)
                {
                    NumericDic[final] = value + NumericDic[bas];
                }
                else
                {
                    NumericDic[final] = value + NumericDic[add];
                }
                
                UpdateNumeric(numericType);
                    
            }
        }

        protected void SetDisplayNumericTypes(IEnumerable<eNumericType> numericTypes)
        {
           
            _displayNumericTypes.Clear();
            if (numericTypes == null) return;
            foreach (var type in numericTypes)
            {
                RegisterDisplayNumericType(type);
            }
        }

        protected void RegisterDisplayNumericType(eNumericType numericType)
        {
            if (!_displayNumericTypes.Contains(numericType))
            {
                _displayNumericTypes.Add(numericType);
            }
        }
        
        private void UpdateNumeric(eNumericType numericType)
        {
           
            int final = (int) numericType;
            float result = this.NumericDic[final];
            var finalType = numericType;
            // When modifying base/add values, emit change events for both the derived and final values.
            if (numericType > eNumericType.Min)
            {
                final = (int) numericType / 10;
                int bas = final * 10 + 1;
                int add = final * 10 + 2;
                
                finalType = (eNumericType)final;
                
                // The final value is built from base + additional values.
                float finalResult = this.GetByKey(bas) + this.GetByKey(add);
                // Update the cached final value before broadcasting.
                NumericDic[(int)finalType] = finalResult;
                _dataEventBus.OnValueChange(new UnitNumericChange(this, finalType, finalResult));

                if (ActorModelType == eActorModelType.eNpc && NpcProfession == eNpcProfession.Boss)
                {
                    _dataEventBus.OnValueChange(new BossUnitNumericChange(this, finalType, finalResult));
                }
            }
            else
            {
                _dataEventBus.OnValueChange(new UnitNumericChange(this, numericType, result));
                if (ActorModelType == eActorModelType.eNpc && NpcProfession == eNpcProfession.Boss)
                {
                    _dataEventBus.OnValueChange(new BossUnitNumericChange(this, numericType, result));
                }
            }
            
        }
        
        public virtual float this[eNumericType numericType]
        {
            get => GetByKey((int) numericType);
            set => UpdateNumeric_SetOperation(numericType, value, false, false);
        }
        
        
    }
    
    //basic property variables
    public abstract partial class ActorNumericComponentBase : MonoBehaviour, IActorNumericComponent
    {
        protected string attrName;
        protected string attrDesc;
        protected string attrIconName;
        protected string attrAvatarName;
        protected uint _netId;
        protected long _unitModelNodeId;
        protected eActorModelType _actorModelType;
        protected eNpcProfession mNpcProfession;
        
        public string AttrName => attrName;
        public string AttrDesc => attrDesc;
        public string AttrIconName => attrIconName;
        public string AttrAvatarName => attrAvatarName;
        public uint NetId => _netId;
        public eActorModelType ActorModelType => _actorModelType;
        public long UnitModelNodeId => _unitModelNodeId;
        public int GetUILevel()
        {
            return GetLevel() + 1;
        }
        public int GetLevel()
        {
            return (int)(this[eNumericType.UnitLv]);
        }
        public eNpcProfession NpcProfession => mNpcProfession;
    }
    
    //system: init/uninit/reset
    public abstract partial class ActorNumericComponentBase : MonoBehaviour, IActorNumericComponent
    {
        
        static readonly ILog log = LogManager.GetLogger(typeof(ActorNumericComponentBase));

        [Inject]
        private ICharacterDataRepository _characterDataRepository;
        
        [Inject] // Zenject automatically calls this after injection.
        public void PostInject() {
            mStrBld = new StringBuilder();
            _displayNumericTypes = new List<eNumericType>();
        }
        
        #region public
        public void OnInitActorNumericComponent(CharacterRuntimeData characterRuntimeData)
        {

            var unitAttr = _characterDataRepository.GetUnitAttribteData(characterRuntimeData._numericId);

            var netid = characterRuntimeData._netId;
            
            var t = unitAttr.GetType();
            if (t != typeof(UnitAttributesNodeDataBase) 
                && !t.IsSubclassOf(typeof(UnitAttributesNodeDataBase)))
            {
                return;
            }
            
            InitNumericDictionaries<eNumericType>();
            
            OnInitCommonProperty(characterRuntimeData, netid, unitAttr);
            
            //-----------------------Base Property-Begin-----------------------//
            SetValueForOrig(eNumericType.UnityProfession, (float)unitAttr.HeroProfession);
            SetValueForOrig(eNumericType.UnitLv, 0);
            SetValueForOrig(eNumericType.UnitMaxLV, unitAttr.UnitMaxLV);
            SetValueForOrig(eNumericType.ActorSide, (float)unitAttr.ActorSide);
            SetValueForOrig(eNumericType.Height, unitAttr.Height);
            SetValueForOrig(eNumericType.Radius, unitAttr.Radius);
            int nSide = (int)unitAttr.ActorSide;
            SetValueForOrig(eNumericType.ActorSide, (float)nSide);
            //-----------------------Base Property-End-----------------------//
            
            SetValueForOrig(eNumericType.Power, unitAttr.Power);
            SetValueForOrig(eNumericType.Agility, unitAttr.Agility);
            SetValueForOrig(eNumericType.Vitality, unitAttr.Vitality);
            
            SetValueForOrig(eNumericType.MovementSpeed, unitAttr.MovementSpeed);
            SetValueForOrig(eNumericType.RotationSpeed, unitAttr.RotationSpeed);
            
            SetValueForOrig(eNumericType.Life, unitAttr.Power);
            SetValueForOrig(eNumericType.MaxLife, unitAttr.Power);
            
            // UI display defaults
            if (_displayNumericTypes.Count == 0)
            {
                SetDisplayNumericTypes(new []
                {
                    eNumericType.Power,
                    eNumericType.Agility,
                    eNumericType.Vitality,
                    eNumericType.MovementSpeed,
                    eNumericType.RotationSpeed,
                    eNumericType.Life,
                    eNumericType.MaxLife
                });
            }
          
            
        }
        
        public void OnUnInitActorNumericComponent()
        {
            _displayNumericTypes.Clear();
        }

        public void OnResetActorNumericComponent()
        {
           
        }
        #endregion
        
        #region private 
        private void OnInitCommonProperty(CharacterRuntimeData characterRuntimeData, uint netid, UnitAttributesNodeDataBase unitAttr)
        {
            _netId = netid;

            _unitModelNodeId = characterRuntimeData._numericId;
            
            if (unitAttr == null)
            {
                // Default placeholders for missing data in edit-mode tests.
                attrAvatarName = string.Empty;
                attrName = $"Unit_{characterRuntimeData._numericId}";
                attrIconName = string.Empty;
                _actorModelType = eActorModelType.eHero;
                log.WarnFormat("[ActorNumericComponentBase] UnitAttributesNodeDataBase is null for numericId {0}; using defaults.", characterRuntimeData._numericId);
                return;
            }

            attrAvatarName = unitAttr.UnitAvatar;

            attrName = unitAttr.UnitName;

            attrIconName = unitAttr.UnitSprite;

            _actorModelType = unitAttr.ActorModelType;
        }
        
        private void InitNumericDictionaries<TEnum>() where TEnum : Enum
        {
            if (mOriNumericDic != null)
            {
                return;
            }
            
            // 1) Retrieve all enum values.
            Array values = Enum.GetValues(typeof(TEnum));
            int count    = values.Length;

            //mNumericKeys = new List<int>();
            
            
            // 2) Pre-allocate dictionary capacity.
            mOriNumericDic = new Dictionary<int, float>(count);
            mNumericDic    = new Dictionary<int, float>(count);

            // 3) Convert to int keys and seed with zero.
            for (int i = 0; i < count; i++)
            {
                // values.GetValue(i) returns a boxed enum value.
                int key = (int)values.GetValue(i)!;
                mOriNumericDic[key] = 0f;
                mNumericDic   [key] = 0f;
            }
        }
        
        
        #endregion
        
    }
}
