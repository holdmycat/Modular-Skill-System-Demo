namespace Ebonor.DataCtrl
{
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
        public int FactionId;
    }

    public struct RpcCreateTeam : IRpc
    {
        public int FactionId;
        public int TeamId;
    }

    public struct RpcCreateSoldier : IRpc
    {
        public int FactionId;
        public int TeamId;
        public int SoldierId; // NetId
    }
    
    /// <summary>
    /// Network Interface for sending Commands and RPCs.
    /// This abstracts the underlying network layer (Socket/Photon/Mirror).
    /// </summary>
    public interface INetworkBus
    {
        // Client -> Server
        void SendCommand<T>(T cmd) where T : ICommand;
        
        // Server -> Client
        void SendRpc<T>(T rpc) where T : IRpc;
        
        // Server -> Client (Sync)
        void SyncTick(int tick);
        
        // Event Listeners (Simulated)
        event System.Action<ICommand> OnCommandReceived;
        event System.Action<IRpc> OnRpcReceived;
        event System.Action<int> OnTickSync;
    }
}
