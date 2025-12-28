using Ebonor.Framework;
using Zenject;

namespace Ebonor.DataCtrl
{
    public class LegionNumericComponent : BaseNumericComponent
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LegionNumericComponent));

        public LegionNumericComponent()
        {
            log.Info("[LegionNumericComponent] Construction");
        }

        public class Factory : PlaceholderFactory<LegionNumericComponent>
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
