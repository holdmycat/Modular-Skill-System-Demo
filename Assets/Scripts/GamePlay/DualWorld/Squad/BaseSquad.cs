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

             // Which row?
             int row = index / maxRow;
             // Which col in that row?
             int col = index % maxRow;
             
             // Calculate row width for centering
             // Usually full rows have maxRow. Last row might have remainder.
             // But usually soldiers fill from front-center? Or front-left?
             // Assuming Standard Centered Grid.
             // We verify if this row is full?
             // Actually, a simple grid usually aligns columns.
             // If we want "Center, Left, Right" fill, that's different.
             // "Squad arrangement" had strict Center-Left-Right rules.
             // "Soldier arrangement" said "Each row based on MaxRowSize".
             // I will use standard grid centering.
             
             // Full width for X offset calculation?
             // If we want columns to align across rows, we should use fixed column positions.
             // e.g. Col 0 is Center? Or Col 0 is Left?
             // "MaxRowSize" implies width.
             // Let's assume standard alignment:
             // X = (col - (maxRow - 1) / 2.0f) * IntervalX
             // This centers the grid relative to Squad Center.
             
             float xOffset = (col - (maxRow - 1) * 0.5f) * _globalGameConfig.SlgSoldierInterval.x;
             float zOffset = -row * _globalGameConfig.SlgSoldierInterval.y; // Y in Vector2 is Z interval

             return new Vector3(xOffset, 0, zOffset);
        }

       
        
    }
}
