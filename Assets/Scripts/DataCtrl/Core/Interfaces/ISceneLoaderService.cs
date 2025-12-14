using System;
using Cysharp.Threading.Tasks;

namespace Ebonor.DataCtrl
{
    public interface ISceneLoaderService
    {
        UniTask LoadSceneAsync(string sceneName);
    }
}
