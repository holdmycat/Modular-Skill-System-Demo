using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public abstract class SquadStateBase
    {
        protected BaseSquad _context;
        protected ILog log = LogManager.GetLogger(typeof(SquadStateBase));

        public abstract eBuffBindAnimStackState StateId { get; }

     
        
        public SquadStateBase(BaseSquad context)
        {
            _context = context;
        }

        public virtual void OnEnter()
        {
            log.Info($"[Squad Behavior][{this.GetType().Name}] OnEnter NetId:{_context.NetId}");
            _context.Blackboard.Set(ConstData.BB_BUFFBINDANIMSTACKSTATE, StateId);
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnRemove()
        {
            log.Info($"[Squad Behavior][{this.GetType().Name}] OnRemove NetId:{_context.NetId}");
        }
        
        public virtual void OnExit()
        {
            log.Info($"[Squad Behavior][{this.GetType().Name}] OnExit NetId:{_context.NetId}");
        }
    }
}
