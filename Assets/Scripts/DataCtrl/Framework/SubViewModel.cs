using Zenject;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Base class for Child ViewModels (Widgets).
    /// Typically created by a Parent ViewModel or Factory.
    /// </summary>
    public abstract class SubViewModel : BaseViewModel
    {
        // Add specific SubViewModel logic here if needed.
        // For now, it shares the same base capabilities (SignalBus, Lifecycle).
        
        [Inject]
        public override void Construct(SignalBus signalBus)
        {
            base.Construct(signalBus);
        }
    }
}
