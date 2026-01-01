using Ebonor.Framework;
using Zenject;

namespace Ebonor.DataCtrl
{
    public class SquadNumericComponent : BaseNumericComponent
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SquadNumericComponent));

        [Inject]
        public SquadNumericComponent(uint netId)
        {
            log.Info("[SquadNumericComponent] Construction");
            _netId = netId;
        }

        public class Factory : PlaceholderFactory<uint, SquadNumericComponent>
        {
        }

        protected override void OnInitialize()
        {
           
        }
        
        protected override void OnLevelUp(){
            
        }
    }
}
