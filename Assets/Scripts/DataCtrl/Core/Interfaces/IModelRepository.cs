using System.Collections.Generic;
namespace Ebonor.DataCtrl
{
    public interface IModelRepository
    {
        void SaveModelAsync(IList<UnityEngine.Object> list);
    }
}
