using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.UI
{
    /// <summary>
    /// Base class for Level 2+ UIs (Widgets/Child Components) that have a ViewModel.
    /// Typically embedded in a Window Prefab.
    /// </summary>
    /// <typeparam name="TViewModel">The specific SubViewModel type.</typeparam>
    public abstract class BaseWidget<TViewModel> : UIPanelBase where TViewModel : BaseViewModel
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BaseWidget<TViewModel>));

        protected TViewModel ViewModel { get; private set; }

        [Inject]
        public void Construct(TViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        protected override async UniTask OnShowAsync()
        {
            log.Info($"[{GetType().Name}] OnShowAsync.");
            if (ViewModel != null)
            {
                ViewModel.OnOpen();
            }
        }

        protected override async UniTask OnHideAsync()
        {
            log.Info($"[{GetType().Name}] OnHideAsync.");
            if (ViewModel != null)
            {
                ViewModel.OnClose();
            }
        }
        
        protected virtual void OnDestroy()
        {
            log.Info($"[{GetType().Name}] OnDestroy.");
            // Note: Widgets might not control ViewModel lifecycle if shared, 
            // but usually they own their SubViewModel instance if Transient.
            if (ViewModel != null)
            {
                ViewModel.Dispose();
            }
        }
    }
}
