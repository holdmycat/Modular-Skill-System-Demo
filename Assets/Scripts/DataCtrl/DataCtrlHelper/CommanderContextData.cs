using Ebonor.DataCtrl;

namespace Ebonor.DataCtrl
{
    public class CommanderContextData
    {
        private bool _isInitialized;

        public ulong LegionId { get; private set; }
        public bool IsServer { get; private set; }
        public CommanderBootstrapInfo BootstrapInfo { get; private set; }

        // --- Convenience Read-Only Properties ---
        
        public FactionType Faction
        {
            get
            {
                ValidateSeedData();
                return BootstrapInfo.LegionConfig.Seed.Faction;
            }
        }

        public CommanderUsageType Usage
        {
            get
            {
                ValidateSeedData();
                return BootstrapInfo.LegionConfig.Seed.Usage;
            }
        }

        public string ScenarioId
        {
            get
            {
                ValidateSeedData();
                return BootstrapInfo.LegionConfig.Seed.ScenarioId;
            }
        }

        public long CommanderId
        {
            get
            {
                ValidateSeedData();
                return BootstrapInfo.LegionConfig.Seed.CommanderId;
            }
        }

        public System.Collections.Generic.IReadOnlyList<long> SquadIds
        {
            get
            {
                if (BootstrapInfo == null) throw new System.InvalidOperationException("[CommanderContextData] BootstrapInfo is null.");
                if (BootstrapInfo.LegionConfig == null) throw new System.InvalidOperationException("[CommanderContextData] LegionConfig is null.");
                if (BootstrapInfo.LegionConfig.SquadIds == null) throw new System.InvalidOperationException("[CommanderContextData] SquadIds list is null.");
                return BootstrapInfo.LegionConfig.SquadIds;
            }
        }

        private void ValidateSeedData()
        {
            if (BootstrapInfo == null) throw new System.InvalidOperationException("[CommanderContextData] BootstrapInfo is null.");
            if (BootstrapInfo.LegionConfig == null) throw new System.InvalidOperationException("[CommanderContextData] LegionConfig is null.");
            if (BootstrapInfo.LegionConfig.Seed == null) throw new System.InvalidOperationException("[CommanderContextData] LegionConfig.Seed is null.");
        }


        // --- Write Access (Restricted) ---

        /// <summary>
        /// Initializes the context data. Can only be called once.
        /// Should only be called by ServerCommander or ClientCommander.
        /// </summary>
        public void SetContext(bool isServer, ulong legionId, CommanderBootstrapInfo info)
        {
            if (_isInitialized)
            {
                throw new System.InvalidOperationException("[CommanderContextData] Already initialized. Cannot set context twice.");
            }

            IsServer = isServer;
            LegionId = legionId;
            BootstrapInfo = info ?? throw new System.ArgumentNullException(nameof(info), "[CommanderContextData] BootstrapInfo cannot be null.");
            _isInitialized = true;
        }
    }
}
