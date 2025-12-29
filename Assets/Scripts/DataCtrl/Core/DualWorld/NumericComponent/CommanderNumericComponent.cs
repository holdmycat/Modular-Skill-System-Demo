using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.DataCtrl
{
    public class CommanderNumericComponent : BaseNumericComponent
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommanderNumericComponent));

        private CommanderContextData _commanderContext;
        
        private CommanderBootstrapInfo _commanderBootstrapInfo;

        private SlgCommanderAttributesNodeData _slgCommanderAttributesNodeData;

        private GlobalGameConfig _globalGameConfig;
        
        [Inject]
        public CommanderNumericComponent(uint netId)
        {
            _netId = netId;
        }
        
        [Inject]
        public void Construct(ICharacterDataRepository characterDataRepository, CommanderContextData commanderContextData, GlobalGameConfig globalGameConfig)
        {
            log.Info("[CommanderNumericComponent] Construct");
            
            _isServer = commanderContextData.IsServer;
            
            _commanderContext = commanderContextData;
            
            _characterDataRepository = characterDataRepository;
            
            _commanderBootstrapInfo = commanderContextData.BootstrapInfo;

            _globalGameConfig = globalGameConfig;

            _factionType = commanderContextData.Faction;
            
            Initialize();
        }
        
        protected override void OnInitialize(){
            
            log.Info("[CommanderNumericComponent] OnInitialize");

            _unitDataId = _commanderBootstrapInfo.LegionConfig.Seed.CommanderId;
            
            _slgCommanderAttributesNodeData = _characterDataRepository.GetSlgCommanderData(_unitDataId);

            _unitName = _slgCommanderAttributesNodeData.UnitName;
            
            _unitIcon= _slgCommanderAttributesNodeData.UnitIcon;
            
            SetValueForOrig(eNumericType.UnitLv, 0f);
            SetValueForOrig(eNumericType.UnitMaxLv, _slgCommanderAttributesNodeData.Level);
            
            SetValueForOrig(eNumericType.InfantryAttackMod, _slgCommanderAttributesNodeData.GlobalInfantryAtkMod);
            SetValueForOrig(eNumericType.MarchSpeed, _slgCommanderAttributesNodeData.GlobalMarchSpeedMod);
        }
        
        protected override void OnLevelUp()
        {

            var lv = GetLevel();

            var newValue = mOriNumericDic[(int)eNumericType.InfantryAttackModBase] * (1 + lv * _globalGameConfig.characterLevelupAddPercent);
            UpdateNumeric_SetOperation(eNumericType.InfantryAttackModBase, newValue, true, true);
            
            newValue = mOriNumericDic[(int)eNumericType.MarchSpeedBase] * (1 + lv * _globalGameConfig.characterLevelupAddPercent);
            UpdateNumeric_SetOperation(eNumericType.MarchSpeedBase, newValue, true, true);
        }
        
        
        public class Factory : PlaceholderFactory<uint, CommanderNumericComponent>
        {
            public Factory()
            {
                log.Info($"[CommanderNumericComponent.Factory] Construction");
            }
        }
    }
}
