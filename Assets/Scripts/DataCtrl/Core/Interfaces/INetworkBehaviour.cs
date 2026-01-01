
using Cysharp.Threading.Tasks;

namespace Ebonor.DataCtrl
{
    public interface INetworkBehaviour
    {
        uint NetId { get; }
        
        void InitAsync();
       
        UniTask ShutdownAsync();
        
        void Tick(int tick);

        void BindId(uint netid);

        void OnRpc(IRpc rpc);

    }
}
