//------------------------------------------------------------
// File: VTD_EventId.cs
// Created: 2025-12-05
// Purpose: Value container for event identifiers resolved from the canvas data store.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ebonor.DataCtrl
{
    public struct VTD_EventId
    {
        [Tooltip("Event identifier pulled from the canvas data manager.")]
        public string Value;

#if UNITY_EDITOR
        // Editor helper: fetch valid event IDs from the currently edited blackboard data manager.
        public IEnumerable<string> GetEventId()
        {
            if (NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager != null)
            {
                return NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager.EventValues;
            }

            return null;
        }
#endif
    }
}
