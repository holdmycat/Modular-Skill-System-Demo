
using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    //PlaceholderFactory (当前方案)：最安全，类型严格。只能创建你明确绑定的类型（Commander/Legion/Squad）。
    public class NumericComponentFactory
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NumericComponentFactory));

        private readonly CommanderNumericComponent.Factory _commanderFactory;
        private readonly SquadNumericComponent.Factory _squadFactory;

        public NumericComponentFactory(
            CommanderNumericComponent.Factory commanderFactory,
            SquadNumericComponent.Factory squadFactory)
        {
            log.Info("[NumericComponentFactory] Construction");
            _commanderFactory = commanderFactory;
            _squadFactory = squadFactory;
        }
        
        public CommanderNumericComponent CreateCommander(uint netId)
        {
            return _commanderFactory.Create(netId);
        }

        public SquadNumericComponent CreateSquad(uint netid)
        {
            return _squadFactory.Create(netid);
        }
        
        
        //Zenject IInstantiator:灵活，可以创建任何东西，比 DiContainer 安全，但依然比 Factory 宽泛
        // private readonly IInstantiator _instantiator; // 比 DiContainer 权限更小，更安全
        // public NumericComponentFactory(IInstantiator instantiator)
        // {
        //     _instantiator = instantiator;
        // }
        // public T Create<T>() where T : Component
        // {
        //     return _instantiator.Instantiate<T>();
        // }
        
    }
    
    
    
  
}
