using Cysharp.Threading.Tasks;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Provides NP_DataSupportor for a given tree id/type (data loading/cache to be implemented elsewhere).
    /// </summary>
    public interface INPRuntimeTreeDataProvider
    {
        NP_DataSupportor GetData(long rootId, RuntimeTreeType treeType);

        UniTask InitializeAsync();
    }
}
