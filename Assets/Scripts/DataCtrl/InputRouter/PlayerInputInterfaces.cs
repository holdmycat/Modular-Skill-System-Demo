//------------------------------------------------------------
// File: PlayerInputInterfaces.cs
// Purpose: Input abstraction interfaces for gameplay (movement, UI, skills).
//------------------------------------------------------------
using UnityEngine;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Skill-trigger inputs.
    /// </summary>
    public interface ISkillInputSource
    {
        bool PrimaryAttackTriggered { get; }
        bool Skill1Triggered { get; }
        bool Skill2Triggered { get; }
        bool DashTriggered { get; }
    }

    /// <summary>
    /// Character control inputs.
    /// </summary>
    public interface IMovementInputSource
    {
        Vector2 MoveAxis { get; }
        bool JumpTriggered { get; }
        bool SprintHeld { get; }
    }

    /// <summary>
    /// UI interaction inputs.
    /// </summary>
    public interface IUiInputSource
    {
        bool OpenMenuTriggered { get; }
        bool ConfirmTriggered { get; }
        bool CancelTriggered { get; }
    }

    /// <summary>
    /// Combined player input surface for routing between devices.
    /// </summary>
    public interface IPlayerInputSource : ISkillInputSource, IMovementInputSource, IUiInputSource { }
}
