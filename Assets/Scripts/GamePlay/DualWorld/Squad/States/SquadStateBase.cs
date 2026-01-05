using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public abstract class SquadStateBase
    {
        protected ServerSquad _context;
        protected ILog log = LogManager.GetLogger(typeof(SquadStateBase));

        public abstract eBuffBindAnimStackState StateId { get; }

        public void Init(ServerSquad context)
        {
            _context = context;
        }

        public virtual void OnEnter()
        {
            log.Info($"[{this.GetType().Name}] OnEnter NetId:{_context.NetId}");
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnExit()
        {
            log.Info($"[{this.GetType().Name}] OnExit NetId:{_context.NetId}");
        }
    }
}
