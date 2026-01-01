namespace Ebonor.DataCtrl
{
    public class SquadInfoViewModel : SubViewModel
    {
        private SquadNumericComponent _squad;
        
        // Properties for UI binding
        public string IconName => _squad != null ? _squad.UnitIcon : "Unknown";
        public string UnitName => _squad != null ? _squad.UnitName : "Unknown";
        
        public int Count => _squad != null ? (int)_squad[eNumericType.SoldierCount] : 0;
        public int Level => _squad != null ? _squad.GetLevelForUI() : 1;

        public float Attack => _squad != null ? _squad[eNumericType.Attack] : 0;
        public float Hp => _squad != null ? _squad[eNumericType.Hp] : 0;

        public event System.Action OnDataUpdated;

        public FactionType FactionType => _squad.FactionType;
        
        public void BindData(SquadNumericComponent squad)
        {
            _squad = squad;
            if (_squad != null)
            {
                // Subscribe to events if needed for real-time updates
                 if(_squad is BaseNumericComponent baseNumeric)
                 {
                     baseNumeric.OnValueChanged += OnNumericChanged;
                 }
                OnDataUpdated?.Invoke();
            }
        }
        
        
        public void LevelUp()
        {
            if (_squad != null)
            {
                _squad.LevelUp();
                log.Info($"[SquadInfoViewModel] Leveled Up to {_squad.GetLevelForUI()}");
                OnDataUpdated?.Invoke();
            }
        }

        public void LevelReset()
        {
            if (_squad != null)
            {
                _squad.Reset();
                log.Info($"[SquadInfoViewModel] Leveled Up to {_squad.GetLevelForUI()}");
                OnDataUpdated?.Invoke();
            }
        }
        
        
        private void OnNumericChanged(eNumericType type, float value)
        {
            OnDataUpdated?.Invoke();
        }

        public override void OnClose()
        {
             if(_squad != null && _squad is BaseNumericComponent baseNumeric)
             {
                 baseNumeric.OnValueChanged -= OnNumericChanged;
             }
             base.OnClose();
        }
    }
}
