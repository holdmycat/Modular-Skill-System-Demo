using System.Collections.Generic;
using Cysharp.Threading.Tasks;
namespace Ebonor.DataCtrl
{
    public interface IModelRepository
    {
        UniTask SaveModelAsync(IList<UnityEngine.Object> list);
    }
}
