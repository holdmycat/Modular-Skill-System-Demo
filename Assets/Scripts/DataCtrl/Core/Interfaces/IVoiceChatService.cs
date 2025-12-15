using Cysharp.Threading.Tasks;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Optional voice chat module; can be swapped with real implementation or a no-op.
    /// </summary>
    public interface IVoiceChatService
    {
        UniTask InitializeAsync();
    }
}
