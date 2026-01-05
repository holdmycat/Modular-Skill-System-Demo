using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Privileged interface for changing Squad FSM states.
    /// Should ONLY be used by Behavior Tree Action Nodes or Infrastructure (RPC).
    /// </summary>
    public interface ISquadFsmHandler
    {
        void TransitionState(eBuffBindAnimStackState newState, bool force = false);
        
        void RemoveState(eBuffBindAnimStackState newState);
    }
}
