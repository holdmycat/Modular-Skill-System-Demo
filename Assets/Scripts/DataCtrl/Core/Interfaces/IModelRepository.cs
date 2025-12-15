using System.Collections.Generic;
using UObject = UnityEngine.Object;
namespace Ebonor.DataCtrl
{
    public interface IModelRepository
    {
        void SaveModelAsync(IList<UnityEngine.Object> list);

        Dictionary<long, UObject> GetGameModelDic(eActorModelType type);
    }
}
