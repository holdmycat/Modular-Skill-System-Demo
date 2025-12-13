using System;
using Cysharp.Threading.Tasks;

namespace Ebonor.UI
{
    public interface IUIService
    {
        UniTask<T> OpenUIAsync<T>(Action<T> beforeOpen = null) where T : UIBase;
        UniTask CloseUIAsync<T>(bool destroy = false) where T : UIBase;
        UniTask CloseUIAsync(UIBase ui, bool destroy = false);
        T GetUI<T>() where T : UIBase;
    }
}
