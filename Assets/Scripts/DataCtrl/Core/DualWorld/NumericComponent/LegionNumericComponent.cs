using Ebonor.Framework;
using Zenject;

namespace Ebonor.DataCtrl
{
    public class LegionNumericComponent : BaseNumericComponent
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LegionNumericComponent));

        [Inject]
        public LegionNumericComponent(uint netId)
        {
            log.Info("[LegionNumericComponent] Construction");
            _netId = netId;
        }
        
        public class Factory : PlaceholderFactory<uint, LegionNumericComponent>
        {
            public Factory()
            {
                log.Info($"[LegionNumericComponent.Factory] Construction");
            }
        }

        protected override void OnInitialize()
        {
           
        }
        
        protected override void OnLevelUp(){
            
        }
    }
}
