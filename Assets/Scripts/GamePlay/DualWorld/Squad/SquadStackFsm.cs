using System;
using Ebonor.DataCtrl;
using Zenject;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Lightweight stack-style animation state machine for squads, keyed by UnitClassType.
    /// </summary>
    public class SquadStackFsm
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
        public void SetState(eBuffBindAnimStackState state, bool force = false)
        {
            if (!force && CurrentState == state) return;

            CurrentState = state;
            OnStateChanged?.Invoke(state);
        }

        public class Factory : PlaceholderFactory<UnitClassType, SquadStackFsm>
        {
        }
    }
}
