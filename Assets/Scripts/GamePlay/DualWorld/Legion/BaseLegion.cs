using System.Collections;
using System.Collections.Generic;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Ebonor.GamePlay
{
    public class BaseLegion : NetworkBehaviour
    {
        protected INetworkBus _networkBus;
        
        protected  IDataLoaderService _dataLoaderService;
        protected ulong _legionId;

        protected ICharacterDataRepository _characterDataRepository;
        
        protected List<long> _squadList;
        
        /// <summary>
        /// Bind net id and register to network bus. Call right after construction.
        /// </summary>
        public void Configure(uint netId, ulong legionId, List<long> list, bool isServer = false)
        {
            if (NetId != 0)
            {
                return;
            }
            BindId(netId);
            _legionId = legionId;
            _squadList = list;
            if (_networkBus == null)
            {
                throw new System.InvalidOperationException("[BaseLegion] Configure failed: network bus is null.");
            }
            _networkBus.RegisterSpawns(NetId, this, isServer);
        }
    }
}
