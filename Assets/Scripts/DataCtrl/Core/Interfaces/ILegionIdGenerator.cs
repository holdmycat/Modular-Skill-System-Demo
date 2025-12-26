//------------------------------------------------------------
// File: ILegionIdGenerator.cs
// Purpose: Interface for generating LegionIds (commanderNetId + local index).
//------------------------------------------------------------
namespace Ebonor.DataCtrl
{
    public interface ILegionIdGenerator
    {
        
        /// <summary>
        /// Returns the next LegionId for a commander, using a per-commander increment.
        /// </summary>
        ulong Next(uint commanderNetId);

        /// <summary>
        /// Deterministically composes a LegionId from commander id and local legion index.
        /// Layout: [commanderNetId (high 32 bits)][localLegionIndex (low 32 bits)].
        /// </summary>
        ulong Compose(uint commanderNetId, uint localLegionIndex);

        /// <summary>
        /// Decomposes a LegionId back into commander id and local legion index.
        /// </summary>
        (uint commanderNetId, uint localLegionIndex) Decompose(ulong legionId);
    }
}
