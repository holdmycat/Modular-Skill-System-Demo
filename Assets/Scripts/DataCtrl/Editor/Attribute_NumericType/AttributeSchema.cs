using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ebonor.DataCtrl.Editor
{
    public enum AttributeType
    {
        FinalOnly,
        BaseAdd
    }

    [Serializable]
    public class AttributeDefinition
    {
        public string Name;
        public int Id;
        public AttributeType Type;
        public string Description;
    }

    [CreateAssetMenu(fileName = "AttributeSchema", menuName = "Ebonor/Attribute Schema")]
    public class AttributeSchema : ScriptableObject
    {
        public List<AttributeDefinition> Attributes = new List<AttributeDefinition>();
    }
}
