//------------------------------------------------------------
// File: LegionIdGenerator.cs
// Purpose: Generate Commander/Legion ids deterministically.
//------------------------------------------------------------
using System.Collections.Concurrent;
using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    public class LegionIdGenerator : ILegionIdGenerator
    {
        private const string CommanderPrefix = "commander";

        // Track per-commander incremental indices; thread-safe for future parallelism.
        private readonly ConcurrentDictionary<uint, uint> _counters = new ConcurrentDictionary<uint, uint>();
        private readonly IScenarioIdRegistry _scenarioRegistry;

        public LegionIdGenerator(IScenarioIdRegistry scenarioRegistry)
        {
            _scenarioRegistry = scenarioRegistry;
        }

        public uint GetCommanderNetId(CommanderSeed seed)
        {
            // Deterministic commander net id based on seed (usage/scenario/faction/slot/variant).
            var key = BuildCommanderKey(seed);
            long hash = GlobalHelper.GetRoleID(key);
            if (hash < 0) hash = -hash;

            // Keep inside uint range and stay away from reserved ids (0/1).
            uint candidate = (uint)(hash & 0x7FFFFFFF);
            if (candidate <= NetworkConstants.ROOM_MANAGER_NET_ID)
            {
                candidate = NetworkConstants.ROOM_MANAGER_NET_ID + 1 + candidate;
            }
            return candidate;
        }

        public ulong Next(uint commanderNetId)
        {
            // Increment per commander and compose the LegionId.
            var nextIndex = _counters.AddOrUpdate(commanderNetId, 1, (_, current) => unchecked(current + 1));
            return Compose(commanderNetId, nextIndex);
        }

        public ulong Compose(uint commanderNetId, uint localLegionIndex)
        {
            return ((ulong)commanderNetId << 32) | localLegionIndex;
        }

        public (uint commanderNetId, uint localLegionIndex) Decompose(ulong legionId)
        {
            uint commanderId = (uint)(legionId >> 32);
            uint localIndex = (uint)(legionId & 0xFFFFFFFF);
            return (commanderId, localIndex);
        }

        private string BuildCommanderKey(CommanderSeed seed)
        {
            if (seed == null)
            {
                seed = new CommanderSeed();
            }

            var scenario = _scenarioRegistry?.Normalize(seed.ScenarioId) ?? NormalizeLocal(seed.ScenarioId);
            _scenarioRegistry?.IsRegistered(scenario);

            return $"{CommanderPrefix}:{seed.Usage.ToString().ToLower()}:{scenario}:{seed.Faction}:{seed.Slot}:{NormalizeLocal(seed.Variant)}";
        }

        private static string NormalizeLocal(string value)
        {
            if (string.IsNullOrEmpty(value)) return "default";
            return value.Trim().Replace(" ", "_").ToLower();
        }
    }
}
