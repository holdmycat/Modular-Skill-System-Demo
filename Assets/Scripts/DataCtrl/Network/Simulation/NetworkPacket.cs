using System.Collections.Generic;

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

    public struct RpcPlayEffect : IRpc
    {
        public long TargetId;
        public string EffectName;
    }

    public struct RpcSyncHealth : IRpc
    {
        public long UnitId;
        public float CurrentHP;
    }

    public struct RpcCreateFaction : IRpc
    {
        public FactionType FactionId;
    }

    public struct RpcCreateCharacter : IRpc
    {
        public uint NetId;
    }
    
    
    public struct RpcCreateTeam : IRpc
    {
        public FactionType FactionId;
        public long TeamId;
        public List<long> SquadList;
    }

    public struct RpcCreateSoldier : IRpc
    {
        public FactionType FactionId;
        public long TeamId;
        public int SoldierId; // NetId
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
        
        // Server -> Client (Sync)
        void SyncTick(int tick);
        
        // Event Listeners (Simulated)
        event System.Action<ICommand> OnCommandReceived;
        event System.Action<IRpc> OnRpcReceived;
        event System.Action<int> OnTickSync;
    }
}
