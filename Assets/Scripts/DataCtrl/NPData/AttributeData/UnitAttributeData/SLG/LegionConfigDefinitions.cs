//------------------------------------------------------------
// File: TeamConfigDefinitions.cs
// Purpose: Config-layer definitions for building team ids and composing teams (no runtime ids stored).
//------------------------------------------------------------
using System.Collections.Generic;

namespace Ebonor.DataCtrl
{
    
    public enum CommanderUsageType
    {
        Player = 0,
        Level = 1,
        PvP = 2,
        Tutorial = 3,
        Sandbox = 4
    }
    
    /// <summary>
    /// Seed data to generate a deterministic team id at runtime.
    /// </summary>
    [System.Serializable]
    public class CommanderSeed
    {
        public CommanderUsageType Usage;
        public string ScenarioId;
        public FactionType Faction;
        public int Slot;
        public string Variant;

        public void Serialize(System.IO.BinaryWriter writer)
        {
            writer.Write((int)Usage);
            writer.Write(ScenarioId ?? string.Empty);
            writer.Write((int)Faction);
            writer.Write(Slot);
            writer.Write(Variant ?? string.Empty);
        }

        public static CommanderSeed Deserialize(System.IO.BinaryReader reader)
        {
            var seed = new CommanderSeed
            {
                Usage = (CommanderUsageType)reader.ReadInt32(),
                ScenarioId = reader.ReadString(),
                Faction = (FactionType)reader.ReadInt32(),
                Slot = reader.ReadInt32(),
                Variant = reader.ReadString()
            };
            return seed;
        }
    }
    
    /// <summary>
    /// A team definition composed of squad references (by SquadDataNodeId).
    /// </summary>
    [System.Serializable]
    public class LegionConfigDefinition
    {
        public CommanderSeed Seed = new CommanderSeed();
        public List<long> SquadIds = new List<long>();

        public void Serialize(System.IO.BinaryWriter writer)
        {
            Seed?.Serialize(writer);
            writer.Write(SquadIds.Count);
            foreach (var id in SquadIds)
            {
                writer.Write(id);
            }
        }

        public static LegionConfigDefinition Deserialize(System.IO.BinaryReader reader)
        {
            var def = new LegionConfigDefinition
            {
                Seed = CommanderSeed.Deserialize(reader)
            };
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                def.SquadIds.Add(reader.ReadInt64());
            }
            return def;
        }
    }
    
    /// <summary>
    /// Commander birth config: teams the player spawns with.
    /// </summary>
    [System.Serializable]
    public class CommanderBirthConfig
    {
        public LegionConfigDefinition LegionConfig = new LegionConfigDefinition();
    }
}
