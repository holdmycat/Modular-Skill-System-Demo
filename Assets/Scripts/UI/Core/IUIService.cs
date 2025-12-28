using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Ebonor.UI
{
    public interface IUIService
    {
        UniTask<T> OpenUIAsync<T>(Action<T> beforeOpen = null, IInstantiator canvasScope = null) where T : UIBase;
        UniTask CloseUIAsync<T>(bool destroy = false) where T : UIBase;
        UniTask CloseUIAsync(UIBase ui, bool destroy = false);
        T GetUI<T>() where T : UIBase;
    }
}
