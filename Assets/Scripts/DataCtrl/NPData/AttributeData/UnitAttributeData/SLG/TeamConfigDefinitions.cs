//------------------------------------------------------------
// File: TeamConfigDefinitions.cs
// Purpose: Config-layer definitions for building team ids and composing teams (no runtime ids stored).
//------------------------------------------------------------
using System.Collections.Generic;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Seed data to generate a deterministic team id at runtime.
    /// </summary>
    [System.Serializable]
    public class TeamIdSeed
    {
        public TeamUsageType Usage;
        public string ScenarioId;
        public FactionType Faction;
        public int Slot;
        public string Variant;
    }

    /// <summary>
    /// A team definition composed of squad references (by SquadDataNodeId).
    /// </summary>
    [System.Serializable]
    public class TeamConfigDefinition
    {
        public TeamIdSeed Seed = new TeamIdSeed();
        public List<long> SquadIds = new List<long>();
    }

    /// <summary>
    /// Wave configuration for a level (multiple teams per wave are allowed).
    /// </summary>
    [System.Serializable]
    public class WaveConfig
    {
        public int WaveIndex;
        public List<TeamConfigDefinition> Teams = new List<TeamConfigDefinition>();
    }

    /// <summary>
    /// Level configuration that defines waves and their teams.
    /// </summary>
    [System.Serializable]
    public class LevelTeamConfig
    {
        public string LevelId;
        public List<WaveConfig> Waves = new List<WaveConfig>();
    }

    /// <summary>
    /// Player birth config: teams the player spawns with.
    /// </summary>
    [System.Serializable]
    public class PlayerBirthTeamConfig
    {
        public TeamConfigDefinition PlayerTeam = new TeamConfigDefinition();
    }
}
