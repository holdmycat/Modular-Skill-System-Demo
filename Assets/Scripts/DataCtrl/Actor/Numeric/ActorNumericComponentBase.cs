//------------------------------------------------------------
// File: ActorNumericComponent.cs
// Purpose: Base implementation storing numeric attribute metadata.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.DataCtrl
{

    //property operations
    public abstract partial class ActorNumericComponentBase : MonoBehaviour, IActorNumericComponent
    {
        protected StringBuilder mStrBld = new StringBuilder();
        
        protected Dictionary<int, float> mNumericDic;

        protected Dictionary<int, float> mOriNumericDic;
        
        public Dictionary<int, float> NumericDic => mNumericDic;

        public Dictionary<int, float> OriNumericDic => mOriNumericDic;
        
        public abstract float GetByKey(int key);
        
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
        /// UpdateNumeric_SetOperation: 处理更新数值操作
        /// </summary>
        /// <param name="numericType"></param>
        /// <param name="value"></param>
        /// <param name="isForceRefresh">是否可以强制刷新</param>
        /// <param name="isAllowAdjustBase">是否可以修改Base数值</param>
        private void UpdateNumeric_SetOperation(eNumericType numericType,float value, bool isForceRefresh = false, bool isAllowAdjustBase = false)
        {
            float cur = this.GetByKey((int)numericType);
            if (numericType < eNumericType.Min)
            {
                //是否强制刷新
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
                    
                //对外接口不能修改Base数据
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
        
        private void UpdateNumeric(eNumericType numericType)
        {
           
            int final = (int) numericType;
            float result = this.NumericDic[final];
            var finalType = numericType;
            //如果不是直接操作最终值，需要发送两次事件，一次是修改的值，一次是最终值
            if (numericType > eNumericType.Min)
            {
                final = (int) numericType / 10;
                int bas = final * 10 + 1;
                int add = final * 10 + 2;
                
                finalType = (eNumericType)final;
                
                //取得最终值，由基础xxx+额外xxx值组成
                float finalResult = this.GetByKey(bas) + this.GetByKey(add);
                //更新最终值
                NumericDic[(int)finalType] = finalResult;
                DataEventManager.OnValueChange(new UnitNumericChange(this, finalType, finalResult));

                if (ActorModelType == eActorModelType.eNpc && NpcProfession == eNpcProfession.Boss)
                {
                    DataEventManager.OnValueChange(new BossUnitNumericChange(this, finalType, finalResult));
                }
            }
            else
            {
                DataEventManager.OnValueChange(new UnitNumericChange(this, numericType, result));
                if (ActorModelType == eActorModelType.eNpc && NpcProfession == eNpcProfession.Boss)
                {
                    DataEventManager.OnValueChange(new BossUnitNumericChange(this, numericType, result));
                }
            }
            
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
        
        public eNpcProfession NpcProfession => mNpcProfession;
    }
    
    //system: init/uninit/reset
    public abstract partial class ActorNumericComponentBase : MonoBehaviour, IActorNumericComponent
    {
        
        static readonly ILog log = LogManager.GetLogger(typeof(ActorNumericComponentBase));
        
        #region public
        public void OnInitActorNumericComponent(CharacterRuntimeData characterRuntimeData, uint netid)
        {
            var unitAttr = DataCtrl.Inst.GetUnitAttributeNodeData(characterRuntimeData._numericId);
            
            var t = unitAttr.GetType();
            if (t != typeof(UnitAttributesNodeDataBase) 
                && !t.IsSubclassOf(typeof(UnitAttributesNodeDataBase)))
            {
                return;
            }
            
            InitNumericDictionaries<eNumericType>();
            
            OnInitCommonProperty(characterRuntimeData, netid, unitAttr);
            
            var dataBase = unitAttr as UnitAttributesNodeDataBase;
            
            //-----------------------Base Property-Begin-----------------------//
            SetValueForOrig(eNumericType.UnityProfession, (float)dataBase.HeroProfession);
            SetValueForOrig(eNumericType.UnitLv, 0);
            SetValueForOrig(eNumericType.UnitMaxLV, dataBase.UnitMaxLV);
            SetValueForOrig(eNumericType.ActorSide, (float)dataBase.ActorSide);
            SetValueForOrig(eNumericType.Height, dataBase.Height);
            SetValueForOrig(eNumericType.Radius, dataBase.Radius);
            int nSide = (int)dataBase.ActorSide;
            SetValueForOrig(eNumericType.ActorSide, (float)nSide);
            //-----------------------Base Property-End-----------------------//
            
        }
        
        public void OnUnInitActorNumericComponent()
        {
            
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
            
            // 1) 拿到所有枚举值的数组
            Array values = Enum.GetValues(typeof(TEnum));
            int count    = values.Length;

            //mNumericKeys = new List<int>();
            
            
            // 2) 预分配字典容量
            mOriNumericDic = new Dictionary<int, float>(count);
            mNumericDic    = new Dictionary<int, float>(count);

            // 3) 逐项转成 int key，赋 0f
            for (int i = 0; i < count; i++)
            {
                // values.GetValue(i) 返回 boxed enum 值
                int key = (int)values.GetValue(i)!;
                mOriNumericDic[key] = 0f;
                mNumericDic   [key] = 0f;
            }
        }
        
        
        #endregion
        
    }
}
