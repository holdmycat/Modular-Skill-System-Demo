using System;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Auto-generated action for menus:
    /// - SlgSquad/Task/SquadFsm/ChangeSquadStackState
    /// </summary>
    [Serializable]
    public class NP_ChangeSquadStackStateAction : NP_ClassForStoreAction
    {
        static readonly ILog log = LogManager.GetLogger(typeof(NP_ChangeSquadStackStateAction));

        [SerializeField]
        public NP_BBValue_BuffBindAnimStackState TargetState = new NP_BBValue_BuffBindAnimStackState();

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
                    fsmHandler.TransitionState(TargetState.GetValue());
                }
                else
                {
                    log.Error($"[NP_ChangeSquadStackStateAction] Squad {squad.NetId} does not implement ISquadFsmHandler or StackFsm is null.");
                }
            }
            else
            {
                 log.Error("[NP_ChangeSquadStackStateAction] executed on a non-BaseSquad unit.");
            }
        }
    }
}
