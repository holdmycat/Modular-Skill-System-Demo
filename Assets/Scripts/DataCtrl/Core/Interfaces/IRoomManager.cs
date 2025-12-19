using Cysharp.Threading.Tasks;

namespace Ebonor.DataCtrl
{
    public interface IRoomManager : INetworkBehaviour
    {
        UniTask InitAsync();
        UniTask ShutdownAsync();
        
        void Tick(int tick);
    }
}
