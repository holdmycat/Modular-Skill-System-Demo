using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class SquadState_Born : SquadStateBase
    {

        public SquadState_Born(BaseSquad context) : base(context)
        {
            
        }
        
        public override eBuffBindAnimStackState StateId => eBuffBindAnimStackState.Born;
        
    }
}
