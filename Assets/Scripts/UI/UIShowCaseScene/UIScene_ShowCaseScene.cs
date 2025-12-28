using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.UI
{
    public class UIScene_ShowCaseScene : BaseWindow<ShowcaseViewModel>
    {
        // No explicit injection needed here anymore, BaseWindow handles it via [Inject] Property.
        private static readonly ILog log = LogManager.GetLogger(typeof(UIScene_ShowCaseScene));
        protected override async UniTask OnCreateAsync()
        {
            log.Info($"[{GetType().Name}] OnCreateAsync.");
        }

        protected override async UniTask OnOpenAsync()
        {
            // Call base to trigger ViewModel.OnOpen()
            await base.OnOpenAsync();
            
            // Example usage:
            if (ViewModel != null)
            {
               // float hp = ViewModel.GetUnitHealth(0);
            }
            
        }

        // protected override async UniTask OnCloseAsync()
        // {
        //     await base.OnCloseAsync();
        // }

        // protected override async UniTask OnDestroyAsync()
        // {
        //     await base.OnDestroyAsync();
        // }
    }
}
