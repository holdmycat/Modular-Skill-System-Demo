//------------------------------------------------------------
// File: NP_BlackBoardRelationData.cs
// Created: 2025-12-01
// Purpose: Blackboard relation configuration for behavior trees.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Ebonor.Framework;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Blackboard relation data used by behavior trees.
    /// </summary>
    public class NP_BlackBoardRelationData
    {
        
        static readonly ILog log = LogManager.GetLogger(typeof(NP_BlackBoardRelationData));
        
        [Tooltip("Blackboard key to read or write.")]
        public string BBKey;

        [Tooltip("Resolved blackboard value type.")]
        [SerializeField]
        public string NP_BBValueType;

        [Tooltip("Whether to write to or compare against the blackboard.")]
        [BsonIgnore]
        public bool WriteOrCompareToBB;
        
        [Tooltip("Blackboard value payload when writing or comparing.")]
        public ANP_BBValue NP_BBValue;

#if UNITY_EDITOR
        private IEnumerable<string> GetBBKeys()
        {
            if (NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager != null)
            {
                return NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager.BBValues.Keys;
            }
        
            return null;
        }
        
        private void OnBBKeySelected()
        {
            if (NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager != null)
            {
                foreach (var bbValues in NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager.BBValues)
                {
                    if (bbValues.Key == this.BBKey)
                    {
                        NP_BBValue = bbValues.Value.DeepCopy();
                        NP_BBValueType = this.NP_BBValue.NP_BBValueType.ToString();
                    }
                }
            }
        }
#endif

        /// <summary>
        /// Get the blackboard value for the configured key.
        /// </summary>
        public T GetBlackBoardValue<T>(Blackboard blackboard)
        {
            return blackboard.Get<T>(this.BBKey);
        }

        /// <summary>
        /// Get the configured payload value.
        /// </summary>
        public T GetTheBBDataValue<T>()
        {
            return (this.NP_BBValue as NP_BBValueBase<T>).GetValue();
        }

        /// <summary>
        /// Set the blackboard value using the configured payload.
        /// </summary>
        /// <param name="blackboard">Target blackboard.</param>
        public void SetBlackBoardValue(Blackboard blackboard)
        {
            
#if UNITY_EDITOR
            if (BBKey.Equals(ConstData.BB_ISIDLE))
            {
                if (NP_BBValue is NP_BBValue_Bool valueBool && valueBool.Value == false)
                {
                    log.DebugFormat("SetBlackBoardValue, BBKey:{0}, BBValue:{1}", BBKey, valueBool.Value);
                }
               
            }
#endif
            NP_BBValueHelper.SetTargetBlackboardUseANP_BBValue(this.NP_BBValue, blackboard, BBKey);
        }

        /// <summary>
        /// Set the blackboard value using an external value.
        /// </summary>
        /// <param name="blackboard">Target blackboard.</param>
        /// <param name="value">Value to assign.</param>
        public void SetBlackBoardValue<T>(Blackboard blackboard, T value)
        {
            blackboard.Set(this.BBKey, value);
        }

        /// <summary>
        /// Copy a value from one blackboard to another using the same key.
        /// </summary>
        /// <param name="oriBB">Source blackboard.</param>
        /// <param name="desBB">Destination blackboard.</param>
        public void SetBBValueFromThisBBValue(Blackboard oriBB, Blackboard desBB)
        {
            //NP_BBValueHelper.SetTargetBlackboardUseANP_BBValue(oriBB.Get(BBKey), desBB, BBKey);
        }
    }
}
