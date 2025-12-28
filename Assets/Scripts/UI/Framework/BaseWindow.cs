using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.UI
{
    /// <summary>
    /// Base class for Level 1 UIs (Windows/Screens) that have a ViewModel.
    /// Managed by UIManager.
    /// </summary>
    /// <typeparam name="TViewModel">The specific ViewModel type.</typeparam>
    public abstract class BaseWindow<TViewModel> : UIBase where TViewModel : BaseViewModel
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BaseWindow<TViewModel>));
        
        protected TViewModel ViewModel { get; private set; }

        [Inject]
        public void Construct(TViewModel viewModel)
        {
            log.Info($"[{GetType().Name}] Construct.");
            ViewModel = viewModel;
        }

        protected override async UniTask OnOpenAsync()
        {
            log.Info($"[{GetType().Name}] OnOpenAsync.");
            if (ViewModel != null)
            {
                ViewModel.OnOpen();
            }
        }

        protected override async UniTask OnCloseAsync()
        {
            log.Info($"[{GetType().Name}] OnCloseAsync.");
            if (ViewModel != null)
            {
                ViewModel.OnClose();
            }
        }

        protected override async UniTask OnDestroyAsync()
        {
            log.Info($"[{GetType().Name}] OnDestroyAsync.");
            if (ViewModel != null)
            {
                ViewModel.Dispose();
            }
        }
    }
}
