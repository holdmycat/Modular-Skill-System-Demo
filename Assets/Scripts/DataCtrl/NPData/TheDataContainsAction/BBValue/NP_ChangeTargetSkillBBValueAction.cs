//------------------------------------------------------------
// File: NP_ChangeTargetSkillBBValueAction.cs
// Created: 2025-12-05
// Purpose: Action that writes a blackboard value to a target skill's runtime tree.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    public enum ValueGetType : byte
    {
        [InspectorName("From Configuration Data")]
        FromDataSet,

        [InspectorName("From Blackboard")]
        FromBBValue,
    }

    /// <summary>
    /// Update a target skill's blackboard value.
    /// </summary>
    public class NP_ChangeTargetSkillBBValueAction : NP_ClassForStoreAction
    {
        [Header("Target Selection")]
        [Tooltip(
            "When true, targets the unit that owns this runtime tree. When false, uses TargetUnitId to locate the owning hero.")]
        public bool TargetUnitIsSelf = true;

        [Tooltip("Hero Unit Id that owns the target skill (used when TargetUnitIsSelf is false).")]
        public NP_BlackBoardRelationData TargetUnitId = new NP_BlackBoardRelationData();

        [Header("Target Skill Id")]
        public VTD_Id TargetSkillId = new VTD_Id();

        [Header("Blackboard Value To Pass")]
        public NP_BlackBoardRelationData NPBBValue_ValueToChange = new NP_BlackBoardRelationData();

        [Tooltip("Choose whether to pull the value from configuration data or from the current blackboard.")]
        public ValueGetType ValueGetType = ValueGetType.FromBBValue;

        public override System.Action GetActionToBeDone()
        {
            this.Action = this.ChangeTargetSkillBBValue;
            return this.Action;
        }

        public void ChangeTargetSkillBBValue()
        {
            // Logic remains commented until target skill routing is implemented.
            // Unit targetUnit = null;
            //
            // if (this.TargetUnitIsSelf)
            // {
            //     targetUnit = BelongToUnit;
            // }
            // else
            // {
            //     targetUnit = BelongToUnit.BelongToRoom.GetComponent<UnitComponent>()
            //         .Get(this.TargetUnitId.GetBlackBoardValue<long>(this.BelongtoRuntimeTree.GetBlackboard()));
            // }
            //
            // List<NP_RuntimeTree> skillContent = targetUnit.GetComponent<SkillCanvasManagerComponent>()
            //     .GetSkillCanvas(this.TargetSkillId.Value);
            //
            // foreach (var skillCanvas in skillContent)
            // {
            //     // Skip the source runtime tree.
            //     if (skillCanvas == this.BelongtoRuntimeTree)
            //     {
            //         continue;
            //     }
            //
            //     if (this.ValueGetType == ValueGetType.FromDataSet)
            //     {
            //         this.NPBBValue_ValueToChange.SetBlackBoardValue(skillCanvas.GetBlackboard());
            //     }
            //     else
            //     {
            //         this.NPBBValue_ValueToChange.SetBBValueFromThisBBValue(this.BelongtoRuntimeTree.GetBlackboard(),
            //             skillCanvas.GetBlackboard());
            //     }
            // }
        }

#if UNITY_EDITOR
        private void ApplyDataGetType()
        {
            if (this.ValueGetType == ValueGetType.FromDataSet)
            {
                NPBBValue_ValueToChange.WriteOrCompareToBB = true;
            }
            else
            {
                NPBBValue_ValueToChange.WriteOrCompareToBB = false;
            }
        }
#endif
    }
}
