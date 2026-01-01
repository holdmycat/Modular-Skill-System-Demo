using Zenject;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// SubViewModel for a single Commander's UI module.
    /// Manages data for one specific Commander instance (Player or Enemy).
    /// </summary>
    public class CommanderInfoViewModel : SubViewModel
    {
        public event System.Action OnDataUpdated;

        private CommanderNumericComponent _commander;
        private IInstantiator _instantiator;
        
        [Inject]
        public void Construct(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }

        public FactionType FactionType => _commander.FactionType;
        
        // Observable Properties (simplified for demo, typically use ReactiveProperty)
        public float Health => _commander != null ? _commander[eNumericType.Hp] : 0;
        public float MaxHealth => _commander != null ? _commander[eNumericType.Hp] : 100;
        
        public int Level => _commander != null ? _commander.GetLevelForUI() : 1;
        
        public string BuffText => _commander != null ? eNumericType.InfantryAttackMod + " +" + _commander[eNumericType.InfantryAttackMod] + "%": "";
        public string Name => _commander != null ? _commander.UnitName : "Unknown";
        
        public string IconName => _commander != null ? _commander.UnitIcon : "Unknown";
        
        // List of owned Squad ViewModels
        private readonly System.Collections.Generic.List<SquadInfoViewModel> _squads = new System.Collections.Generic.List<SquadInfoViewModel>();
        public System.Collections.Generic.IReadOnlyList<SquadInfoViewModel> Squads => _squads;
        
        public string SquadCountText => $"Squads: {_squads.Count}";

        // Bind a specific commander data component to this ViewModel
        public void BindData(CommanderNumericComponent commander)
        {
            _commander = commander;
            if (_commander != null)
            {
                log.Info($"[CommanderInfoViewModel] Bound to Commander: {_commander.UnitName}");
                OnDataUpdated?.Invoke();
            }
            else
            {
                log.Warn("[CommanderInfoViewModel] Bound to NULL Commander");
            }
        }
        
        public void LevelUp()
        {
            if (_commander != null)
            {
                _commander.LevelUp();
                log.Info($"[CommanderInfoViewModel] Leveled Up to {_commander.GetLevelForUI()}");
                OnDataUpdated?.Invoke();
            }
        }

        public void LevelReset()
        {
            if (_commander != null)
            {
                _commander.Reset();
                log.Info($"[CommanderInfoViewModel] Leveled Up to {_commander.GetLevelForUI()}");
                OnDataUpdated?.Invoke();
            }
        }
        
        public void AddSquad(SquadNumericComponent squad)
        {
            if (squad != null)
            {
                // Check if already exists (opt: optimize with Dictionary)
                foreach(var existing in _squads) 
                {
                    // Assuming we can check equality via underlying data or logic, 
                    // but SquadInfoViewModel doesn't expose it yet directly. 
                    // Better to rely on logic caller generally, or check manually.
                    // For now, allow simple add, assuming uniqueness from caller.
                }
                
                var vm = _instantiator.Instantiate<SquadInfoViewModel>();
                vm.BindData(squad);
                _squads.Add(vm);
                
                log.Info($"[CommanderInfoViewModel] Added Squad VM: {squad.NetId}");
                OnDataUpdated?.Invoke();
            }
        }
    }
}
