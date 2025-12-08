//------------------------------------------------------------
// File: KeyboardPlayerInputSource.cs
// Purpose: Keyboard/mouse implementation of gameplay input (movement, UI, skills).
//------------------------------------------------------------
using UnityEngine;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Keyboard + mouse input source. Swap out via PlayerInputRouter when new devices are added.
    /// </summary>
    public class KeyboardPlayerInputSource : MonoBehaviour, IPlayerInputSource
    {
        [Header("Movement axes")]
        [SerializeField] private string horizontalAxis = "Horizontal";
        [SerializeField] private string verticalAxis = "Vertical";

        public Vector2 MoveAxis => new Vector2(Input.GetAxisRaw(horizontalAxis), Input.GetAxisRaw(verticalAxis));
        public bool JumpTriggered => Input.GetKeyDown(KeyCode.Space);
        public bool SprintHeld => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        public bool PrimaryAttackTriggered => Input.GetMouseButtonDown(0);
        public bool Skill1Triggered => Input.GetKeyDown(KeyCode.J);
        public bool Skill2Triggered => Input.GetKeyDown(KeyCode.K);
        public bool DashTriggered => Input.GetKeyDown(KeyCode.L);
        
        public bool OpenMenuTriggered => Input.GetKeyDown(KeyCode.Escape);
        public bool ConfirmTriggered => Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.LeftControl);
        public bool CancelTriggered => Input.GetKeyDown(KeyCode.Backspace);
    }
}
