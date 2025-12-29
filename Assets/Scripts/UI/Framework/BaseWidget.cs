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
        protected static readonly ILog log = LogManager.GetLogger(typeof(BaseWidget<TViewModel>));

        protected TViewModel ViewModel { get; private set; }

        /// <summary>
        /// 1. Factory Creation: Zenject calls this with the ViewModel.
        /// 2. Scene Object: Zenject calls this with null (Optional=true), so we can use Open() later manually.
        /// </summary>
        [Inject]
        public void Construct([Inject(Optional = true)] TViewModel viewModel)
        {
            if (viewModel != null)
            {
                ViewModel = viewModel;
            }
        }

        /// <summary>
        /// Manual Binding Entry Point.
        /// Use this for Scene Objects where Construct() cannot be easily used.
        /// </summary>
        public virtual async UniTask ManualBindAndShow(TViewModel vm)
        {
            ViewModel = vm;
            await Show();
        }

        public virtual async UniTask Show()
        {
            log.Info($"[{GetType().Name}] Show.");
            gameObject.SetActive(true);
            await ShowAsync();
        }

        public virtual async UniTask Hide()
        {
            log.Info($"[{GetType().Name}] Hide.");
            await HideAsync();
            gameObject.SetActive(false);
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
