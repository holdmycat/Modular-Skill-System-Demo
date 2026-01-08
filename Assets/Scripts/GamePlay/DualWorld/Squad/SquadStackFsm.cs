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

        public event Action<eBuffBindAnimStackState> OnStateChanged;

        public SquadStackFsm(UnitClassType classType)
        {
            ClassType = classType;
        }
        
        // Registered states
        private readonly System.Collections.Generic.Dictionary<eBuffBindAnimStackState, SquadStateBase> _stateMap = new System.Collections.Generic.Dictionary<eBuffBindAnimStackState, SquadStateBase>();
        // Active states ordered by priority (higher first). Only the head runs.
        private readonly System.Collections.Generic.LinkedList<SquadStateBase> _activeStates = new System.Collections.Generic.LinkedList<SquadStateBase>();
        private SquadStateBase _currentLogicState;
        private bool _disposed;

        public void RegisterState(SquadStateBase state)
        {
            if (_stateMap.ContainsKey(state.StateId))
            {
                _stateMap[state.StateId] = state;
            }
            else
            {
                _stateMap.Add(state.StateId, state);
            }
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
            InternalRemoveState(stateId);
        }
        
        void ISquadFsmHandler.ClearStates(bool clearStateMap)
        {
            _currentLogicState?.OnExit();

            if (clearStateMap)
            {
                foreach (var kvp in _stateMap)
                {
                    kvp.Value.OnRemove();
                }
                _activeStates.Clear();
                _stateMap.Clear();
            }
            else
            {
                foreach (var state in _activeStates)
                {
                    state.OnRemove();
                }
                _activeStates.Clear();
            }

            _currentLogicState = null;
            CurrentState = eBuffBindAnimStackState.NullStateID;
        }
        
        #endregion
        
        /// <summary>
        /// Internal logic shared by Interface and RPC.
        /// </summary>
        private void InternalSetState(eBuffBindAnimStackState state, bool force)
        {
            if (_disposed) return;
            if (!_stateMap.TryGetValue(state, out var targetState))
            {
                log.Warn($"[SquadStackFsm] SetState failed: state {state} not registered.");
                return;
            }

            var previousHead = _activeStates.First?.Value;
            var existingNode = FindActiveNode(state);

            // Reposition existing entry to respect priority ordering
            if (existingNode != null)
            {
                _activeStates.Remove(existingNode);
            }

            InsertByPriority(targetState);

            var newHead = _activeStates.First?.Value;

            // No change and not forced
            if (!force && previousHead != null && newHead != null && previousHead.StateId == newHead.StateId)
            {
                return;
            }

            log.Info($"[Squad Behavior][SquadStackFsm] SetState: {previousHead?.StateId.ToString() ?? "None"} -> {newHead?.StateId.ToString() ?? "None"} (Force:{force})");

            if (previousHead != null && previousHead != newHead)
            {
                log.Info($"[Squad Behavior][SquadStackFsm] Exiting Logic State: {previousHead.GetType().Name}");
                previousHead.OnExit();
            }

            if (newHead != null)
            {
                _currentLogicState = newHead;
                CurrentState = newHead.StateId;
                log.Info($"[Squad Behavior][SquadStackFsm] Entering Logic State: {_currentLogicState.GetType().Name}");
                _currentLogicState.OnEnter();
            }
            else
            {
                _currentLogicState = null;
                CurrentState = eBuffBindAnimStackState.NullStateID;
            }

            OnStateChanged?.Invoke(CurrentState);
        }

        private void InternalRemoveState(eBuffBindAnimStackState stateId)
        {
            if (_disposed) return;

            var node = FindActiveNode(stateId);
            if (node == null)
            {
                return;
            }

            var wasHead = _activeStates.First == node;
            var removedState = node.Value;

            if (wasHead)
            {
                removedState.OnExit();
            }

            removedState.OnRemove();
            _activeStates.Remove(node);

            var newHead = _activeStates.First?.Value;
            var previousState = CurrentState;

            if (newHead != null && (wasHead || _currentLogicState == removedState))
            {
                _currentLogicState = newHead;
                CurrentState = newHead.StateId;
                newHead.OnEnter();
            }
            else if (_activeStates.First == null)
            {
                _currentLogicState = null;
                CurrentState = eBuffBindAnimStackState.NullStateID;
            }

            if (previousState != CurrentState)
            {
                OnStateChanged?.Invoke(CurrentState);
            }
        }

        private void InsertByPriority(SquadStateBase state)
        {
            int targetPriority = GetPriority(state.StateId);
            var node = _activeStates.First;
            while (node != null && GetPriority(node.Value.StateId) > targetPriority)
            {
                node = node.Next;
            }

            if (node == null)
            {
                _activeStates.AddLast(state);
            }
            else
            {
                _activeStates.AddBefore(node, state);
            }
        }

        private System.Collections.Generic.LinkedListNode<SquadStateBase> FindActiveNode(eBuffBindAnimStackState stateId)
        {
            var node = _activeStates.First;
            while (node != null)
            {
                if (node.Value.StateId == stateId)
                {
                    return node;
                }
                node = node.Next;
            }

            return null;
        }

        private int GetPriority(eBuffBindAnimStackState stateId)
        {
            switch (stateId)
            {
                case eBuffBindAnimStackState.Die:
                    return 99;
                case eBuffBindAnimStackState.Victory:
                    return 80;
                case eBuffBindAnimStackState.Invincible:
                    return 70;
                // case eBuffBindAnimStackState.KnockUp:
                //     return 60;
                // case eBuffBindAnimStackState.Repulse:
                //     return 50;
                // case eBuffBindAnimStackState.Stunned:
                //     return 45;
                // case eBuffBindAnimStackState.GetHurt:
                //     return 40;
                // case eBuffBindAnimStackState.Disarmed:
                //     return 35;
                case eBuffBindAnimStackState.CastSkill:
                    return 30;
                case eBuffBindAnimStackState.NormalAttack:
                    return 25;
                case eBuffBindAnimStackState.Chasing:
                    return 20;
                case eBuffBindAnimStackState.Idle:
                    return 15;
                case eBuffBindAnimStackState.Born:
                    return 10;
                default:
                    return 0;
            }
        }

        public void OnUpdate()
        {
            if (_disposed) return;
            _currentLogicState?.OnUpdate();
        }

        public void Dispose()
        {
            if (_disposed) return;
            if (this is ISquadFsmHandler fsmHandler)
            {
                fsmHandler.ClearStates();
            }
            OnStateChanged = null;
            _disposed = true;
        }

        public class Factory : PlaceholderFactory<UnitClassType, SquadStackFsm>
        {
        }
    }
}
