//------------------------------------------------------------
// File: NP_BBValueHelper.cs
// Created: 2025-12-05
// Purpose: Utilities for creating, copying, and comparing NP blackboard values.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using Ebonor.Framework;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Ebonor.DataCtrl
{
    public static class NP_BBValueHelper
    {
        
        static readonly ILog log = LogManager.GetLogger(typeof(NP_BBValueHelper));
        
        /// <summary>
        /// Set a target blackboard value using an ANP_BBValue wrapper.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="blackboard"></param>
        /// <param name="key"></param>
        public static void SetTargetBlackboardUseANP_BBValue(ANP_BBValue anpBbValue, Blackboard blackboard, string key)
        {
            // Use ToString() to identify the full type; Name would be abbreviated.
            
            switch (anpBbValue.NP_BBValueType.ToString())
            {
                case "System.String":
                    blackboard.Set(key, (anpBbValue as NP_BBValue_String).GetValue());
                    break;
                case "System.Single":
                    blackboard.Set(key, (anpBbValue as NP_BBValue_Float).GetValue());
                    break;
                case "System.Int32":
                    blackboard.Set(key, (anpBbValue as NP_BBValue_Int).GetValue());
                    break;
                case "System.Int64":
                    blackboard.Set(key, (anpBbValue as NP_BBValue_Long).GetValue());
                    break;
                case "System.UInt32":
                    blackboard.Set(key, (anpBbValue as NP_BBValue_UInt).GetValue());
                    break;
                case "System.Boolean":
                   
                    blackboard.Set(key, (anpBbValue as NP_BBValue_Bool).GetValue());
                    break;
                case "System.Collections.Generic.List`1[System.Int64]":
                    blackboard.Set(key, (anpBbValue as NP_BBValue_List_Long).GetValue());
                    break;
                case "System.Numerics.Vector3":
                    blackboard.Set(key, (anpBbValue as NP_BBValue_Vector3).GetValue());
                    break;
            }
        }

        /// <summary>
        /// Automatically create an NP_BBValue from a generic value.
        /// </summary>
        public static ANP_BBValue AutoCreateNPBBValueFromTValue<T>(T value)
        {
            string valueType = typeof(T).ToString();
            object boxedValue = value;
            ANP_BBValue anpBbValue = null;
            switch (valueType)
            {
                case "System.String":
                    anpBbValue = new NP_BBValue_String() {Value = boxedValue as string};
                    break;
                case "System.Single":
                    anpBbValue = new NP_BBValue_Float() {Value = (float) boxedValue};
                    break;
                case "System.Int32":
                    anpBbValue = new NP_BBValue_Int() {Value = (int) boxedValue};
                    break;
                case "System.Int64":
                    anpBbValue = new NP_BBValue_Long() {Value = (long) boxedValue};
                    break;
                case "System.UInt32":
                    anpBbValue = new NP_BBValue_UInt() {Value = (uint) boxedValue};
                    break;
                case "System.Boolean":
                    anpBbValue = new NP_BBValue_Bool() {Value = (bool) boxedValue};
                    break;
                case "System.Collections.Generic.List`1[System.Int64]":
                    // List is a reference type; copy elements explicitly for safety.
                    NP_BBValue_List_Long list = new NP_BBValue_List_Long();
                    
                    INP_BBValue<List<long>> tmp = (INP_BBValue<List<long>>)boxedValue;
                    
                    list.SetValueFrom(tmp);
                    
                    anpBbValue = list;
                    
                    break;
                case "System.Numerics.Vector3":
                    anpBbValue = new NP_BBValue_Vector3() {Value = (Vector3) boxedValue};
                    break;
            }
            
            return anpBbValue;
        }


        /// <summary>
        /// Copy data from anpBbValue into self.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="anpBbValue"></param>
        public static void SetValueFrom(in ANP_BBValue self, ANP_BBValue anpBbValue)
        {
            if (anpBbValue == null)
            {
                log.Error("anpBbValue is null");
                return;
            }

            if (self.NP_BBValueType != anpBbValue.NP_BBValueType)
            {
                log.Error(
                    $"Cannot copy NP_BBValue instances of different types. Self: {self.NP_BBValueType} anpBbValue: {anpBbValue.NP_BBValueType}");
            }

            self.SetValueFrom(anpBbValue);
        }

        public static bool Compare(ANP_BBValue lhs, ANP_BBValue rhs, Operator op)
        {
            if (lhs == null)
            {
                log.Error("Compare failed: lhs is null");
                return false;
            }

            // For operators that rely on rhs, guard against null
            bool rhsRequired = op != Operator.IS_SET && op != Operator.ALWAYS_TRUE;
            if (rhsRequired && rhs == null)
            {
                log.Error($"Compare failed: rhs is null for operator {op} and lhs type {lhs.GetType().Name}");
                return false;
            }

            switch (op)
            {
                case Operator.IS_SET: return true;
                case Operator.IS_EQUAL:
                {
                    switch (lhs)
                    {
                        case NP_BBValue_Bool npBbValue:
                            return npBbValue == rhs as NP_BBValue_Bool;
                        case NP_BBValue_Float npBbValue:
                            return npBbValue == rhs as NP_BBValue_Float;
                        case NP_BBValue_UInt npBbValue:
                            return (rhs as NP_BBValue_UInt) == npBbValue;
                        case NP_BBValue_Int npBbValue:
                            return npBbValue == rhs as NP_BBValue_Int;
                        case NP_BBValue_String npBbValue:
                            return npBbValue == rhs as NP_BBValue_String;
                        case NP_BBValue_Vector3 npBbValue:
                            return npBbValue == rhs as NP_BBValue_Vector3;
                        case NP_BBValue_Long npBbValue:
                            return npBbValue == rhs as NP_BBValue_Long;
                        case NP_BBValue_List_Long npBbValue:
                            return npBbValue == rhs as NP_BBValue_List_Long;
                        default:
                            log.Error($"Type {lhs.GetType()} is not registered as NP_BBValue");
                            return false;
                    }
                }
                case Operator.IS_NOT_EQUAL:
                {
                    switch (lhs)
                    {
                        case NP_BBValue_Bool npBbValue:
                            return npBbValue != rhs as NP_BBValue_Bool;
                        case NP_BBValue_Float npBbValue:
                            return npBbValue != rhs as NP_BBValue_Float;
                        case NP_BBValue_Int npBbValue:
                            return npBbValue != rhs as NP_BBValue_Int;
                        case NP_BBValue_String npBbValue:
                            return npBbValue != rhs as NP_BBValue_String;
                        case NP_BBValue_Long npBbValue:
                            return npBbValue != rhs as NP_BBValue_Long;
                        case NP_BBValue_Vector3 npBbValue:
                            return npBbValue != rhs as NP_BBValue_Vector3;
                        case NP_BBValue_List_Long npBbValue:
                            return npBbValue != rhs as NP_BBValue_List_Long;
                        case NP_BBValue_List_Byte npBbValue:
                            return npBbValue != rhs as NP_BBValue_List_Byte;
                        default:
                            log.Error($"Type {lhs.GetType()} is not registered as NP_BBValue");
                            return false;
                    }
                }

                case Operator.IS_GREATER_OR_EQUAL:
                {
                    switch (lhs)
                    {
                        case NP_BBValue_Bool npBbValue:
                            return (rhs as NP_BBValue_Bool) >= npBbValue;
                        case NP_BBValue_Float npBbValue:
                            return (rhs as NP_BBValue_Float) >= npBbValue;
                        case NP_BBValue_Int npBbValue:
                            return (rhs as NP_BBValue_Int) >= npBbValue;
                        case NP_BBValue_String npBbValue:
                            return (rhs as NP_BBValue_String) >= npBbValue;
                        case NP_BBValue_Long npBbValue:
                            return (rhs as NP_BBValue_Long) >= npBbValue;
                        case NP_BBValue_Vector3 npBbValue:
                            return (rhs as NP_BBValue_Vector3) >= npBbValue;
                        case NP_BBValue_List_Long npBbValue:
                            return (rhs as NP_BBValue_List_Long) >= npBbValue;
                        default:
                            log.Error($"Type {lhs.GetType()} is not registered as NP_BBValue");
                            return false;
                    }
                }

                case Operator.IS_GREATER:
                {
                    switch (lhs)
                    {
                        case NP_BBValue_Bool npBbValue:
                            return (rhs as NP_BBValue_Bool) > npBbValue;
                        case NP_BBValue_Float npBbValue:
                            return (rhs as NP_BBValue_Float) > npBbValue;
                        case NP_BBValue_Int npBbValue:
                            return (rhs as NP_BBValue_Int) > npBbValue;
                        case NP_BBValue_UInt npBbValue:
                            return (rhs as NP_BBValue_UInt) > npBbValue;
                        case NP_BBValue_String npBbValue:
                            return (rhs as NP_BBValue_String) > npBbValue;
                        case NP_BBValue_Long npBbValue:
                            return (rhs as NP_BBValue_Long) > npBbValue;
                        case NP_BBValue_Vector3 npBbValue:
                            return (rhs as NP_BBValue_Vector3) > npBbValue;
                        case NP_BBValue_List_Long npBbValue:
                            return (rhs as NP_BBValue_List_Long) > npBbValue;
                        default:
                            log.Error($"Type {lhs.GetType()} is not registered as NP_BBValue");
                            return false;
                    }
                }

                case Operator.IS_SMALLER_OR_EQUAL:
                    switch (lhs)
                    {
                        case NP_BBValue_Bool npBbValue:
                            return (rhs as NP_BBValue_Bool) <= npBbValue;
                        case NP_BBValue_Float npBbValue:
                            return (rhs as NP_BBValue_Float) <= npBbValue;
                        case NP_BBValue_Int npBbValue:
                            return (rhs as NP_BBValue_Int) <= npBbValue;
                        case NP_BBValue_UInt npBbValue:
                            return (rhs as NP_BBValue_UInt) <= npBbValue;
                        case NP_BBValue_String npBbValue:
                            return (rhs as NP_BBValue_String) <= npBbValue;
                        case NP_BBValue_Long npBbValue:
                            return (rhs as NP_BBValue_Long) <= npBbValue;
                        case NP_BBValue_Vector3 npBbValue:
                            return (rhs as NP_BBValue_Vector3) <= npBbValue;
                        default:
                            log.Error($"Type {lhs.GetType()} is not registered as NP_BBValue");
                            return false;
                    }
                case Operator.IS_SMALLER:
                    switch (lhs)
                    {
                        case NP_BBValue_Bool npBbValue:
                            return (rhs as NP_BBValue_Bool) < npBbValue;
                        case NP_BBValue_Float npBbValue:
                            return (rhs as NP_BBValue_Float) < npBbValue;
                        case NP_BBValue_Int npBbValue:
                            return (rhs as NP_BBValue_Int) < npBbValue;
                        case NP_BBValue_String npBbValue:
                            return (rhs as NP_BBValue_String) < npBbValue;
                        case NP_BBValue_Long npBbValue:
                            return (rhs as NP_BBValue_Long) < npBbValue;
                        case NP_BBValue_Vector3 npBbValue:
                            return (rhs as NP_BBValue_Vector3) < npBbValue;
                        default:
                            log.Error($"Type {lhs.GetType()} is not registered as NP_BBValue");
                            return false;
                    }

                default: return false;
            }
        }
    }
}
