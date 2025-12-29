using Zenject;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// ViewModel for the Showcase Scene UI.
    /// Bridges the Data Layer (ShowcaseContext) and the View (UIScene_ShowCaseScene).
    /// </summary>
    public sealed class ShowcaseViewModel : BaseViewModel
    {
        public CommanderInfoViewModel PlayerInfo { get; private set; }
        public CommanderInfoViewModel EnemyInfo { get; private set; }

        private readonly ShowcaseContext _context;
        private readonly IInstantiator _instantiator;

        [Inject]
        public ShowcaseViewModel(ShowcaseContext context, IInstantiator instantiator)
        {
            _context = context;
            _instantiator = instantiator;
            
            // Create SubViewModels using Zenject to ensure they get dependencies (SignalBus, etc.)
            PlayerInfo = _instantiator.Instantiate<CommanderInfoViewModel>();
            EnemyInfo = _instantiator.Instantiate<CommanderInfoViewModel>();
            
            log.Info("[ShowcaseViewModel] Constructed.");
        }

        public override void OnOpen()
        {
            base.OnOpen();
            log.Info("[ShowcaseViewModel] OnOpen - Binding SubViewModels.");

            // 1. Try Bind Existing (Synchronous)
            TryBindExisting();

            // 2. Subscribe for Future (Reactive)
            _context.OnCommanderAdded += HandleCommanderAdded;
        }

        public override void OnClose()
        {
            base.OnClose();
            _context.OnCommanderAdded -= HandleCommanderAdded;
        }

        private void TryBindExisting()
        {
             var playerCmder = _context.PlayerCommander;
             if(playerCmder != null) 
                 PlayerInfo.BindData(playerCmder);

             var enemyCmder = _context.GetEntities(FactionType.Enemy)?.FirstCommander;
             if(enemyCmder != null) 
                 EnemyInfo.BindData(enemyCmder);
        }

        private void HandleCommanderAdded(CommanderNumericComponent commander)
        {
             if (commander.FactionType == FactionType.Player)
             {
                 PlayerInfo.BindData(commander);
             }
             // For demo, assume any non-player is enemy or bind specific logic
             else if (commander.FactionType == FactionType.Enemy || commander.FactionType == FactionType.Terrorist)
             {
                 EnemyInfo.BindData(commander);
             }
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
        
        /// <summary>
        /// Logic: Get Local Player Health without knowing NetId manually.
        /// </summary>
        public float GetPlayerHealth()
        {
            var commander = _context.PlayerCommander;
            // var netId = _context.PlayerNetId; // Deprecated
            if (commander != null)
            {
                 log.Info($"[ShowcaseViewModel] GetPlayerHealth found PlayerCommander: {commander.UnitName}");
                 return commander[eNumericType.Hp]; // Assume GetNumeric exists on BaseNumericComponent
            }
            log.Warn("[ShowcaseViewModel] PlayerCommander Not Found in Context.");
            return 0f;
        }
    }
}
