
using Cysharp.Threading.Tasks;

namespace Ebonor.DataCtrl
{
    public interface INetworkBehaviour
    {
        uint NetId { get; }
        
        void InitAsync();
        void InitFromSpawnPayload(byte[] payload); // 新增：从 Spawn 数据初始化
        
        UniTask ShutdownAsync();
        
        void Tick(int tick);

        void BindId(uint netid);

    }
}
