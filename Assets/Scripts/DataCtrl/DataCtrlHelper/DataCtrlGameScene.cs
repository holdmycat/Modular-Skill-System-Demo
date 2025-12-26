//------------------------------------------------------------
// File: DataCtrlStruct.cs
// Created: 2025-12-01
// Purpose: Shared data-control enums and animation configuration structures.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
namespace Ebonor.DataCtrl
{
    
    public sealed class CommanderBootstrapInfo
    {
        public string PlayerId { get; }
        public LegionConfigDefinition LegionConfig { get; }
        
        public CommanderBootstrapInfo(string playerId, LegionConfigDefinition legionConfig)
        {
            PlayerId = playerId;
            //FactionId = factionId;
            LegionConfig = legionConfig;
        }

        public void Serialize(System.IO.BinaryWriter writer)
        {
            writer.Write(PlayerId ?? string.Empty);
            bool hasConfig = LegionConfig != null;
            writer.Write(hasConfig);
            if (hasConfig)
            {
                LegionConfig.Serialize(writer);
            }
        }

        public static CommanderBootstrapInfo Deserialize(System.IO.BinaryReader reader)
        {
            string playerId = reader.ReadString();
            LegionConfigDefinition legionConfig = null;
            bool hasConfig = reader.ReadBoolean();
            if (hasConfig)
            {
                legionConfig = LegionConfigDefinition.Deserialize(reader);
            }
            return new CommanderBootstrapInfo(playerId, legionConfig);
        }
    }
    
}
