using Cysharp.Threading.Tasks;

namespace Ebonor.Manager
{
    public interface IRoomManager
    {
        UniTask InitAsync();
        UniTask ShutdownAsync();

        void Tick(int tick);
    }
}
