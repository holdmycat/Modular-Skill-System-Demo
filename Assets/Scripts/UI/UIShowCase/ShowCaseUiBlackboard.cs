//------------------------------------------------------------
// File: ShowCaseUiBlackboard.cs
// Purpose: Read-only data surface injected into Showcase UIs.
//------------------------------------------------------------
using Ebonor.DataCtrl;
using UnityEngine;

namespace Ebonor.UI
{
    /// <summary>Data-only blackboard that Showcase UI panels can consume.</summary>
    public sealed class ShowCaseUiBlackboard : UiBlackboardBase
    {
        public ActorNumericComponentBase PlayerNumeric { get; private set; }

        /// <summary>Inject player numeric component.</summary>
        public void SetPlayerNumeric(ActorNumericComponentBase playerNumeric) => PlayerNumeric = playerNumeric;

        public override void Clear() => PlayerNumeric = null;
    }
}
