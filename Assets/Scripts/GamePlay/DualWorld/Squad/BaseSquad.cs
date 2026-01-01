using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    
    public abstract class BaseSquad : SlgBattleEntity
    {
        
        protected SlgUnitSquadAttributesNodeData _squadUnitAttr;

        protected FactionType _faction;
        
        /// <summary>
        /// Bind net id and register to network bus. Call right after construction.
        /// </summary>
        public void Configure(uint netId, SlgUnitSquadAttributesNodeData squadUnitAttr, FactionType factionType, bool isServer = false)
        {
            if (NetId != 0)
            {
                return;
            }

            _squadUnitAttr = squadUnitAttr;
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
        
    }
}
