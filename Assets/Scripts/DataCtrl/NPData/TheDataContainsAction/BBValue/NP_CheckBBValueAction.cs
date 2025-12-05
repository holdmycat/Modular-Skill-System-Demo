//------------------------------------------------------------
// File: NP_CheckBBValueAction.cs
// Created: 2025-12-05
// Purpose: Action that compares a preset value against a blackboard entry with various operators.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;

namespace Plugins.NodeEditor
{
    public class NP_CheckBBValueAction: NP_ClassForStoreAction
    {
        
        static readonly ILog log = LogManager.GetLogger(typeof(NP_CheckBBValueAction));
        
        [Header("Comparison Settings")]
        [Tooltip("Operator used to compare the preset value with the blackboard value.")]
        public Operator Ope = Operator.IS_EQUAL;

        [Tooltip("Blackboard relation and value to compare against.")]
        public NP_BlackBoardRelationData NpBlackBoardRelationData = new NP_BlackBoardRelationData() { WriteOrCompareToBB = true };

        public override Func<bool> GetFuncToBeDone()
        {
            this.Func = this.CheckBBValue;
            return this.Func;
        }

        private bool CheckBBValue()
        {
            if (Ope == Operator.ALWAYS_TRUE)
            {
                return true;
            }

            string key = NpBlackBoardRelationData.BBKey;
            Blackboard selfBlackboard = this.BelongtoRuntimeTree.GetBlackboard();
            if (!selfBlackboard.Isset(key))
            {
                return Ope == Operator.IS_NOT_SET;
            }

            ANP_BBValue preSetValue = this.NpBlackBoardRelationData.NP_BBValue;
            ANP_BBValue bbValue = (ANP_BBValue)selfBlackboard.Get(key);

            switch (this.Ope)
            {
                case Operator.IS_SET: return true;
                case Operator.IS_EQUAL:
                {
                    switch (preSetValue)
                    {
                        case NP_BBValue_Bool npBbValue:
                            return npBbValue == bbValue as NP_BBValue_Bool;
                        case NP_BBValue_Float npBbValue:
                            return npBbValue == bbValue as NP_BBValue_Float;
                        case NP_BBValue_Int npBbValue:
                            return npBbValue == bbValue as NP_BBValue_Int;
                        case NP_BBValue_String npBbValue:
                            return npBbValue == bbValue as NP_BBValue_String;
                        case NP_BBValue_Vector3 npBbValue:
                            return npBbValue == bbValue as NP_BBValue_Vector3;
                        case NP_BBValue_Long npBbValue:
                            return npBbValue == bbValue as NP_BBValue_Long;
                        default:
                            log.Error($"Type {preSetValue.GetType()} is not registered as NP_BBValue");
                            return false;
                    }
                }
                case Operator.IS_NOT_EQUAL:
                {
                    switch (preSetValue)
                    {
                        case NP_BBValue_Bool npBbValue:
                            return npBbValue != bbValue as NP_BBValue_Bool;
                        case NP_BBValue_Float npBbValue:
                            return npBbValue != bbValue as NP_BBValue_Float;
                        case NP_BBValue_Int npBbValue:
                            return npBbValue != bbValue as NP_BBValue_Int;
                        case NP_BBValue_String npBbValue:
                            return npBbValue != bbValue as NP_BBValue_String;
                        case NP_BBValue_Long npBbValue:
                            return npBbValue != bbValue as NP_BBValue_Long;
                        case NP_BBValue_Vector3 npBbValue:
                            return npBbValue != bbValue as NP_BBValue_Vector3;
                        default:
                            log.Error($"Type {preSetValue.GetType()} is not registered as NP_BBValue");
                            return false;
                    }
                }

                case Operator.IS_GREATER_OR_EQUAL:
                {
                    switch (preSetValue)
                    {
                        case NP_BBValue_Bool npBbValue:
                            return (bbValue as NP_BBValue_Bool) >= npBbValue;
                        case NP_BBValue_Float npBbValue:
                            return (bbValue as NP_BBValue_Float) >= npBbValue;
                        case NP_BBValue_Int npBbValue:
                            return (bbValue as NP_BBValue_Int) >= npBbValue;
                        case NP_BBValue_String npBbValue:
                            return (bbValue as NP_BBValue_String) >= npBbValue;
                        case NP_BBValue_Long npBbValue:
                            return (bbValue as NP_BBValue_Long) >= npBbValue;
                        case NP_BBValue_Vector3 npBbValue:
                            return (bbValue as NP_BBValue_Vector3) >= npBbValue;
                        default:
                            log.Error($"Type {preSetValue.GetType()} is not registered as NP_BBValue");
                            return false;
                    }
                }

                case Operator.IS_GREATER:
                {
                    switch (preSetValue)
                    {
                        case NP_BBValue_Bool npBbValue:
                            return (bbValue as NP_BBValue_Bool) > npBbValue;
                        case NP_BBValue_Float npBbValue:
                            return (bbValue as NP_BBValue_Float) > npBbValue;
                        case NP_BBValue_Int npBbValue:
                            return (bbValue as NP_BBValue_Int) > npBbValue;
                        case NP_BBValue_String npBbValue:
                            return (bbValue as NP_BBValue_String) > npBbValue;
                        case NP_BBValue_Long npBbValue:
                            return (bbValue as NP_BBValue_Long) > npBbValue;
                        case NP_BBValue_Vector3 npBbValue:
                            return (bbValue as NP_BBValue_Vector3) > npBbValue;
                        default:
                            log.Error($"Type {preSetValue.GetType()} is not registered as NP_BBValue");
                            return false;
                    }
                }

                case Operator.IS_SMALLER_OR_EQUAL:
                    switch (preSetValue)
                    {
                        case NP_BBValue_Bool npBbValue:
                            return (bbValue as NP_BBValue_Bool) <= npBbValue;
                        case NP_BBValue_Float npBbValue:
                            return (bbValue as NP_BBValue_Float) <= npBbValue;
                        case NP_BBValue_Int npBbValue:
                            return (bbValue as NP_BBValue_Int) <= npBbValue;
                        case NP_BBValue_String npBbValue:
                            return (bbValue as NP_BBValue_String) <= npBbValue;
                        case NP_BBValue_Long npBbValue:
                            return (bbValue as NP_BBValue_Long) <= npBbValue;
                        case NP_BBValue_Vector3 npBbValue:
                            return (bbValue as NP_BBValue_Vector3) <= npBbValue;
                        default:
                            log.Error($"Type {preSetValue.GetType()} is not registered as NP_BBValue");
                            return false;
                    }
                case Operator.IS_SMALLER:
                    switch (preSetValue)
                    {
                        case NP_BBValue_Bool npBbValue:
                            return (bbValue as NP_BBValue_Bool) < npBbValue;
                        case NP_BBValue_Float npBbValue:
                            return (bbValue as NP_BBValue_Float) < npBbValue;
                        case NP_BBValue_Int npBbValue:
                            return (bbValue as NP_BBValue_Int) < npBbValue;
                        case NP_BBValue_String npBbValue:
                            return (bbValue as NP_BBValue_String) < npBbValue;
                        case NP_BBValue_Long npBbValue:
                            return (bbValue as NP_BBValue_Long) < npBbValue;
                        case NP_BBValue_Vector3 npBbValue:
                            return (bbValue as NP_BBValue_Vector3) < npBbValue;
                        default:
                            log.Error($"Type {preSetValue.GetType()} is not registered as NP_BBValue");
                            return false;
                    }

                default: return false;
            }
        }
    }
}
