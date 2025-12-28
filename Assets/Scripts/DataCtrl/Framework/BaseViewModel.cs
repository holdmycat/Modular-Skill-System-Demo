using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Base class for all ViewModels in the SLG MVVM Framework.
    /// Resides in Data Layer as per architectural requirements.
    /// Manages Lifecycle, SignalBus, and Async Operations.
    /// </summary>
    public abstract class BaseViewModel : IDisposable
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(BaseViewModel));

        protected SignalBus _signalBus;
        
        // CancellationToken for async operations scoped to this ViewModel's lifecycle
        private CancellationTokenSource _lifecycleCts = new CancellationTokenSource();
        protected CancellationToken LifecycleToken => _lifecycleCts.Token;

        [Inject]
        public virtual void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public virtual void OnOpen()
        {
            log.Info($"[{GetType().Name}] OnOpen.");
            // Hook for UI Open
        }

        public virtual void OnClose()
        {
            log.Info($"[{GetType().Name}] OnClose.");
            // Hook for UI Close
        }

        public virtual void Dispose()
        {
            log.Info($"[{GetType().Name}] Dispose.");
            if (_lifecycleCts != null)
            {
                _lifecycleCts.Cancel();
                _lifecycleCts.Dispose();
                _lifecycleCts = null;
            }
        }
    }
}
