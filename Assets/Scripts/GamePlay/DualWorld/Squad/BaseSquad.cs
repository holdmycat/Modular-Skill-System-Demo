using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    public class BaseSquad : NetworkBehaviour
    {
        protected INetworkBus _networkBus;
        
        protected  IDataLoaderService _dataLoaderService;
        
        protected ICharacterDataRepository _characterDataRepository;

        protected SlgUnitSquadAttributesNodeData _squadUnitAttr;

        protected FactionType _faction;
        
        /// <summary>
        /// Bind net id and register to network bus. Call right after construction.
        /// </summary>
        public void Configure(uint netId, SlgUnitSquadAttributesNodeData squadUnitAttr, bool isServer = false)
        {
            if (NetId != 0)
            {
                return;
            }

            _squadUnitAttr = squadUnitAttr;
            // Faction is injected
            
            BindId(netId);
           
            if (_networkBus == null)
            {
                throw new System.InvalidOperationException("[BaseLegion] Configure failed: network bus is null.");
            }
            _networkBus.RegisterSpawns(NetId, this, isServer);
        }
    }
}
