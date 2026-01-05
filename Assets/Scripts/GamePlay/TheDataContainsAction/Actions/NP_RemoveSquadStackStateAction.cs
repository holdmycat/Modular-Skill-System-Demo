using System;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Auto-generated action for menus:
    /// - SlgSquad/Task/SquadFsm/RemoveSquadStackState
    /// </summary>
    [Serializable]
    public class NP_RemoveSquadStackStateAction : NP_ClassForStoreAction
    {
        static readonly ILog log = LogManager.GetLogger(typeof(NP_RemoveSquadStackStateAction));

        [UnityEngine.SerializeField]
        public NP_BBValue_BuffBindAnimStackState StateToRemove = new NP_BBValue_BuffBindAnimStackState();

        public override System.Action GetActionToBeDone()
        {
            Action = OnAction;
            return Action;
        }

        private void OnAction()
        {
            var actor = Context.Resolver.Resolve(Context.OwnerId, Context.netPosition);
            if (actor is BaseSquad squad)
            {
                if (squad.StackFsm is ISquadFsmHandler fsmHandler)
                {
                    fsmHandler.RemoveState(StateToRemove.GetValue());
                }
                else
                {
                    log.Error($"[NP_RemoveSquadStackStateAction] Squad {squad.NetId} StackFsm is null.");
                }
            }
            else
            {
                log.Error("[NP_RemoveSquadStackStateAction] executed on a non-BaseSquad unit.");
            }
        }
    }
}
