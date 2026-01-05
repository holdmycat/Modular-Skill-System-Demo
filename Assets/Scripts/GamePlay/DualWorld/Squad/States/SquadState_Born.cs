using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    public class SquadState_Born : SquadStateBase
    {

        public SquadState_Born(BaseSquad context) : base(context)
        {
            
        }
        
        public override eBuffBindAnimStackState StateId => eBuffBindAnimStackState.Born;

        //private float _timer;
        //private const float BORN_DURATION = 1.5f; // Hardcoded simulation duration

        public override void OnEnter()
        {
            base.OnEnter();
            //_timer = 0f;
            
            // Sync state to client
            //_context.SetStackStateOnServer(StateId, true);
        }
        
        public override void OnRemove()
        {
            
        }
        
    }
}
