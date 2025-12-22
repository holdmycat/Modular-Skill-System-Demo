using UnityEngine;
using Zenject;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ActorNumericComponentFactory : IActorNumericComponentFactory
    {
        private readonly IInstantiator _instantiator;
        private static readonly ILog log = LogManager.GetLogger(typeof(ActorNumericComponentFactory));

        public ActorNumericComponentFactory(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }
        
        public T Create<T>(GameObject target) where T : ActorNumericComponentBase
        {
            if (target == null)
            {
                log.Error("[ActorNumericComponentFactory] Target GameObject is null!");
                return null;
            }

            // This is the key: The Factory uses the IInstantiator to instantiate the component.
            // This allows the component (T) to have its own [Inject] dependencies resolved,
            // while the Entity (caller) remains ignorant of the Container.
            return _instantiator.InstantiateComponent<T>(target);
        }
    }
}
