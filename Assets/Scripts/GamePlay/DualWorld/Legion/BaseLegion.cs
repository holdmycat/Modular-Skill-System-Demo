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

        /// <summary>
        /// Bind net id and register to network bus. Call right after construction.
        /// </summary>
        public void Configure(uint netId, ulong legionId, bool isServer = false)
        {
            if (NetId != 0)
            {
                return;
            }
            BindId(netId);
            _legionId = legionId;
            _networkBus?.RegisterSpawns(NetId, this, isServer);
        }
    }
}
