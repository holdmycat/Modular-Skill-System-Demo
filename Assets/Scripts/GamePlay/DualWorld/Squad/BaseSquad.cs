using Ebonor.DataCtrl;
using UnityEngine;
using Zenject;

namespace Ebonor.GamePlay
{
    
    public abstract class BaseSquad : SlgBattleEntity
    {
        
        protected SlgUnitSquadAttributesNodeData _squadUnitAttr;

        protected FactionType _faction;
        
        protected SlgUnitAttributesNodeData _unitAttr;

        protected NP_RuntimeTree _npRuntimeTree;
        
        /// <summary>
        /// Bind net id and register to network bus. Call right after construction.
        /// </summary>
        public void Configure(uint netId, SlgUnitSquadAttributesNodeData squadUnitAttr, SlgUnitAttributesNodeData unitAttr, FactionType factionType, bool isServer = false)
        {
            if (NetId != 0)
            {
                return;
            }

            _squadUnitAttr = squadUnitAttr;
            _unitAttr = unitAttr;
            // Faction is injected

            Faction = factionType;
            
            BindId(netId);
           
            InitializeNumeric();
            
            if (_networkBus == null)
            {
                throw new System.InvalidOperationException("[BaseLegion] Configure failed: network bus is null.");
            }
            _networkBus.RegisterSpawns(NetId, this, isServer);
        }

        [Inject] protected GlobalGameConfig _globalGameConfig;

        public CombatPositionType GetCombatPositionType()
        {
            if (_unitAttr != null)
            {
                return _unitAttr.CombatPositionType;
                
            }
            return CombatPositionType.Melee;
        }

        public int GetInitialCount()
        {
            return _squadUnitAttr != null ? _squadUnitAttr.InitialCount : 0;
        }

        public Vector3 GetSoldierLocalPosition(int index)
        {
             if (_globalGameConfig == null) return Vector3.zero;

             int maxRow = _globalGameConfig.SlgSquadMaxRowSize;
             if (maxRow <= 0) maxRow = 1;
             
             int totalCount = GetInitialCount();

             // Which row?
             int row = index / maxRow;
             // Which col in that row?
             int col = index % maxRow;
             
             // Calculate how many items are in this specific row
             // If this is a full row, it's maxRow.
             // If it's the last row, it's the remainder (or maxRow if exact multiple).
             // Math.Min(maxRow, RemainingItems)
             // Remaining items starting from this row's start index:
             int startIndex = row * maxRow;
             int countInRow = Mathf.Min(maxRow, totalCount - startIndex);
             
             // Center based on the actual count in this row
             // X = (col - (count - 1) / 2.0f) * IntervalX
             float xOffset = (col - (countInRow - 1) * 0.5f) * _globalGameConfig.SlgSoldierInterval.x;
             float zOffset = -row * _globalGameConfig.SlgSoldierInterval.y;

             return new Vector3(xOffset, 0, zOffset);
        }

       
        
    }
}
