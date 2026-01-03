using System;
using System.Collections.Generic;
using System.Text;
using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    public abstract class BaseNumericComponent : INumericComponent
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BaseNumericComponent));

        private bool _isInit;

        protected ICharacterDataRepository _characterDataRepository;
        protected GlobalGameConfig _globalGameConfig;
        protected CommanderContextData _commanderContext;

        protected bool _isServer;
        public bool IsServer => _isServer;

        protected readonly StringBuilder mStrBld = new StringBuilder();

        protected Dictionary<int, float> mNumericDic;

        protected Dictionary<int, float> mOriNumericDic;

        protected Dictionary<int, List<ADataModifier>> _allModifiers = new Dictionary<int, List<ADataModifier>>();

        protected long _unitDataId;

        protected string _unitName;
        public string UnitName => _unitName;


        protected string _unitIcon;
        public string UnitIcon => _unitIcon;


        protected uint _netId;
        public uint NetId => _netId;

        protected FactionType _factionType;

        public FactionType FactionType => _factionType;

        //behavour tree id
        protected long _behaviorTreeId;
        public long BehavourTreeId => _behaviorTreeId;
        
        public int GetLevel()
        {
            return (int)this[eNumericType.UnitLv];
        }

        public int GetLevelForUI()
        {
            return GetLevel() + 1;
        }

        public void Initialize()
        {
            if (!_isInit)
            {
                InitNumericDictionaries<eNumericType>();

                OnInitialize();
                _isInit = true;
                return;
            }

            log.Error($"[BaseNumericComponent] Construction, _isInit twice");

            void InitNumericDictionaries<TEnum>() where TEnum : Enum
            {
                // 1) 拿到所有枚举值的数组
                Array values = Enum.GetValues(typeof(TEnum));
                int count = values.Length;

                // 2) 预分配字典容量
                mOriNumericDic = new Dictionary<int, float>(count);
                mNumericDic = new Dictionary<int, float>(count);

                // 3) 逐项转成 int key，赋 0f
                for (int i = 0; i < count; i++)
                {
                    // values.GetValue(i) 返回 boxed enum 值
                    int key = (int)values.GetValue(i)!;
                    mOriNumericDic[key] = 0f;
                    mNumericDic[key] = 0f;
                }
            }

        }

        public void LevelUp()
        {
            var lv = GetLevel();
            var newLv = lv + 1;
            if (lv + 1 > this[eNumericType.UnitMaxLv])
            {
                newLv = lv;
            }

            SetValueForOrig(eNumericType.UnitLv, newLv);

            OnLevelUp();
        }

        public void Reset()
        {
            SetValueForOrig(eNumericType.UnitLv, 0);
            OnLevelUp();
        }

        public virtual void Dispose()
        {
            mNumericDic?.Clear();
            mOriNumericDic?.Clear();

            foreach (var list in _allModifiers.Values)
            {
                foreach (var mod in list)
                {
                    mod.Clear();
                    ReferencePool.Release(mod as IReference);
                }

                list.Clear();
            }

            _allModifiers.Clear();

            mNumericDic = null;
            mOriNumericDic = null;
            _isInit = false;
        }

        public float this[eNumericType numericType]
        {
            get => GetByKey((int)numericType);
            set => UpdateNumeric_SetOperation(numericType, value, false, false);
        }

        protected abstract void OnInitialize();

        protected abstract void OnLevelUp();

        protected float GetByKey(int key)
        {
            mNumericDic.TryGetValue(key, out float value);

            return value;
        }

        protected void SetValueForOrig(eNumericType type, float value)
        {
            eNumericType baseType = (eNumericType)((int)type * 10 + 1);
            eNumericType addType = (eNumericType)((int)type * 10 + 2);
            if (Enum.IsDefined(typeof(eNumericType), baseType) && Enum.IsDefined(typeof(eNumericType), addType))
            {
                SetValueForOrigPrivate(baseType, value);
                SetValueForOrigPrivate(addType, 0);
            }

            SetValueForOrigPrivate(type, value);


            void SetValueForOrigPrivate(eNumericType numericType, float value)
            {
                mOriNumericDic[(int)numericType] = value;
                mNumericDic[(int)numericType] = value;
            }
        }

        protected void UpdateNumeric_SetOperation(eNumericType numericType, float value, bool isForceRefresh = false,
            bool isAllowAdjustBase = false)
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

                mNumericDic[(int)numericType] = value;
                mNumericDic[(int)numericType * 10 + 2] = value - baseValue;
                UpdateNumeric(numericType);

            }
            else if (numericType > eNumericType.Min) // base / add
            {
                int final = (int)numericType / 10;
                int bas = final * 10 + 1;
                int add = final * 10 + 2;

                //对外接口不能修改Base数据
                if ((int)numericType == bas && !isAllowAdjustBase)
                {
                    log.ErrorFormat(
                        "You should only modify cur and  add values directly. this time you want to change {0}",
                        numericType);
                    return;
                }

                mNumericDic[(int)numericType] = value;

                if ((int)numericType == add)
                {
                    mNumericDic[final] = value + mNumericDic[bas];
                }
                else
                {
                    mNumericDic[final] = value + mNumericDic[add];
                }

                UpdateNumeric(numericType);

            }
        }

        public event Action<eNumericType, float> OnValueChanged;

        private void UpdateNumeric(eNumericType numericType)
        {

            int final = (int)numericType;
            float result = this.mNumericDic[final];
            var finalType = numericType;

            // Invoke for the specific type changed
            OnValueChanged?.Invoke(numericType, result);

            //如果不是直接操作最终值，需要发送两次事件，一次是修改的值，一次是最终值
            if (numericType > eNumericType.Min)
            {
                final = (int)numericType / 10;
                int bas = final * 10 + 1;
                int add = final * 10 + 2;

                finalType = (eNumericType)final;

                //取得最终值，由基础xxx+额外xxx值组成
                float finalResult = this.GetByKey(bas) + this.GetByKey(add);
                //更新最终值
                mNumericDic[(int)finalType] = finalResult;

                //DataCtrl.Inst.OnValueChange(new UnitNumericChange(this, finalType, finalResult));
                OnValueChanged?.Invoke(finalType, finalResult);

            }
            else
            {
                //DataCtrl.Inst.OnValueChange(new UnitNumericChange(this, numericType, result));

            }

        }

        #region Modifiers

        public ADataModifier AddDataModifier(ModifierType modifierType, float value, eNumericType numericType)
        {
            var mod = NumericDataModifier.Create(modifierType, value);
            AddDataModifier(mod, numericType);
            return mod;
        }

        public void AddDataModifier(ADataModifier dataModifier, eNumericType numericType)
        {
            int key = (int)numericType;
            if (!_allModifiers.TryGetValue(key, out var list))
            {
                list = new List<ADataModifier>();
                _allModifiers[key] = list;
            }

            list.Add(dataModifier);

            // Recalculate if it's a valid property (Final type usually)
            if (numericType < eNumericType.Min) // Final
            {
                RecalculateModifiers(numericType);
            }
        }

        public void RemoveDataModifier(ADataModifier dataModifier, eNumericType numericType)
        {
            int key = (int)numericType;
            if (_allModifiers.TryGetValue(key, out var list))
            {
                if (list.Remove(dataModifier))
                {
                    ReferencePool.Release(dataModifier as IReference);
                    if (numericType < eNumericType.Min)
                    {
                        RecalculateModifiers(numericType);
                    }
                }
            }
        }

        protected void RecalculateModifiers(eNumericType numericType)
        {
            if (numericType >= eNumericType.Min) return; // Only calc for Final types

            int key = (int)numericType;

            // Base Type ID calculation
            int baseTypeId = (int)numericType * 10 + 1;
            float baseValue = GetByKey(baseTypeId);

            float finalValue = baseValue;

            if (_allModifiers.TryGetValue(key, out var list))
            {
                float constantSum = 0f;
                float percentageSum = 0f;

                foreach (var mod in list)
                {
                    if (mod.ModifierType == ModifierType.Constant)
                        constantSum += mod.GetModifierValue();
                    else if (mod.ModifierType == ModifierType.Percentage)
                        percentageSum += mod.GetModifierValue();
                }

                finalValue = (baseValue + constantSum) * (1 + percentageSum);
            }

            // Set Final Value (Triggers UpdateNumeric_SetOperation -> Updates Add)
            this[numericType] = finalValue;
        }

        #endregion
    }
}



