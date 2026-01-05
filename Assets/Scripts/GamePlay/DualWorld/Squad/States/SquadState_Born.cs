using UnityEngine;
using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    public class SquadState_Born : SquadStateBase
    {
        public override eBuffBindAnimStackState StateId => eBuffBindAnimStackState.Born;

        private float _timer;
        private const float BORN_DURATION = 1.5f; // Hardcoded simulation duration

        public override void OnEnter()
        {
            base.OnEnter();
            _timer = 0f;
            
            // Sync state to client
            _context.SetStackStateOnServer(StateId, true);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _timer += Time.deltaTime;

            if (_timer >= BORN_DURATION)
            {
                // Auto switch to Idle
                log.Info($"[SquadState_Born] Timer Finished ({_timer:F2}s), request switch to Idle");
                //_context.StackFsm.SetState(eBuffBindAnimStackState.Idle, true);
            }
        }
    }
}
