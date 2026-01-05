using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    public class SquadState_Idle : SquadStateBase
    {
        public override eBuffBindAnimStackState StateId => eBuffBindAnimStackState.Idle;

        public override void OnEnter()
        {
            base.OnEnter();
            
            // Sync state to client
            _context.SetStackStateOnServer(StateId, true);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            // Idle logic: waiting for BattleStart signal which is handled in ServerSquad context or AI tree.
            // This state primarily ensures the animation is "Idle".
        }
    }
}
