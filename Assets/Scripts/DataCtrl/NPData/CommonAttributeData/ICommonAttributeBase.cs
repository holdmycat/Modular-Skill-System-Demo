//------------------------------------------------------------
// File: ICommonAttributeBase.cs
// Created: 2025-11-29
// Purpose: Interface contract for attribute node data with discriminator property.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using MongoDB.Bson.Serialization.Attributes;

namespace Ebonor.DataCtrl
{
    [BsonDeserializerRegister]
    public interface ICommonAttributeBase
    {
        /// <summary>
        /// Combine identifying fields (e.g., profession + sprite) into a single role key string.
        /// </summary>
        /// <returns>Role key used to produce a deterministic role id.</returns>
        string BuildRoleKey();

        /// <summary>
        /// Generate a deterministic role id from the role key.
        /// </summary>
        /// <returns>Generated role id.</returns>
        long GenerateRoleIdFromData();

        /// <summary>
        /// Hook for committing in-memory changes (e.g., saving or marking dirty).
        /// </summary>
        void CommitRoleChanges();
    }
}
