using System;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Auto-generated action for menus:
    /// - SlgSquad/Task/SquadFsm/ClearSquadStackState
    /// </summary>
    [Serializable]
    public class NP_ClearSquadStackStateAction : NP_ClassForStoreAction
    {
        static readonly ILog log = LogManager.GetLogger(typeof(NP_ClearSquadStackStateAction));

        public override System.Action GetActionToBeDone()
        {
            Action = OnAction;
            return Action;
        }

        private void OnAction()
        {
            // TODO: implement action logic
            
            var actor = Context.Resolver.Resolve(Context.OwnerId, Context.netPosition);
            if (actor is BaseSquad squad)
            {
                if (squad.StackFsm is ISquadFsmHandler fsmHandler)
                {
                    log.Info($"[Squad Behavior][NP_ClearSquadStackStateAction] NetId:{squad.NetId} clear states");
                    fsmHandler.ClearStates();
                }
                else
                {
                    log.Error($"[Squad Behavior][NP_ClearSquadStackStateAction] Squad {squad.NetId} does not implement ISquadFsmHandler or StackFsm is null.");
                }
            }
            else
            {
                log.Error("[Squad Behavior][NP_ClearSquadStackStateAction] executed on a non-BaseSquad unit.");
            }
            
        }

    }
}
