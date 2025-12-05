//------------------------------------------------------------
// File: NP_SupportSkillDataSupportor.cs
// Created: 2025-12-01
// Purpose: Support skill data container for behavior trees.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Support skill configuration data container.
    /// </summary>
    [BsonDeserializerRegister]
    public class NP_SupportSkillDataSupportor : NP_BaseDataSupportor
    {
        [Tooltip("Behavior tree ID for support skill.")]
        public long NPBehaveTreeDataId;
    }
}
