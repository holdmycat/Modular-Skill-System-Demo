//------------------------------------------------------------
// File: NP_BlackBoardDataManager.cs
// Created: 2025-12-05
// Purpose: Editor-only container for blackboard, event, and ID mappings used by NP canvases.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

namespace Ebonor.DataCtrl
{
#if UNITY_EDITOR
    // TODO: Consider consolidating all data here to separate logic data during editing and support import/export with Excel and the node editor.
    /// <summary>
    /// Canvas data manager for editor use, including blackboard, event, and ID mappings.
    /// </summary>
    public class NP_BlackBoardDataManager
    {
        [Header("Blackboard Entries")]
        [Tooltip("All blackboard entries for this NPBehave canvas. Key: string, Value: NP_BBValue subtype.")]
        public Dictionary<string, ANP_BBValue> BBValues = new Dictionary<string, ANP_BBValue>();
        
        [Header("Event Names")]
        [Tooltip("All event names referenced by this NPBehave canvas.")]
        public List<string> EventValues = new List<string>();
        
        [Header("Id Mappings")]
        [Tooltip("Mapping of ID descriptions to their numeric values.")]
        public Dictionary<string,long> Ids = new Dictionary<string, long>();

#if UNITY_EDITOR
        /// <summary>
        /// Set by the GraphEditor when the Blackboard button is clicked.
        /// </summary>
        public static NP_BlackBoardDataManager CurrentEditedNP_BlackBoardDataManager;
#endif
    }
#endif
}
