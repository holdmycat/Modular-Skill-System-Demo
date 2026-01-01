// ---------------------------------------------
// Script: ClientRoomManager.cs
// Purpose: Client-side room coordinator that listens for RPC spawn/destroy messages and wires up local actors.
// ---------------------------------------------
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Ebonor.GamePlay
{

    //network rpc
    public  abstract class BaseRoomManager : MonoBehaviour, IRoomManager
    {
        protected NetworkIdHandle _netHandle;
        public uint NetId => _netHandle.NetId;
        public void BindId(uint netid) => _netHandle.BindId(netid);
        
        protected ICharacterDataRepository _characterDataRepository;
        
        public virtual void OnRpc(IRpc rpc)
        {
           
        }

        public virtual void InitAsync()
        {
           
        }
        

        public virtual async UniTask ShutdownAsync()
        {
            
        }

        public virtual void Tick(int tick)
        {
            
        }

        public virtual void OnUpdate()
        {
        }
       
    }
}
