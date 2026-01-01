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
            
            // Set Soldier Counts
            var initialCount = _slgSquadAttributesNodeData.InitialCount;
            SetValueForOrig(eNumericType.SoldierCount, initialCount);
            SetValueForOrig(eNumericType.SoldierMaxCount, _slgSquadAttributesNodeData.MaxCount);
            
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
        }
        
        protected override void OnLevelUp(){
            
        }
        
        
        public class Factory : PlaceholderFactory<uint, SlgUnitSquadAttributesNodeData, SquadNumericComponent>
        {
        }
    }
}
