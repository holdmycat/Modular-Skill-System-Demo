using Cysharp.Threading.Tasks;

namespace Ebonor.DataCtrl
{
    public interface ISystemDataService
    {
        /// <summary>
        /// Initializes low-level system data, such as BSON serialization mappings.
        /// </summary>
        UniTask InitializeAsync();
    }
}
