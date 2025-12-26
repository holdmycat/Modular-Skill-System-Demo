//------------------------------------------------------------
// File: CommanderAttributesNodeData.cs
// Purpose: Commander-level default attributes (Global Modifiers).
//------------------------------------------------------------

using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;
using Ebonor.Framework;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// SLG Commander 属性基类：描述领主全局加成 (Modifiers)。
    /// </summary>
    [System.Serializable]
    [BsonSerializer(typeof(AttributesDataSerializer<SlgCommanderAttributesNodeData>))]
    public class SergeyCommanderAttributesNodeData  : SlgCommanderAttributesNodeData
    {
        
    }

}
