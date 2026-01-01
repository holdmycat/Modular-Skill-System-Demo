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

        public FactionType FactionType => _commander.FactionType;
        
        // Observable Properties (simplified for demo, typically use ReactiveProperty)
        public float Health => _commander != null ? _commander[eNumericType.Hp] : 0;
        public float MaxHealth => _commander != null ? _commander[eNumericType.Hp] : 100;
        
        public int Level => _commander != null ? _commander.GetLevelForUI() : 1;
        
        public string BuffText => _commander != null ? eNumericType.InfantryAttackMod + " +" + _commander[eNumericType.InfantryAttackMod] + "%": "";
        public string Name => _commander != null ? _commander.UnitName : "Unknown";
        
        public string IconName => _commander != null ? _commander.UnitIcon : "Unknown";
        
        // List of owned Squads
        private readonly System.Collections.Generic.List<SquadNumericComponent> _squads = new System.Collections.Generic.List<SquadNumericComponent>();
        public System.Collections.Generic.IReadOnlyList<SquadNumericComponent> Squads => _squads;
        
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
            if (squad != null && !_squads.Contains(squad))
            {
                _squads.Add(squad);
                log.Info($"[CommanderInfoViewModel] Added Squad: {squad.NetId}");
                OnDataUpdated?.Invoke();
            }
        }
    }
}
