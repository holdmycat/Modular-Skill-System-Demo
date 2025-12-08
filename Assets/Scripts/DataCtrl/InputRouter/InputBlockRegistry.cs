//------------------------------------------------------------
// File: InputBlockRegistry.cs
// Purpose: Allows systems to query global input block state without depending on Manager assembly.
//------------------------------------------------------------

namespace Ebonor.DataCtrl
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
