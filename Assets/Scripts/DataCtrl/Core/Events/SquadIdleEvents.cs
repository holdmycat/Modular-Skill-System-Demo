namespace Ebonor.DataCtrl
{
    public struct ClientSquadIdleEvent
    {
        public uint NetId;
    }

    public struct ClientAllSquadsIdleEvent
    {
        public uint CommanderNetId;
        public int SquadCount;
    }
}
