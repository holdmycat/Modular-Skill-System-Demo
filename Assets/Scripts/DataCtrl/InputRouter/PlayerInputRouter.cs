//------------------------------------------------------------
// File: PlayerInputRouter.cs
// Purpose: Central point to swap between different input sources (keyboard, gamepad, etc.).
//------------------------------------------------------------
using UnityEngine;
using Ebonor.DataCtrl;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Stores the active input source so gameplay systems can read from one surface.
    /// </summary>
    public class PlayerInputRouter : MonoBehaviour, IPlayerInputSource
    {
        // Managed by GameClientManager, no self-singleton.
        
        public void OnUpdate(float deltaTime)
        {
            // Update logic if needed (currently input is event/poll based, but good to have the hook)
        }

        public void Exit()
        {
            // Cleanup logic
        }

        private IPlayerInputSource _current;
        
        [Header("Enable Flags")]
        [SerializeField] private eInputControlFlag controlFlags = eInputControlFlag.All;

        /// <summary>Global toggle state (read-only). Use SetInputEnabled to change.</summary>
        public bool InputEnabled => (controlFlags & ~GetBlockedFlags()) != eInputControlFlag.None;
        /// <summary>Skill input state (read-only). Use SetSkillsEnabled to change.</summary>
        public bool SkillsEnabled => (controlFlags & eInputControlFlag.Skills) != 0 && !IsBlocked(eInputControlFlag.Skills);
        /// <summary>Movement input state (read-only). Use SetMovementEnabled to change.</summary>
        public bool MovementEnabled => (controlFlags & eInputControlFlag.Movement) != 0 && !IsBlocked(eInputControlFlag.Movement);
        /// <summary>UI input state (read-only). Use SetUiEnabled to change.</summary>
        public bool UiEnabled => (controlFlags & eInputControlFlag.Ui) != 0 && !IsBlocked(eInputControlFlag.Ui);

        /// <summary>Set all group toggles from a flag mask (overwrites previous state).</summary>
        private void ApplyControlFlags(eInputControlFlag flags)
        {
            controlFlags = flags;
        }
        
        public void InitPlayerInputRouter(IPlayerInputSource source)
        {
            if (_current != null)
            {
                return;
            }
  
            _current = source;

            // Initialize booleans from the flag mask for consistency.
            ApplyControlFlags(controlFlags);
        }

        public void EnableAllInputControlFlags()
        {
            ApplyControlFlags(eInputControlFlag.All);
        }
        
        public void DisableAllInputControlFlags()
        {
            ApplyControlFlags(eInputControlFlag.None);
        }
        
        public void SetInputEnabled(bool enabled) => controlFlags = enabled ? eInputControlFlag.All : eInputControlFlag.None;
        public void SetSkillsEnabled(bool enabled) => controlFlags = enabled ? controlFlags | eInputControlFlag.Skills : controlFlags & ~eInputControlFlag.Skills;
        public void SetMovementEnabled(bool enabled) => controlFlags = enabled ? controlFlags | eInputControlFlag.Movement : controlFlags & ~eInputControlFlag.Movement;
        public void SetUiEnabled(bool enabled) => controlFlags = enabled ? controlFlags | eInputControlFlag.Ui : controlFlags & ~eInputControlFlag.Ui;

        public void SetInputSource(IPlayerInputSource source)
        {
            _current = source;
        }

        private static bool IsBlocked(eInputControlFlag flag)
        {
            var src = InputBlockRegistry.Source;
            return src != null && src.IsBlocked(flag);
        }

        private static eInputControlFlag GetBlockedFlags()
        {
            var src = InputBlockRegistry.Source;
            return src != null ? src.BlockedFlags : eInputControlFlag.None;
        }

        // ISkillInputSource
        public bool PrimaryAttackTriggered => SkillsEnabled && _current != null && _current.PrimaryAttackTriggered;
        public bool Skill1Triggered => SkillsEnabled && _current != null && _current.Skill1Triggered;
        public bool Skill2Triggered => SkillsEnabled && _current != null && _current.Skill2Triggered;
        public bool DashTriggered => SkillsEnabled && _current != null && _current.DashTriggered;

        // IMovementInputSource
        public Vector2 MoveAxis => MovementEnabled && _current != null ? _current.MoveAxis : Vector2.zero;
        public bool JumpTriggered => MovementEnabled && _current != null && _current.JumpTriggered;
        public bool SprintHeld => MovementEnabled && _current != null && _current.SprintHeld;

        // IUiInputSource
        public bool OpenMenuTriggered => UiEnabled && _current != null && _current.OpenMenuTriggered;
        public bool ConfirmTriggered => UiEnabled && _current != null && _current.ConfirmTriggered;
        public bool CancelTriggered => UiEnabled && _current != null && _current.CancelTriggered;
    }
}
