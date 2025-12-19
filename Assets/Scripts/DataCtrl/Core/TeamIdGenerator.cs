//------------------------------------------------------------
// File: TeamIdGenerator.cs
// Purpose: Deterministic team id generation based on scenario/usage/faction/slot.
//------------------------------------------------------------
using Ebonor.Framework;
using Ebonor.DataCtrl;

namespace Ebonor.DataCtrl
{
    public enum TeamUsageType
    {
        Player = 0,
        Level = 1,
        PvP = 2,
        Tutorial = 3,
        Sandbox = 4
    }

    /// <summary>
    /// Components that define a unique team context.
    /// </summary>
    public struct TeamIdComponents
    {
        public TeamUsageType Usage;
        public string ScenarioId;   // e.g., level id / scene name / playlist id
        public FactionType Faction; // already defined in DataCtrl.Network
        public int Slot;            // e.g., player index, wave index, or team index within scenario
        public string Variant;      // optional: wave, difficulty, or custom tag

        public TeamIdComponents(TeamUsageType usage, string scenarioId, FactionType faction, int slot, string variant = "")
        {
            Usage = usage;
            ScenarioId = scenarioId ?? string.Empty;
            Faction = faction;
            Slot = slot;
            Variant = variant ?? string.Empty;
        }
    }

    public class TeamIdGenerator : ITeamIdGenerator
    {
        private const string Prefix = "team";

        public string BuildTeamKey(TeamIdComponents components)
        {
            // Normalized, readable, deterministic key.
            return $"{Prefix}:{components.Usage.ToString().ToLower()}:{Normalize(components.ScenarioId)}:{components.Faction}:{components.Slot}:{Normalize(components.Variant)}";
        }

        public int GenerateTeamId(TeamIdComponents components)
        {
            string key = BuildTeamKey(components);
            long hash = GlobalHelper.GetRoleID(key);
            if (hash < 0) hash = -hash;
            // Fit into int while keeping deterministic nature.
            return unchecked((int)(hash & 0x7FFFFFFF));
        }

        private static string Normalize(string value)
        {
            if (string.IsNullOrEmpty(value)) return "default";
            return value.Trim().Replace(" ", "_").ToLower();
        }
    }
}
