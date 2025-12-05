//------------------------------------------------------------
// File: VTD_Id.cs
// Created: 2025-12-05
// Purpose: Value container for node IDs resolved from the canvas data store.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ebonor.DataCtrl
{
    public struct VTD_Id
    {
        public static void ModifyStruct(ref VTD_Id myStruct, string key, long value)
        {
            myStruct.IdKey = key;
            myStruct.Value = value;
        }
        
        [Tooltip("Key of this node ID inside the canvas data manager.")]
        [BsonIgnore]
        public string IdKey;
        
        [Tooltip("Resolved ID value pulled from the canvas data manager.")]
        [BsonIgnore]
        public long Value;

#if UNITY_EDITOR
        // Editor helper: fetch valid keys from the currently edited blackboard data manager.
        public IEnumerable<string> GetIdKey()
        {
            if (NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager != null)
            {
                return NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager.Ids.Keys;
            }

            return null;
        }

        // Editor helper: apply the selected key to resolve the ID value.
        public void ApplyId()
        {
            if (NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager != null)
            {
                if (NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager.Ids.TryGetValue(IdKey, out var targetId))
                {
                    Value = targetId;
                }
            }
        }
#endif
    }
}
