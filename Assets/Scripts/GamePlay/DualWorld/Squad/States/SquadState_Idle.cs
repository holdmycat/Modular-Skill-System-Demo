using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    public class SquadState_Idle : SquadStateBase
    {
        public override eBuffBindAnimStackState StateId => eBuffBindAnimStackState.Idle;

        public SquadState_Idle(BaseSquad context) : base(context)
        {
            
        }
    }
}
