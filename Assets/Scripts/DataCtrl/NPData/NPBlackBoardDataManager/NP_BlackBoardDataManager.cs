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
    [System.Serializable]
    public class NP_BlackBoardDataManager : ISerializationCallbackReceiver
    {
        // Runtime/Logical Storage
        public Dictionary<string, ANP_BBValue> BBValues = new Dictionary<string, ANP_BBValue>();
        public List<string> EventValues = new List<string>();
        public Dictionary<string,long> Ids = new Dictionary<string, long>();

        // Serialization Backing Fields
        [SerializeField] private List<string> _bbValueKeys = new List<string>();
        [SerializeReference] private List<ANP_BBValue> _bbValueValues = new List<ANP_BBValue>(); // Polymorphic serialization for ANP_BBValue

        [SerializeField] private List<string> _idKeys = new List<string>();
        [SerializeField] private List<long> _idValues = new List<long>();

        public void OnBeforeSerialize()
        {
            _bbValueKeys.Clear();
            _bbValueValues.Clear();
            foreach (var kvp in BBValues)
            {
                _bbValueKeys.Add(kvp.Key);
                _bbValueValues.Add(kvp.Value);
            }

            _idKeys.Clear();
            _idValues.Clear();
            foreach (var kvp in Ids)
            {
                _idKeys.Add(kvp.Key);
                _idValues.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            BBValues.Clear();
            if (_bbValueKeys.Count == _bbValueValues.Count)
            {
                for (int i = 0; i < _bbValueKeys.Count; i++)
                {
                    if (!string.IsNullOrEmpty(_bbValueKeys[i]) && _bbValueValues[i] != null)
                        BBValues[_bbValueKeys[i]] = _bbValueValues[i];
                }
            }

            Ids.Clear();
            if (_idKeys.Count == _idValues.Count)
            {
                for (int i = 0; i < _idKeys.Count; i++)
                {
                    if (!string.IsNullOrEmpty(_idKeys[i]))
                        Ids[_idKeys[i]] = _idValues[i];
                }
            }
        }

#if UNITY_EDITOR
        public static NP_BlackBoardDataManager CurrentEditedNP_BlackBoardDataManager;
#endif
    }
#endif
}
