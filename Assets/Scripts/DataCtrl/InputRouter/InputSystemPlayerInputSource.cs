//------------------------------------------------------------
// File: InputSystemPlayerInputSource.cs
// Purpose: Unity Input System-backed implementation of IPlayerInputSource.
//------------------------------------------------------------
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Bridges Unity Input System actions to the game's input routing surface.
    /// </summary>
    public class InputSystemPlayerInputSource : MonoBehaviour, IPlayerInputSource
    {
        [Header("Source")]
        [SerializeField] private PlayerInput playerInput;

        [Header("Action Names (fallback if reference not assigned)")]
        [SerializeField] private string moveActionName = "Move";
        [SerializeField] private string primaryAttackActionName = "CastSkill1";
        [SerializeField] private string skill1ActionName = "CastSkill1";
        [SerializeField] private string skill2ActionName = "CastSkill2";
        [SerializeField] private string dashActionName = "CastDash";
        [SerializeField] private string jumpActionName = "Jump";
        [SerializeField] private string sprintActionName = "Sprint";
        [SerializeField] private string openMenuActionName = "OpenMenu";
        [SerializeField] private string confirmActionName = "Submit";
        [SerializeField] private string cancelActionName = "Cancel";

        private InputAction _moveAction;
        private InputAction _primaryAction;
        private InputAction _skill1Action;
        private InputAction _skill2Action;
        private InputAction _dashAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;
        private InputAction _openMenuAction;
        private InputAction _confirmAction;
        private InputAction _cancelAction;

        private Vector2 _moveAxis;
        private bool _sprintHeld;

        private bool _primaryTriggered;
        private int _primaryFrame;

        private bool _skill1Triggered;
        private int _skill1Frame;

        private bool _skill2Triggered;
        private int _skill2Frame;

        private bool _dashTriggered;
        private int _dashFrame;

        private bool _jumpTriggered;
        private int _jumpFrame;

        private bool _openMenuTriggered;
        private int _openMenuFrame;

        private bool _confirmTriggered;
        private int _confirmFrame;

        private bool _cancelTriggered;
        private int _cancelFrame;

        private void Awake()
        {
            if (playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
            }
        }

        private void OnEnable()
        {
            CacheActions();

            EnableAction(_moveAction, OnMovePerformed, OnMoveCanceled);
            EnableAction(_sprintAction, OnSprintPerformed, OnSprintCanceled);
            EnableAction(_jumpAction, OnJumpPerformed);
            EnableAction(_primaryAction, OnPrimaryPerformed);
            EnableAction(_skill1Action, OnSkill1Performed);
            EnableAction(_skill2Action, OnSkill2Performed);
            EnableAction(_dashAction, OnDashPerformed);
            EnableAction(_openMenuAction, OnOpenMenuPerformed);
            EnableAction(_confirmAction, OnConfirmPerformed);
            EnableAction(_cancelAction, OnCancelPerformed);
        }

        private void OnDisable()
        {
            DisableAction(_moveAction, OnMovePerformed, OnMoveCanceled);
            DisableAction(_sprintAction, OnSprintPerformed, OnSprintCanceled);
            DisableAction(_jumpAction, OnJumpPerformed);
            DisableAction(_primaryAction, OnPrimaryPerformed);
            DisableAction(_skill1Action, OnSkill1Performed);
            DisableAction(_skill2Action, OnSkill2Performed);
            DisableAction(_dashAction, OnDashPerformed);
            DisableAction(_openMenuAction, OnOpenMenuPerformed);
            DisableAction(_confirmAction, OnConfirmPerformed);
            DisableAction(_cancelAction, OnCancelPerformed);

            _moveAxis = Vector2.zero;
            _sprintHeld = false;
        }

        // IMovementInputSource
        public Vector2 MoveAxis => _moveAxis;
        public bool JumpTriggered => Consume(ref _jumpTriggered, ref _jumpFrame);
        public bool SprintHeld => _sprintHeld;

        // ISkillInputSource
        public bool PrimaryAttackTriggered => Consume(ref _primaryTriggered, ref _primaryFrame);
        public bool Skill1Triggered => Consume(ref _skill1Triggered, ref _skill1Frame);
        public bool Skill2Triggered => Consume(ref _skill2Triggered, ref _skill2Frame);
        public bool DashTriggered => Consume(ref _dashTriggered, ref _dashFrame);

        // IUiInputSource
        public bool OpenMenuTriggered => Consume(ref _openMenuTriggered, ref _openMenuFrame);
        public bool ConfirmTriggered => Consume(ref _confirmTriggered, ref _confirmFrame);
        public bool CancelTriggered => Consume(ref _cancelTriggered, ref _cancelFrame);

        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            _moveAxis = ctx.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            _moveAxis = Vector2.zero;
        }

        private void OnSprintPerformed(InputAction.CallbackContext ctx)
        {
            _sprintHeld = true;
        }

        private void OnSprintCanceled(InputAction.CallbackContext ctx)
        {
            _sprintHeld = false;
        }

        private void OnJumpPerformed(InputAction.CallbackContext ctx) => Flag(ref _jumpTriggered, ref _jumpFrame);
        private void OnPrimaryPerformed(InputAction.CallbackContext ctx) => Flag(ref _primaryTriggered, ref _primaryFrame);
        private void OnSkill1Performed(InputAction.CallbackContext ctx) => Flag(ref _skill1Triggered, ref _skill1Frame);
        private void OnSkill2Performed(InputAction.CallbackContext ctx) => Flag(ref _skill2Triggered, ref _skill2Frame);
        private void OnDashPerformed(InputAction.CallbackContext ctx) => Flag(ref _dashTriggered, ref _dashFrame);
        private void OnOpenMenuPerformed(InputAction.CallbackContext ctx) => Flag(ref _openMenuTriggered, ref _openMenuFrame);
        private void OnConfirmPerformed(InputAction.CallbackContext ctx) => Flag(ref _confirmTriggered, ref _confirmFrame);
        private void OnCancelPerformed(InputAction.CallbackContext ctx) => Flag(ref _cancelTriggered, ref _cancelFrame);

        private void CacheActions()
        {
            _moveAction = ResolveAction(moveActionName);
            _primaryAction = ResolveAction(primaryAttackActionName);
            _skill1Action = ResolveAction(skill1ActionName);
            _skill2Action = ResolveAction(skill2ActionName);
            _dashAction = ResolveAction(dashActionName);
            _jumpAction = ResolveAction(jumpActionName);
            _sprintAction = ResolveAction(sprintActionName);
            _openMenuAction = ResolveAction(openMenuActionName);
            _confirmAction = ResolveAction(confirmActionName);
            _cancelAction = ResolveAction(cancelActionName);
        }

        private InputAction ResolveAction(string actionName)
        {
            if (playerInput != null && playerInput.actions != null && !string.IsNullOrEmpty(actionName))
            {
                return playerInput.actions.FindAction(actionName, false);
            }

            return null;
        }

        private static void EnableAction(InputAction action, System.Action<InputAction.CallbackContext> performed, System.Action<InputAction.CallbackContext> canceled = null)
        {
            if (action == null) return;
            if (performed != null) action.performed += performed;
            if (canceled != null) action.canceled += canceled;
            action.Enable();
        }

        private static void DisableAction(InputAction action, System.Action<InputAction.CallbackContext> performed, System.Action<InputAction.CallbackContext> canceled = null)
        {
            if (action == null) return;
            if (performed != null) action.performed -= performed;
            if (canceled != null) action.canceled -= canceled;
            action.Disable();
        }

        private static void Flag(ref bool flag, ref int frame)
        {
            flag = true;
            frame = Time.frameCount;
        }

        private static bool Consume(ref bool flag, ref int frame)
        {
            bool active = flag && frame == Time.frameCount;
            flag = false;
            return active;
        }
    }
}
