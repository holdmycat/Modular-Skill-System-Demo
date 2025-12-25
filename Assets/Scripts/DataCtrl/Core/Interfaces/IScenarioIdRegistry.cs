//------------------------------------------------------------
// File: IScenarioIdRegistry.cs
// Purpose: Normalizes and validates scenario ids.
//------------------------------------------------------------
namespace Ebonor.DataCtrl
{
    public interface IScenarioIdRegistry
    {
        /// <summary>Normalize a scenario id (trim, replace spaces, lower-case).</summary>
        string Normalize(string scenarioId);

        /// <summary>Returns true if the normalized id is registered.</summary>
        bool IsRegistered(string normalizedId);
    }
}
