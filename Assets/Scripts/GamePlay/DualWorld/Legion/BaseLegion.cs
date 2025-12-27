using System.Collections.Generic;
using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    public abstract class BaseLegion : SlgBattleEntity
    {
        protected ulong _legionId;

        protected ICharacterDataRepository _characterDataRepository;

        protected FactionType _faction;
        
        protected List<long> _squadList;
        
        protected List<BaseSquad> _listBaseSquads;
        
        /// <summary>
        /// Bind net id and register to network bus. Call right after construction.
        /// </summary>
        public void Configure(uint netId, List<long> list, bool isServer = false)
        {
            if (NetId != 0)
            {
                return;
            }
            
            _dataLoaderService.NextId();
            
            BindId(netId);
            // LegionId and Faction should be set via DI context in partials
            _squadList = list;
            if (_networkBus == null)
            {
                throw new System.InvalidOperationException("[BaseLegion] Configure failed: network bus is null.");
            }
            _networkBus.RegisterSpawns(NetId, this, isServer);

           
        }
        
        protected override void InitializeNumeric()
        {
           
        }
    }
}
