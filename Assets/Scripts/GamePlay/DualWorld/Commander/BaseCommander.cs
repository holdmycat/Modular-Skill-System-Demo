using System.Collections.Generic;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public abstract class BaseCommander : SlgBattleEntity
    {
        protected CommanderSeed _seed;
        protected CommanderBootstrapInfo _bootstrapInfo;
        protected static readonly ILog log = LogManager.GetLogger(typeof(BaseCommander));
        protected List<BaseSquad> _spawnedSquads = new List<BaseSquad>();
        
        /// <summary>
        /// Inject commander bootstrap data and bind net id (call once after creation).
        /// </summary>
        public void Configure(CommanderBootstrapInfo bootstrapInfo,  bool isServer = true, uint tmpnetId = 0)
        {
            _bootstrapInfo = bootstrapInfo;

            Faction = bootstrapInfo.LegionConfig.Seed.Faction;
            
            
            // base.Configure(bootstrapInfo); // MOVED TO END
            
            // Manual set for local logic usage if needed, though we use param 'bootstrapInfo'
            _bootstrapInfo = bootstrapInfo; 
            
            if (_bootstrapInfo == null || _bootstrapInfo.LegionConfig == null)
            {
                log.Error("[ServerCommander] Configure failed: bootstrap info missing.");
                throw new System.InvalidOperationException("[ServerCommander] Configure failed: bootstrap info missing.");
            }

            if (_bootstrapInfo.LegionConfig.Seed == null)
            {
                log.Error("[ServerCommander] Configure failed: LegionConfig.Seed is null. This is a critical data error.");
                throw new System.InvalidOperationException("[ServerCommander] Configure failed: LegionConfig.Seed is null.");
            }

            uint netId = tmpnetId;
            if (isServer)
            {
                _seed = _bootstrapInfo.LegionConfig.Seed;
                netId = _dataLoaderService.NextId();
            }
            
            // Populate Context Data (Write Once)
            _contextData.SetContext(true, _bootstrapInfo);
            
            BindId(netId);
            
            // Now call base to trigger Numeric Init (which depends on Context)
            InitializeNumeric();
            
            _networkBus.RegisterSpawns(NetId, this, isServer);
            
            
        }
        
    }
}
