using Zenject;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// ViewModel for the Showcase Scene UI.
    /// Bridges the Data Layer (ShowcaseContext) and the View (UIScene_ShowCaseScene).
    /// </summary>
    public sealed class ShowcaseViewModel : BaseViewModel
    {
        private readonly ShowcaseContext _context;

        [Inject]
        public ShowcaseViewModel(ShowcaseContext context)
        {
            _context = context;
            log.Info("[ShowcaseViewModel] Constructed.");
        }

        public override void OnOpen()
        {
            base.OnOpen();
            // log.Info("[ShowcaseViewModel] OnOpen."); // BaseViewModel already logs this with type name
            // Example: Fire signal or refresh data
        }

        /// <summary>
        /// Example of Logic: Retrieve numeric data for a specific netId.
        /// The View calls this, not _context direct.
        /// </summary>
        public float GetUnitHealth(uint netId)
        {
             log.Info($"[ShowcaseViewModel] GetUnitHealth called for NetId: {netId}");
             // var data = _context.GetData(netId);
             // if (data != null)
             // {
             //     // Return some numeric value
             //     return data.GetNumeric(eNumericType.Hp); 
             // }
             return 0f;
        }
        
        
    }
}
