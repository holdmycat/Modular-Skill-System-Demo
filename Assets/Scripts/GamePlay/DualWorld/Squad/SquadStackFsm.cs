using System;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Lightweight stack-style animation state machine for squads, keyed by UnitClassType.
    /// </summary>
    public class SquadStackFsm : ISquadFsmHandler, IDisposable
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
        
        /// <summary>
        /// Set the current stack state; fires change event if transitioned or forced.
        /// </summary>
        // Logic State Management
        private System.Collections.Generic.Dictionary<eBuffBindAnimStackState, SquadStateBase> _stateMap = new System.Collections.Generic.Dictionary<eBuffBindAnimStackState, SquadStateBase>();
        private SquadStateBase _currentLogicState;
        private bool _disposed;

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
        
        public void ClearStates()
        {
            if (_currentLogicState != null)
            {
                _currentLogicState.OnExit();
            }
            foreach (var kvp in _stateMap)
            {
                kvp.Value.OnRemove();
            }
            _currentLogicState = null;
            _stateMap.Clear();
            CurrentState = eBuffBindAnimStackState.NullStateID;
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(SquadStackFsm));

        #region Explicit implementation for privileged state transition (BT only).
        /// <summary>
        /// Explicit implementation for privileged state transition (BT only).
        /// </summary>
        void ISquadFsmHandler.TransitionState(eBuffBindAnimStackState state, bool force)
        {
            InternalSetState(state, force);
        }

        void ISquadFsmHandler.RemoveState(eBuffBindAnimStackState stateId)
        {
            if (_stateMap.ContainsKey(stateId))
            {
                if (_currentLogicState != null && _currentLogicState.StateId == stateId)
                {
                    _currentLogicState.OnExit();
                    _currentLogicState.OnRemove();
                    _currentLogicState = null;
                }
                _stateMap.Remove(stateId);
            }
        }
        #endregion
        
        /// <summary>
        /// Internal logic shared by Interface and RPC.
        /// </summary>
        private void InternalSetState(eBuffBindAnimStackState state, bool force)
        {
            if (_disposed) return;
            if (!force && CurrentState == state) return;
            
            log.Info($"[SquadStackFsm] SetState: {CurrentState} -> {state} (Force:{force})");

            // 1. Exit previous logic state
            if (_currentLogicState != null)
            {
                if (CurrentState != state || force)
                {
                    log.Info($"[SquadStackFsm] Exiting Logic State: {_currentLogicState.GetType().Name}");
                    _currentLogicState.OnExit();
                    _currentLogicState.OnRemove();
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
            if (_disposed) return;
            _currentLogicState?.OnUpdate();
        }

        public void Dispose()
        {
            if (_disposed) return;
            ClearStates();
            OnStateChanged = null;
            _disposed = true;
        }

        public class Factory : PlaceholderFactory<UnitClassType, SquadStackFsm>
        {
        }
    }
}
