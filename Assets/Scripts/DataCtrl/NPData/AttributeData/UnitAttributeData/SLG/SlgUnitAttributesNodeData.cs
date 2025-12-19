//------------------------------------------------------------
// File: SlgUnitAttributesNodeData.cs
// Purpose: SLG unit attribute definition (separate from action/hero attributes).
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
    /// SLG单位属性，面向兵种/编制，不耦合动作类数值。
    /// </summary>
    [System.Serializable]
    [BsonSerializer(typeof(AttributesDataSerializer<SlgUnitAttributesNodeData>))]
    public abstract class SlgUnitAttributesNodeData : ICommonAttributeBase
    {
        [BsonElement("typeStr")]
        public string typeStr = "";

        [BsonElement("UnitDataNodeId")]
        public long UnitDataNodeId;

        [BsonElement("UnitName")]
        public string UnitName;

        [BsonElement("UnitSprite")]
        public string UnitSprite;

        [BsonElement("UnitAvatar")]
        public string UnitAvatar;

        [Header("SLG Identity")]
        [Tooltip("战斗站位分类，决定阵列位置与优先级。")]
        [BsonElement("CombatPositionType")]
        public CombatPositionType CombatPositionType;

        [Tooltip("兵种职业大类，用于克制关系。")]
        [BsonElement("UnitClassType")]
        public UnitClassType UnitClassType;

        [Tooltip("护甲类型，配合伤害类型决定克制。")]
        [BsonElement("ArmorType")]
        public ArmorType ArmorType;

        [Tooltip("主要伤害类型。")]
        [BsonElement("DamageType")]
        public DamageType DamageType;

        [Header("SLG Stats")]
        [Tooltip("基础生命值。")]
        [BsonElement("BaseHp")]
        public float BaseHp;

        [Tooltip("基础攻击力。")]
        [BsonElement("Attack")]
        public float Attack;

        [Tooltip("攻击距离（米）。")]
        [BsonElement("AttackRange")]
        public float AttackRange;

        [Tooltip("移动速度。")]
        [BsonElement("MoveSpeed")]
        public float MoveSpeed;

        [Tooltip("人口/编制占用。")]
        [BsonElement("PopulationCost")]
        public int PopulationCost;

        public string BuildRoleKey()
        {
            string name = string.IsNullOrEmpty(UnitName) ? "Unnamed" : UnitName;
            string sprite = string.IsNullOrEmpty(UnitSprite) ? "UnknownSprite" : UnitSprite;
            return $"{UnitClassType}_{CombatPositionType}_{sprite}_{name}";
        }

        public long GenerateRoleIdFromData()
        {
            string roleKey = BuildRoleKey();
            long roleId = GlobalHelper.GetRoleID(roleKey);
            if (roleId < 0)
            {
                roleId = 0;
            }
            UnitDataNodeId = roleId;
            return roleId;
        }

        public void CommitRoleChanges()
        {
            // Hook for editor persistence if needed.
        }
    }

    public enum CombatPositionType
    {
        Melee = 0,
        MidRange = 1,
        Ranged = 2
    }

    public enum UnitClassType
    {
        Infantry = 0,
        Cavalry = 1,
        Archer = 2,
        Siege = 3
    }

    public enum ArmorType
    {
        Unarmored = 0,
        Light = 1,
        Medium = 2,
        Heavy = 3,
        Fortified = 4
    }

    public enum DamageType
    {
        Slash = 0,
        Pierce = 1,
        Blunt = 2,
        Magic = 3,
        Siege = 4
    }

