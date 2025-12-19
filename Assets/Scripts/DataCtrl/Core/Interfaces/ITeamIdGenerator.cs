//------------------------------------------------------------
// File: ITeamIdGenerator.cs
// Purpose: Interface for generating deterministic team ids.
//------------------------------------------------------------
namespace Ebonor.DataCtrl
{
    public interface ITeamIdGenerator
    {
        /// <summary>
        /// Generates a deterministic numeric team id based on the provided components.
        /// </summary>
        /// <param name="components">Key parts describing team usage and context.</param>
        /// <returns>Non-negative team id.</returns>
        long GenerateTeamId(TeamIdComponents components);

        /// <summary>
        /// Builds the string key used for hashing (useful for debugging/interop).
        /// </summary>
        string BuildTeamKey(TeamIdComponents components);
    }
}
