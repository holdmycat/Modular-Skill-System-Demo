using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    public abstract class BaseCommander : SlgBattleEntity
    {
       
        
        protected ILegionIdGenerator _legionIdGenerator;
        protected CommanderSeed _seed;
        protected ulong _legionId;
        protected CommanderBootstrapInfo _bootstrapInfo;
        protected BaseLegion _baseLegion;

        public ulong LegionId => _legionId;

        /// <summary>
        /// Inject commander bootstrap data and bind net id (call once after creation).
        /// </summary>
        public virtual void Configure(CommanderBootstrapInfo bootstrapInfo)
        {
            _bootstrapInfo = bootstrapInfo;
            
            InitializeNumeric();
        }
        
    }
}