#if UNITY_EDITOR
    /// <summary>
    /// Inspector layout for SLG unit attributes, mirroring UnitAttributesNodeDataBase drawer style.
    /// </summary>
    [CustomPropertyDrawer(typeof(SlgUnitAttributesNodeData), true)]
    public class SlgUnitAttributesNodeDataDrawer : PropertyDrawer
    {
        private static readonly string[] BaseProperties =
        {
            "UnitDataNodeId",
            "UnitName",
            "UnitSprite",
            "UnitAvatar"
        };

        private static readonly string[] IdentityProperties =
        {
            "CombatPositionType",
            "UnitClassType",
            "ArmorType",
            "DamageType"
        };

        private static readonly string[] StatProperties =
        {
            "BaseHp",
            "Attack",
            "AttackRange",
            "MoveSpeed",
            "PopulationCost",
            "SquadSize"
        };

        private static readonly Dictionary<string, int> TabSelectionStates = new Dictionary<string, int>();
        private static readonly string[] TabTitles = { "Identity", "Stats" };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight; // foldout
            if (!property.isExpanded)
                return height;

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

        private static float GetGroupedHeight(SerializedProperty root, IReadOnlyList<string> names)
        {
            float height = 0f;
            foreach (var name in names)
            {
                var child = root.FindPropertyRelative(name);
                if (child == null) continue;
                height += EditorGUI.GetPropertyHeight(child, true) + EditorGUIUtility.standardVerticalSpacing;
            }
            if (height > 0f)
                height -= EditorGUIUtility.standardVerticalSpacing;
            return height;
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

        private static int DrawTabs(Rect rect, int currentIndex)
        {
            return GUI.Toolbar(rect, currentIndex, TabTitles);
        }

        private static IReadOnlyList<string> GetActiveTabProperties(string key)
        {
            int idx = GetTabIndex(key);
            return idx == 0 ? (IReadOnlyList<string>)IdentityProperties : StatProperties;
        }

        private static int GetTabIndex(string key)
        {
            if (!TabSelectionStates.TryGetValue(key, out var idx))
            {
                idx = 0;
                TabSelectionStates[key] = idx;
            }
            return Mathf.Clamp(idx, 0, TabTitles.Length - 1);
        }

        private static void SetTabIndex(string key, int idx)
        {
            TabSelectionStates[key] = Mathf.Clamp(idx, 0, TabTitles.Length - 1);
        }

        private static void DrawActionButtons(Rect fullPosition, float startY, SerializedProperty property)
        {
            Rect row = EditorGUI.IndentedRect(new Rect(fullPosition.x, startY, fullPosition.width, EditorGUIUtility.singleLineHeight));
            float buttonWidth = (row.width - 4f) / 2f;

            Rect generateRect = new Rect(row.x, row.y, buttonWidth, row.height);
            Rect copyRect = new Rect(generateRect.xMax + 4f, row.y, buttonWidth, row.height);

            if (GUI.Button(generateRect, "Generate Role ID"))
            {
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
                long generatedId = GenerateRoleId(property);
                Debug.Log($"Generated role id: {generatedId}");
                property.serializedObject.ApplyModifiedProperties();
            }

            if (GUI.Button(copyRect, "Copy Role ID"))
            {
                var idProp = property.FindPropertyRelative("UnitDataNodeId");
                if (idProp != null)
                {
                    EditorGUIUtility.systemCopyBuffer = idProp.longValue.ToString();
                    Debug.Log($"Copied role id: {EditorGUIUtility.systemCopyBuffer}");
                }
            }
        }

        private static long GenerateRoleId(SerializedProperty property)
        {
            string roleKey = BuildRoleKey(property);
            long roleId = GlobalHelper.GetRoleID(roleKey);
            var idProp = property.FindPropertyRelative("UnitDataNodeId");
            if (idProp != null)
            {
                idProp.longValue = roleId < 0 ? 0 : roleId;
            }
            return roleId;
        }

        private static string BuildRoleKey(SerializedProperty property)
        {
            string unitName = property.FindPropertyRelative("UnitName")?.stringValue ?? "Unnamed";
            string sprite = property.FindPropertyRelative("UnitSprite")?.stringValue ?? "UnknownSprite";
            string classType = GetEnumName(property.FindPropertyRelative("UnitClassType"));
            string posType = GetEnumName(property.FindPropertyRelative("CombatPositionType"));
            if (string.IsNullOrEmpty(unitName)) unitName = "Unnamed";
            if (string.IsNullOrEmpty(sprite)) sprite = "UnknownSprite";
            return $"{classType}_{posType}_{sprite}_{unitName}";
        }

        private static string GetEnumName(SerializedProperty enumProp)
        {
            if (enumProp == null || enumProp.propertyType != SerializedPropertyType.Enum)
                return "Unknown";
            string[] names = enumProp.enumNames;
            int idx = Mathf.Clamp(enumProp.enumValueIndex, 0, names.Length - 1);
            return names[idx];
        }
    }
#endif
}
