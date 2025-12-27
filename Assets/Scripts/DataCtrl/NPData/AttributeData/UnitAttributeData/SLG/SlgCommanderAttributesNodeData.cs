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
    public abstract class SlgCommanderAttributesNodeData : ICommonAttributeBase
    {
        [BsonElement("typeStr")]
        public string typeStr = "Commander";

        [BsonElement("UnitDataNodeId")]
        public long UnitDataNodeId;
        
        [Tooltip("领主名称/ID标识")]
        [BsonElement("UnitName")]
        public string UnitName;
        
        [Tooltip("领主等级")]
        [BsonElement("Level")]
        public int Level = 1;

        // --- Global Military Modifiers (BaseAdd) ---
        [Tooltip("全局步兵攻击加成 (1000 = 10%)")]
        [BsonElement("GlobalInfantryAtkMod")]
        public int GlobalInfantryAtkMod;

        [Tooltip("全局行军速度加成")]
        [BsonElement("GlobalMarchSpeedMod")]
        public int GlobalMarchSpeedMod;

        // --- Global Economy Modifiers (BaseAdd) ---
        [Tooltip("全局建造速度加成")]
        [BsonElement("GlobalConstructionSpeedMod")]
        public int GlobalConstructionSpeedMod;
        
        [Tooltip("全局研发速度加成")]
        [BsonElement("GlobalResearchSpeedMod")]
        public int GlobalResearchSpeedMod;
        
        public virtual string BuildRoleKey()
        {
            return $"{UnitName}_{Level}";
        }

        public virtual long GenerateRoleIdFromData()
        {
            string key = BuildRoleKey();
            long id = GlobalHelper.GetRoleID(key);
            if (id < 0) 
                id = 0;
            UnitDataNodeId= id;
            return id;
        }

        public virtual void CommitRoleChanges()
        {
            // Hook
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SlgCommanderAttributesNodeData), true)]
    public class SlgCommanderAttributesNodeDataDrawer : PropertyDrawer
    {
        private static readonly string[] BaseProperties =
        {
            "UnitDataNodeId",
            "UnitName",
            "Level"
        };

        private static readonly string[] MilitaryProperties =
        {
            "GlobalInfantryAtkMod",
            "GlobalMarchSpeedMod"
        };
        
        private static readonly string[] EconomyProperties =
        {
            "GlobalConstructionSpeedMod",
            "GlobalResearchSpeedMod"
        };

        private static readonly Dictionary<string, int> TabSelectionStates = new Dictionary<string, int>();
        private static readonly string[] TabTitles = { "Military", "Economy" };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight; // foldout
            if (!property.isExpanded) return height;

            height += EditorGUIUtility.standardVerticalSpacing;
            height += GetGroupedHeight(property, BaseProperties);
            
            height += EditorGUIUtility.standardVerticalSpacing;
            height += EditorGUIUtility.singleLineHeight; // tabs
            
            height += EditorGUIUtility.standardVerticalSpacing;
            height += GetGroupedHeight(property, GetActiveTabProperties(property.propertyPath));
            
            height += EditorGUIUtility.standardVerticalSpacing;
            height += EditorGUIUtility.singleLineHeight; // buttons
            
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);
            if (!property.isExpanded)
            {
                EditorGUI.EndProperty();
                return;
            }

            float y = foldoutRect.yMax + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.indentLevel++;

            y = DrawPropertyGroup(position, y, property, BaseProperties);
            y += EditorGUIUtility.standardVerticalSpacing;

            Rect tabRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
            int tabIndex = DrawTabs(tabRect, GetTabIndex(property.propertyPath));
            SetTabIndex(property.propertyPath, tabIndex);
            y = tabRect.yMax + EditorGUIUtility.standardVerticalSpacing;

            y = DrawPropertyGroup(position, y, property, GetActiveTabProperties(property.propertyPath));
            y += EditorGUIUtility.standardVerticalSpacing;

            DrawActionButtons(position, y, property);

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        // Helper methods mirror SlgUnitSquadAttributesNodeDataDrawer
        private static float GetGroupedHeight(SerializedProperty root, IReadOnlyList<string> names)
        {
            float h = 0f;
            foreach (var name in names)
            {
                var child = root.FindPropertyRelative(name);
                if (child != null) h += EditorGUI.GetPropertyHeight(child, true) + EditorGUIUtility.standardVerticalSpacing;
            }
            if (h > 0) h -= EditorGUIUtility.standardVerticalSpacing;
            return h;
        }

        private static float DrawPropertyGroup(Rect fullPosition, float startY, SerializedProperty root, IReadOnlyList<string> names)
        {
            float y = startY;
            foreach (var name in names)
            {
                var child = root.FindPropertyRelative(name);
                if (child == null) continue;
                float h = EditorGUI.GetPropertyHeight(child, true);
                Rect r = new Rect(fullPosition.x, y, fullPosition.width, h);
                bool readonlyId = name == "UnitDataNodeId";
                if (readonlyId) EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(r, child, true);
                if (readonlyId) EditorGUI.EndDisabledGroup();
                y += h + EditorGUIUtility.standardVerticalSpacing;
            }
            return y;
        }

        private static int DrawTabs(Rect rect, int idx) => GUI.Toolbar(rect, idx, TabTitles);
        private static int GetTabIndex(string key) => TabSelectionStates.ContainsKey(key) ? TabSelectionStates[key] : 0;
        private static void SetTabIndex(string key, int idx) => TabSelectionStates[key] = idx;
        private static IReadOnlyList<string> GetActiveTabProperties(string key) => GetTabIndex(key) == 0 ? MilitaryProperties : EconomyProperties;

        private static void DrawActionButtons(Rect fullPosition, float startY, SerializedProperty property)
        {
            Rect row = EditorGUI.IndentedRect(new Rect(fullPosition.x, startY, fullPosition.width, EditorGUIUtility.singleLineHeight));
            float btnW = (row.width - 4) / 2;
            if (GUI.Button(new Rect(row.x, row.y, btnW, row.height), "Generate ID"))
            {
                 property.serializedObject.ApplyModifiedProperties();
                 long id = GenerateId(property);
                 Debug.Log($"Generated ID: {id}");
            }
            
            if (GUI.Button(new Rect(row.x + btnW + 4, row.y, btnW, row.height), "Copy ID"))
            {
                var idProp = property.FindPropertyRelative("UnitDataNodeId");
                if (idProp != null)
                {
                    EditorGUIUtility.systemCopyBuffer = idProp.longValue.ToString();
                    Debug.Log($"Copied ID to clipboard: {idProp.longValue}");
                }
            }
        }
        
        private static long GenerateId(SerializedProperty property)
        {
            string name = property.FindPropertyRelative("UnitName")?.stringValue ?? "Cmd";
            int level = property.FindPropertyRelative("Level")?.intValue ?? 1;
            string key = $"{name}_{level}";
            long id = GlobalHelper.GetRoleID(key);
            var idProp = property.FindPropertyRelative("UnitDataNodeId");
            if (idProp != null) idProp.longValue = id < 0 ? 0 : id;
            return id;
        }
    }
#endif
}
