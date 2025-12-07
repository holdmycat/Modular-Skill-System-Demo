//------------------------------------------------------------
// File: InputBlockRegistry.cs
// Purpose: Allows systems to query global input block state without depending on Manager assembly.
//------------------------------------------------------------
using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    public interface IInputBlockSource
    {
        bool IsBlocked(eInputControlFlag flag);
        eInputControlFlag BlockedFlags { get; }
    }

    /// <summary>
    /// Registry for the active input block source (set by manager layer).
    /// </summary>
    public static class InputBlockRegistry
    {
        public static IInputBlockSource Source { get; set; }
    }
}
