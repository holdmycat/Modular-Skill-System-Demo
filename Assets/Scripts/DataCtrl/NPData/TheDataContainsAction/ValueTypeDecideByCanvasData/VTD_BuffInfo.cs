//------------------------------------------------------------
// File: VTD_BuffInfo.cs
// Created: 2025-12-05
// Purpose: Value container for buff metadata used by canvas-driven actions.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using UnityEngine;

namespace Ebonor.DataCtrl
{

    public class VTD_BuffInfo
    {
        [Header("Buff Node Id")]
        [Tooltip("Identifier of the buff node associated with this entry.")]
        public VTD_Id BuffNodeId;

        [HideInInspector]
        public long BuffId;
        
        // Optional fields for layer configuration and model filters can be re-enabled and localized as needed.
    }
}
