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
        None = 0,
        Player = 1, 
        Legion = 10, 
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
        // Redundant: public uint CommanderNetId;
       
        public CommanderBootstrapInfo Bootstrap;

        public byte[] Serialize()
        {
            var ms = new System.IO.MemoryStream();
            using (var writer = new System.IO.BinaryWriter(ms))
            {
                // writer.Write(CommanderNetId);
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
                // payload.CommanderNetId = reader.ReadUInt32();
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
    
    [System.Serializable]
    public struct SquadSpawnPayload
    {
        public long SquadId;
        public long OwnerNetId;
        public FactionType Faction;
        
        public byte[] Serialize()
        {
            var ms = new System.IO.MemoryStream();
            using (var writer = new System.IO.BinaryWriter(ms))
            {
                writer.Write(SquadId);
                writer.Write(OwnerNetId);
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
                    Faction = (FactionType)reader.ReadInt32()
                };
            }
        }
    }
    
    public struct RpcDestroyObject : IRpc
    {
        public uint NetId;
    }

    /// <summary>
    /// RPC: Sync squad stack animation state from server to clients.
    /// </summary>
    public struct RpcSquadStackStateChanged : IRpc
    {
        public uint NetId;
        public UnitClassType ClassType;
        public eBuffBindAnimStackState State;
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
        void SendRpc<T>(uint netId, T rpc) where T : IRpc;
        void RegisterRpcListener(uint netId, System.Action<IRpc> handler);
        void UnregisterRpcListener(uint netId, System.Action<IRpc> handler);

        void RegisterSpawns(uint netId, INetworkBehaviour behaviour, bool isServer = false, bool isAutoRegisterRpc = true);
        
        void UnRegisterSpawns(uint netId, INetworkBehaviour behaviour, bool isServer = false, bool isAutoUnRegisterRpc = true);

        INetworkBehaviour GetSpawnedOrNull(uint netId, bool preferServer = false);
        
        
        // Server -> Client (Sync)
        void SyncTick(int tick);
        
        // Event Listeners (Simulated)
        event System.Action<ICommand> OnCommandReceived;
        event System.Action<IRpc> OnRpcReceived;
        event System.Action<int> OnTickSync;
    }
    
    
}
