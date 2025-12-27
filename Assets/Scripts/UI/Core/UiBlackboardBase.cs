//------------------------------------------------------------
// File: UiBlackboardBase.cs
// Purpose: Base class for UI data blackboards (MonoBehaviour).
//------------------------------------------------------------
using UnityEngine;

namespace Ebonor.UI
{
    /// <summary>Base MonoBehaviour for UI data-only blackboards.</summary>
    public abstract class UIBlackboardBase : MonoBehaviour
    {
        /// <summary>Optional hook for clearing injected context.</summary>
        public virtual void Clear() { }
    }
}
