//------------------------------------------------------------
// File: HeroAttributesNode.cs
// Created: 2025-11-29
// Purpose: Graph node wrapper for hero attribute data.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using GraphProcessor;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Plugins.NodeEditor
{
    [System.Serializable]
    [NodeMenuItem("Unit Attributes/Hero Attributes", typeof(UnitAttributesDataGraph))]
    public class HeroAttributesNode: UnitAttributesNodeBase
    {
        public override bool isRenamable => true;
        public override bool needsInspector => true;
    
        [SerializeReference]
        private HeroAttributesNodeData heroAttributesNodeData = new HeroAttributesNodeData();
        public override UnitAttributesNodeDataBase UnitAttributesData_GetNodeData()
        {
            return heroAttributesNodeData;
        }
    }
    
}
