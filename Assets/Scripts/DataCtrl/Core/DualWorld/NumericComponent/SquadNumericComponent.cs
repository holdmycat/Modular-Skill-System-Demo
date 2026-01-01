using Ebonor.Framework;
using Zenject;

namespace Ebonor.DataCtrl
{
    public class SquadNumericComponent : BaseNumericComponent
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SquadNumericComponent));
        
        private SlgUnitSquadAttributesNodeData _slgSquadAttributesNodeData;
        
        private SlgUnitAttributesNodeData _slgUnitAttributesNodeData;

        private long _squadId;
        private long _soldierId;
        
        private NumericDataModifier _comAttackMod;
        
        [Inject]
        public SquadNumericComponent(uint netId, SlgUnitSquadAttributesNodeData slgSquadAttributesNodeData)
        {
            log.Info("[SquadNumericComponent] Construction");
            _netId = netId;
            _slgSquadAttributesNodeData = slgSquadAttributesNodeData;
            _squadId = slgSquadAttributesNodeData.UnitDataNodeId;
            _soldierId = slgSquadAttributesNodeData.UnitId;
            
        }
        
        [Inject]
        public void Construct(
            ICharacterDataRepository characterDataRepository, 
            CommanderContextData commanderContextData, 
            GlobalGameConfig globalGameConfig)
        {
            log.Info("[SquadNumericComponent] Construct");
            
            _isServer = commanderContextData.IsServer;
            
            _commanderContext = commanderContextData;
            
            _characterDataRepository = characterDataRepository;
            
            _globalGameConfig = globalGameConfig;

            _factionType = commanderContextData.Faction;
            
            _slgUnitAttributesNodeData = _characterDataRepository.GetSlgUnitData(_soldierId);
            
            Initialize();
        }
        
  

        protected override void OnInitialize()
        {
            log.Info("[SquadNumericComponent] OnInitialize");
            
            _unitName = _slgSquadAttributesNodeData.UnitName;

            _unitIcon = _slgUnitAttributesNodeData.UnitSprite;
            
            // Set Soldier Counts
            var initialCount = _slgSquadAttributesNodeData.InitialCount;
            SetValueForOrig(eNumericType.SoldierCount, initialCount);
            SetValueForOrig(eNumericType.SoldierMaxCount, _slgSquadAttributesNodeData.MaxCount);
            
            // Subscribe to Commander Events
            if (_commanderContext.NumericComponent != null)
            {
                _commanderContext.NumericComponent.OnValueChanged += OnCommanderNumericChanged;
            }
            
            // Calculate Base Combat Stats based on Unit Stats * Count
            // Assuming _slgUnitAttributesNodeData is not null (guaranteed by logic flow or throws)
            if (_slgUnitAttributesNodeData != null)
            {
                 SetValueForOrig(eNumericType.Hp, _slgUnitAttributesNodeData.BaseHp * initialCount);
                 SetValueForOrig(eNumericType.Attack, _slgUnitAttributesNodeData.Attack * initialCount);
                 
                 // March Speed is usually not multiplied by count, but referencing NodeData default
                 // If NodeData doesn't have MarchSpeed, use default or from Commander Mod later
                 SetValueForOrig(eNumericType.MarchSpeed, _slgUnitAttributesNodeData.MoveSpeed); 
            }
            else
            {
                log.Error($"[SquadNumericComponent] Unit Data is Null for ID: {_soldierId}");
            }
            
            RecalculateStatsFromCommander();//squad initialization
        }
        
        protected override void OnLevelUp()
        {
            if (_slgUnitAttributesNodeData == null) return;
            
            var lv = GetLevel();
            var growthFactor = 1 + lv * _globalGameConfig.characterLevelupAddPercent;
            var initialCount = (int)this[eNumericType.SoldierCount];

            // Update Base Stats with Growth
            var newBaseHp = _slgUnitAttributesNodeData.BaseHp * initialCount * growthFactor;
            var newBaseAtk = _slgUnitAttributesNodeData.Attack * initialCount * growthFactor;
            
            SetValueForOrig(eNumericType.Hp, newBaseHp);
            SetValueForOrig(eNumericType.Attack, newBaseAtk);
            
            // Re-apply buffs on top of new base
            RecalculateStatsFromCommander();//squad level up
        }
        
        private void RecalculateStatsFromCommander()
        {
            var commander = _commanderContext.NumericComponent;
            if (commander == null) return;
            
            // Determine Unit Type (Infantry/Cavalry/Archer) to pick right mod
            // Assuming SlgUnitAttributesNodeData has UnitClassType
            var classType = _slgUnitAttributesNodeData.UnitClassType;
            
            float attackMod = 0f;
            float defenseMod = 0f;
            float hpMod = 0f;

            // Fetch Mod from Commander based on type
            switch (classType)
            {
                case UnitClassType.Infantry:
                    attackMod = commander[eNumericType.InfantryAttackMod];
                    // defenseMod = commander[eNumericType.InfantryDefenseMod];
                    // hpMod = commander[eNumericType.InfantryHpMod];
                    break;
                case UnitClassType.Cavalry:
                     // attackMod = commander[eNumericType.LancerAttackMod];
                     break;
                case UnitClassType.Archer:
                     // attackMod = commander[eNumericType.MarksmanAttackMod];
                     break;
            }
            
            // Apply Mod to Add Stats using Modifier System
            var atkFactor = attackMod / 10000f; 
            
            if (_comAttackMod != null)
            {
                // Update existing modifier
                _comAttackMod.UpdateValue(atkFactor);
                RecalculateModifiers(eNumericType.Attack);
            }
            else
            {
                // Create new modifier
                // Note: AddDataModifier automatically recalculates
                if (atkFactor > 0)
                {
                    _comAttackMod = (NumericDataModifier)AddDataModifier(ModifierType.Percentage, atkFactor, eNumericType.Attack);
                    _comAttackMod.TargetAttributeName = "CommanderBuff_Attack";
                }
            }
            
            // Repeat for HP/Def if needed
        }
        
        private void OnCommanderNumericChanged(eNumericType type, float value)
        {
            // Optimization can be added here to filter only relevant attributes
            RecalculateStatsFromCommander();//commander level up
        }

        public override void Dispose()
        {
            if (_commanderContext?.NumericComponent != null)
            {
                _commanderContext.NumericComponent.OnValueChanged -= OnCommanderNumericChanged;
            }
            base.Dispose();
        }
        
        
        public class Factory : PlaceholderFactory<uint, SlgUnitSquadAttributesNodeData, SquadNumericComponent>
        {
        }
    }
}
