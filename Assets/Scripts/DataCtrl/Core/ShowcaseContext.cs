using System;
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

        public event Action<CommanderNumericComponent> OnCommanderAdded;
        public event Action<LegionNumericComponent> OnLegionAdded;

        // Stores numeric components by NetId. 
        // Using BaseNumericComponent to allow storing Commander, Legion, Squad components uniformly,
        // or separate Dictionaries if type safety is preferred.
        private readonly Dictionary<uint, BaseNumericComponent> _numericComponents = new Dictionary<uint, BaseNumericComponent>();
        
        // Optional: Keep separate tracks for fast access if needed
        private readonly Dictionary<uint, CommanderNumericComponent> _commanderComponents = new Dictionary<uint, CommanderNumericComponent>();
        private readonly Dictionary<uint, LegionNumericComponent> _legionComponents = new Dictionary<uint, LegionNumericComponent>();
        
        // Faction Map: Type -> Structured Entity Collection
        private readonly Dictionary<FactionType, FactionEntities> _factionMap = new Dictionary<FactionType, FactionEntities>();

        public ShowcaseContext()
        {
            log.Info("[ShowcaseContext] Constructed.");
        }
        
        /// <summary>
        /// Convenience property to get the local player's Commander Component.
        /// Returns null if not found.
        /// </summary>
        public CommanderNumericComponent PlayerCommander
        {
            get
            {
                if (_factionMap.TryGetValue(FactionType.Player, out var entities))
                {
                    return entities.FirstCommander;
                }
                return null;
            }
        }
        
        /// <summary>
        /// Get all entities for a specific faction.
        /// </summary>
        public FactionEntities GetEntities(FactionType faction)
        {
            if (_factionMap.TryGetValue(faction, out var entities))
            {
                return entities;
            }

            //log.Warn($"[ShowcaseContext] GetEntities Faction: {faction} not found at _factionMap");
            return null;
        }

        /// <summary>
        /// Registers a numeric component. Called by the Gameplay layer (Commander/Legion) upon initialization.
        /// </summary>
        public void Register(uint netId, BaseNumericComponent component, FactionType faction = FactionType.Neutral)
        {
            if (!_numericComponents.ContainsKey(netId))
            {
                _numericComponents.Add(netId, component);
                log.Info($"[ShowcaseContext] Registered NumericComponent for NetId: {netId}, Faction: {faction}, Type: {component.GetType().Name}");

                if (component is CommanderNumericComponent commanderNumeric)
                {
                    _commanderComponents[netId] = commanderNumeric;
                    OnCommanderAdded?.Invoke(commanderNumeric);
                }
                else if (component is LegionNumericComponent legionNumeric)
                {
                    _legionComponents[netId] = legionNumeric;
                    OnLegionAdded?.Invoke(legionNumeric);
                }
                
                // Add to Faction Map
                if (!_factionMap.ContainsKey(faction))
                {
                    _factionMap[faction] = new FactionEntities();
                }
                
                var entities = _factionMap[faction];
                
                if (component is CommanderNumericComponent c)
                {
                    entities.Commanders.Add(c);
                }
                else if (component is LegionNumericComponent l)
                {
                    entities.Legions.Add(l);
                }
                else if (component is SquadNumericComponent s)
                {
                    entities.Squads.Add(s);
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
            if (_numericComponents.TryGetValue(netId, out var component))
            {
                _numericComponents.Remove(netId);
                _commanderComponents.Remove(netId);
                _legionComponents.Remove(netId);
                
                // Remove from all faction maps
                // Iterate factions to find where it is stored.
                foreach (var kvp in _factionMap)
                {
                    var entities = kvp.Value;
                    
                    if (component is CommanderNumericComponent c)
                    {
                        if(entities.Commanders.Remove(c)) break;
                    }
                    else if (component is LegionNumericComponent l)
                    {
                        if(entities.Legions.Remove(l)) break;
                    }
                    else if (component is SquadNumericComponent s)
                    {
                        if(entities.Squads.Remove(s)) break;
                    }
                }
                
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
        /// Retrieves specific Legion data.
        /// </summary>
        public LegionNumericComponent GetLegionData(uint netId)
        {
            if (_legionComponents.TryGetValue(netId, out var component))
            {
                return component;
            }
            return null;
        }
        
        /// <summary>
        /// Gets all registered numeric components.
        /// </summary>
        public IEnumerable<BaseNumericComponent> GetAllData() => _numericComponents.Values;

        /// <summary>
        /// Data structure to organize entities within a faction.
        /// Directly stores Component references.
        /// </summary>
        public class FactionEntities
        {
             public readonly HashSet<CommanderNumericComponent> Commanders = new HashSet<CommanderNumericComponent>();
             public readonly HashSet<LegionNumericComponent> Legions = new HashSet<LegionNumericComponent>();
             public readonly HashSet<SquadNumericComponent> Squads = new HashSet<SquadNumericComponent>();
             
             // Helper logic
             public CommanderNumericComponent FirstCommander 
             {
                 get
                 {
                     foreach (var c in Commanders) return c;
                     return null;
                 }
             }
        }
    }
}
