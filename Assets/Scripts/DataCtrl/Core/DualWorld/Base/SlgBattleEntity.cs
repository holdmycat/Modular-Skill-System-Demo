using Ebonor.DataCtrl;
using UnityEngine;
using Zenject;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Base class for all combat entities in the SLG dual-world system.
    /// Provides common identity, numeric component management, and network behavior.
    /// </summary>
    public abstract partial class SlgBattleEntity : NetworkBehaviour
    {
        [Inject]
        protected NumericComponentFactory _numericFactory;

        protected BaseNumericComponent _numericComponent;
        public BaseNumericComponent NumericComponent => _numericComponent;

        protected INetworkBus _networkBus;
        protected IDataLoaderService _dataLoaderService;
        
        /// <summary>
        /// Initialize the numeric component specific to this entity type.
        /// Should be called during the Configure/Init phase.
        /// </summary>
        protected abstract void InitializeNumeric();

        /// <summary>
        /// Helper to access the concrete numeric component.
        /// </summary>
        public T GetNumeric<T>() where T : BaseNumericComponent
        {
            return _numericComponent as T;
        }
    }
}
