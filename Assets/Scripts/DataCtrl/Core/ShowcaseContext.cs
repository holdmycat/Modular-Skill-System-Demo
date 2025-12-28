using System.Collections.Generic;
using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Provides a unified data access layer for the Showcase UI.
    /// Strictly adheres to the Data Layer; only stores Data Objects (Numeric Components).
    /// Does NOT reference Gameplay or Manager layers.
    /// </summary>
    public class ShowcaseContext : IContext
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ShowcaseContext));

        // Stores numeric components by NetId. 
        // Using BaseNumericComponent to allow storing Commander, Legion, Squad components uniformly,
        // or separate Dictionaries if type safety is preferred.
        private readonly Dictionary<uint, BaseNumericComponent> _numericComponents = new Dictionary<uint, BaseNumericComponent>();
        
        // Optional: Keep separate tracks for fast access if needed
        private readonly Dictionary<uint, CommanderNumericComponent> _commanderComponents = new Dictionary<uint, CommanderNumericComponent>();

        public ShowcaseContext()
        {
            log.Info("[ShowcaseContext] Constructed.");
        }

        /// <summary>
        /// Registers a numeric component. Called by the Gameplay layer (Commander/Legion) upon initialization.
        /// </summary>
        public void Register(uint netId, BaseNumericComponent component)
        {
            if (!_numericComponents.ContainsKey(netId))
            {
                _numericComponents.Add(netId, component);
                log.Info($"[ShowcaseContext] Registered NumericComponent for NetId: {netId}");

                if (component is CommanderNumericComponent commanderNumeric)
                {
                    _commanderComponents[netId] = commanderNumeric;
                }
            }
            else
            {
                log.Warn($"[ShowcaseContext] NumericComponent for NetId: {netId} already registered.");
            }
        }

        /// <summary>
        /// Unregisters a numeric component.
        /// </summary>
        public void Unregister(uint netId)
        {
            if (_numericComponents.ContainsKey(netId))
            {
                _numericComponents.Remove(netId);
                _commanderComponents.Remove(netId);
                log.Info($"[ShowcaseContext] Unregistered NumericComponent for NetId: {netId}");
            }
        }

        /// <summary>
        /// Retrieves a numeric component by NetId.
        /// </summary>
        public BaseNumericComponent GetData(uint netId)
        {
            if (_numericComponents.TryGetValue(netId, out var component))
            {
                return component;
            }
            return null;
        }

        /// <summary>
        /// Retrieves specific Commander data.
        /// </summary>
        public CommanderNumericComponent GetCommanderData(uint netId)
        {
            if (_commanderComponents.TryGetValue(netId, out var component))
            {
                return component;
            }
            return null;
        }
        
        /// <summary>
        /// Gets all registered numeric components.
        /// </summary>
        public IEnumerable<BaseNumericComponent> GetAllData() => _numericComponents.Values;
    }
}
