using System;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Lightweight stack-style animation state machine for squads, keyed by UnitClassType.
    /// </summary>
    public class SquadStackFsm : ISquadFsmHandler
    {
        public UnitClassType ClassType { get; }

        public eBuffBindAnimStackState CurrentState { get; private set; } = eBuffBindAnimStackState.NullStateID;

        /// <summary>
        /// Birth logic already executed; prevents duplicate bootstrap.
        /// </summary>
        //public bool IsBirthInitialized { get; private set; }

        public event Action<eBuffBindAnimStackState> OnStateChanged;

        public SquadStackFsm(UnitClassType classType)
        {
            ClassType = classType;
        }

        // /// <summary>
        // /// Default birth flow: flag birth, fire Birth, then transition into Idle.
        // /// </summary>
        // public void BootstrapBirthThenIdle()
        // {
        //     if (IsBirthInitialized) return;
        //
        //     IsBirthInitialized = true;
        //     SetState(eBuffBindAnimStackState.Birth, true);
        //     //SetState(eBuffBindAnimStackState.Idle, true);
        // }

        /// <summary>
        /// Set the current stack state; fires change event if transitioned or forced.
        /// </summary>
        // Logic State Management
        private System.Collections.Generic.Dictionary<eBuffBindAnimStackState, SquadStateBase> _stateMap = new System.Collections.Generic.Dictionary<eBuffBindAnimStackState, SquadStateBase>();
        private SquadStateBase _currentLogicState;

        public void RegisterState(SquadStateBase state)
        {
            if (_stateMap.ContainsKey(state.StateId))
            {
                // overwrite or ignore?
                _stateMap[state.StateId] = state;
            }
            else
            {
                _stateMap.Add(state.StateId, state);
            }
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(SquadStackFsm));

        /// <summary>
        /// Explicit implementation for privileged state transition (BT only).
        /// </summary>
        void ISquadFsmHandler.TransitionState(eBuffBindAnimStackState state, bool force)
        {
            InternalSetState(state, force);
        }

        /// <summary>
        /// Infrastructure method for syncing state from RPC (Client only).
        /// </summary>
        public void SyncStateFromRpc(eBuffBindAnimStackState state, bool force = false)
        {
            log.Info($"[SquadStackFsm] SyncStateFromRpc: {state}");
            InternalSetState(state, force);
        }

        /// <summary>
        /// Internal logic shared by Interface and RPC.
        /// </summary>
        private void InternalSetState(eBuffBindAnimStackState state, bool force)
        {
            if (!force && CurrentState == state) return;
            
            log.Info($"[SquadStackFsm] SetState: {CurrentState} -> {state} (Force:{force})");

            // 1. Exit previous logic state
            if (_currentLogicState != null)
            {
                if (CurrentState != state || force)
                {
                    log.Info($"[SquadStackFsm] Exiting Logic State: {_currentLogicState.GetType().Name}");
                    _currentLogicState.OnExit();
                }
            }

            // 2. Update Enum Data
            CurrentState = state;
            
            // 3. Enter new logic state
            if (_stateMap.TryGetValue(state, out var newStateObj))
            {
                _currentLogicState = newStateObj;
                log.Info($"[SquadStackFsm] Entering Logic State: {_currentLogicState.GetType().Name}");
                _currentLogicState.OnEnter();
            }
            else
            {
                _currentLogicState = null;
                log.Info($"[SquadStackFsm] No Logic State registered for {state}");
            }

            OnStateChanged?.Invoke(state);
        }

        public void OnUpdate()
        {
            _currentLogicState?.OnUpdate();
        }

        public class Factory : PlaceholderFactory<UnitClassType, SquadStackFsm>
        {
        }
    }
}
