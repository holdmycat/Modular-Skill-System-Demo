using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class SquadState_Death : SquadStateBase
    {
        public override eBuffBindAnimStackState StateId => eBuffBindAnimStackState.Die;

        public SquadState_Death(BaseSquad context) : base(context)
        {
            
        }
        
        public override void OnRemove()
        {
            _context.Blackboard.Set(ConstData.BB_BUFFBINDANIMSTACKSTATE, eBuffBindAnimStackState.NullStateID);
        }
        
    }
}
