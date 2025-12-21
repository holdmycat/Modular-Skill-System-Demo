
using Cysharp.Threading.Tasks;

namespace Ebonor.DataCtrl
{
    public interface INetworkBehaviour
    {
        uint NetId { get; }
        
        UniTask InitAsync();
        UniTask ShutdownAsync();
        
        void Tick(int tick);

        void BindId(uint netid);

    }
}
