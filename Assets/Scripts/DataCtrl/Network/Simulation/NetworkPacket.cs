// ---------------------------------------------
// Script: NetworkPacket.cs
// Purpose: Defines network-facing enums, packet contracts, and spawn payload serialization helpers.
// ---------------------------------------------
using System.Collections.Generic;
using System;

namespace Ebonor.DataCtrl
{
    public enum FactionType
    {
        Player = 1,
        Ally = 2,
        Enemy = 3,
        Neutral = 4,
        Terrorist = 5,
    }
    
    public enum NetworkPrefabType : int
    {
        Player = 1, 
        Team = 10, 
        Squad = 1000, 
        Soldier = 10000, 
    }
    

    // Marker Interfaces
    public interface ICommand { }
    public interface IRpc { }

    // --- Basic Packets ---

    public struct CmdCastSkill : ICommand
    {
        public long CasterId;
        public long SkillId;
        public long TargetId;
    }


    public static class NetworkConstants
    {
        public const uint ROOM_MANAGER_NET_ID = 1;
    }

    
    public struct RpcSpawnObject : IRpc
    {
        public NetworkPrefabType Type;
        public uint NetId;
        public byte[] Payload; // Serialized data (SpawnPayloads)
    }

    [System.Serializable]
    public struct CommanderSpawnPayload
    {
        public uint CommanderNetId;
        public ulong LegionId;
        public CommanderBootstrapInfo Bootstrap;

        public byte[] Serialize()
        {
            var ms = new System.IO.MemoryStream();
            using (var writer = new System.IO.BinaryWriter(ms))
            {
                writer.Write(CommanderNetId);
                writer.Write(LegionId);
                bool hasBootstrap = Bootstrap != null;
                writer.Write(hasBootstrap);
                if (hasBootstrap)
                {
                    Bootstrap.Serialize(writer);
                }
            }
            return ms.ToArray();
        }

        public static CommanderSpawnPayload Deserialize(byte[] data)
        {
            if (data == null || data.Length == 0) return default;
            var ms = new System.IO.MemoryStream(data);
            using (var reader = new System.IO.BinaryReader(ms))
            {
                var payload = new CommanderSpawnPayload();
                payload.CommanderNetId = reader.ReadUInt32();
                payload.LegionId = reader.ReadUInt64();
                bool hasBootstrap = reader.ReadBoolean();
                if (!hasBootstrap)
                {
                    throw new InvalidOperationException("[CommanderSpawnPayload] Missing bootstrap data in payload.");
                }
                
                payload.Bootstrap = CommanderBootstrapInfo.Deserialize(reader);
                return payload;
            }
        }
    }
    
    public struct RpcCreateSoldier : IRpc
    {
        public FactionType FactionType;
        public long LegionId;
        public int SoldierId; // NetId
    }
    
    public struct RpcDestroyObject : IRpc
    {
        public uint NetId;
    }

    public struct RpcSquadDamage : IRpc
    {
        public uint SquadNetId;
        public List<int> DeadSoldierIndices;
        public float RemainingTotalHp;
    }
    /// <summary>
    /// Network Interface for sending Commands and RPCs.
    /// This abstracts the underlying network layer (Socket/Photon/Mirror).
    /// </summary>
    public interface INetworkBus
    {
        // Client -> Server
        void SendCommand<T>(uint netId, T cmd) where T : ICommand;
        void RegisterCommandListener(uint netId, System.Action<ICommand> handler);
        void UnregisterCommandListener(uint netId, System.Action<ICommand> handler);
        
        // Server -> Client
        void SendRpc<T>(T rpc) where T : IRpc;
        void RegisterRpcListener(uint netId, System.Action<IRpc> handler);
        void UnregisterRpcListener(uint netId, System.Action<IRpc> handler);

        void RegisterSpawns(uint netId, INetworkBehaviour behaviour, bool isServer = false);
        
        void UnRegisterSpawns(uint netId, INetworkBehaviour behaviour);

        INetworkBehaviour GetSpawnedOrNull(uint netId, bool preferServer = false);
        
        
        // Server -> Client (Sync)
        void SyncTick(int tick);
        
        // Event Listeners (Simulated)
        event System.Action<ICommand> OnCommandReceived;
        event System.Action<IRpc> OnRpcReceived;
        event System.Action<int> OnTickSync;
    }
    public struct LegionSpawnPayload
    {
        public long LegionId;
        public List<long> SquadList;
        public FactionType Faction;
        public uint OwnerNetId;
        
        public byte[] Serialize()
        {
            var ms = new System.IO.MemoryStream();
            using (var writer = new System.IO.BinaryWriter(ms))
            {
                writer.Write(LegionId);
                writer.Write(SquadList.Count);
                foreach (var id in SquadList)
                {
                    writer.Write(id);
                }
                writer.Write((int)Faction);
                writer.Write(OwnerNetId);
            }
            return ms.ToArray();
        }

        public static LegionSpawnPayload Deserialize(byte[] data)
        {
            if (data == null || data.Length == 0) return default;
            var ms = new System.IO.MemoryStream(data);
            using (var reader = new System.IO.BinaryReader(ms))
            {
                var payload = new LegionSpawnPayload();
                payload.LegionId = reader.ReadInt64();
                int count = reader.ReadInt32();
                payload.SquadList = new List<long>();
                for (int i = 0; i < count; i++)
                {
                    payload.SquadList.Add(reader.ReadInt64());
                }
                payload.Faction = (FactionType)reader.ReadInt32();
                payload.OwnerNetId = (uint)reader.ReadInt32();
                return payload;
            }
        }
    }

    [System.Serializable]
    public struct SquadSpawnPayload
    {
        public long SquadId;
        public long OwnerNetId;
        public long LegionNetId;
        public FactionType Faction;
        
        public byte[] Serialize()
        {
            var ms = new System.IO.MemoryStream();
            using (var writer = new System.IO.BinaryWriter(ms))
            {
                writer.Write(SquadId);
                writer.Write(OwnerNetId);
                writer.Write(LegionNetId);
                writer.Write((int)Faction);
            }
            return ms.ToArray();
        }

        public static SquadSpawnPayload Deserialize(byte[] data)
        {
            if (data == null || data.Length == 0) return default;
            var ms = new System.IO.MemoryStream(data);
            using (var reader = new System.IO.BinaryReader(ms))
            {
                return new SquadSpawnPayload
                {
                    SquadId = reader.ReadInt64(),
                    OwnerNetId = reader.ReadInt64(),
                    LegionNetId = reader.ReadInt64(),
                    Faction = (FactionType)reader.ReadInt32()
                };
            }
        }
    }
}
