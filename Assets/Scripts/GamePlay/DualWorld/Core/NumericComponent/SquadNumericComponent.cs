using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class SquadNumericComponent : BaseNumericComponent
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SquadNumericComponent));

        public SquadNumericComponent()
        {
            log.Info("[SquadNumericComponent] Construction");
        }

        public class Factory : PlaceholderFactory<SquadNumericComponent>
        {
        }

        protected override void OnInitialize()
        {
           
        }
        
        protected override void OnLevelUp(){
            
        }
    }
}
