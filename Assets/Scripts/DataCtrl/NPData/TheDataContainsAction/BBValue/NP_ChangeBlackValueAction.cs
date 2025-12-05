//------------------------------------------------------------
// File: NP_ChangeBlackValueAction.cs
// Created: 2025-12-05
// Purpose: Action that writes a configured value to the current tree's blackboard.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Writes a blackboard value for the owning runtime tree.
    /// </summary>
    public class NP_ChangeBlackValueAction: NP_ClassForStoreAction
    {
        
        static readonly ILog log = LogManager.GetLogger(typeof(Blackboard));
        
        [Header("Blackboard Value To Write")]
        [Tooltip("Blackboard relation definition to be written to the current tree.")]
        public NP_BlackBoardRelationData NPBalckBoardRelationData = new NP_BlackBoardRelationData() { WriteOrCompareToBB = true };
        
        public override System.Action GetActionToBeDone()
        {
            this.Action = this.ChangeBlackBoard;
            return this.Action;
        }
        
        public void ChangeBlackBoard()
        {
            NPBalckBoardRelationData.SetBlackBoardValue(this.BelongtoRuntimeTree.GetBlackboard());
        }
    }
}
