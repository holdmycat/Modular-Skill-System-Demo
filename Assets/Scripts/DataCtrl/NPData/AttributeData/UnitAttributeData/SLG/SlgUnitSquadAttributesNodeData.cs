//------------------------------------------------------------
// File: SlgUnitSquadAttributesNodeData.cs
// Purpose: Squad-level default spawn attributes for each unit type.
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
    /// SLG Squad 属性基类：描述某兵种对应的编制默认参数（人数、阵型、出生偏移等）。
    /// </summary>
    [System.Serializable]
    [BsonSerializer(typeof(AttributesDataSerializer<SlgUnitSquadAttributesNodeData>))]
    public abstract class SlgUnitSquadAttributesNodeData : ICommonAttributeBase
    {
        [BsonElement("typeStr")]
        public string typeStr = "";

        [BsonElement("UnitDataNodeId")]
        public long UnitDataNodeId;
        
        [Tooltip("关联的兵种 UnitId，对应 SlgUnitAttributesNodeData.UnitDataNodeId")]
        [BsonElement("UnitId")]
        public long UnitId;

        [BsonElement("UnitName")]
        public string UnitName;
        
        [BsonElement("UnitSprite")]
        public string UnitSprite;
        
        [Tooltip("初始人数")]
        [BsonElement("InitialCount")]
        public int InitialCount = 10;

        [Tooltip("编制上限")]
        [BsonElement("MaxCount")]
        public int MaxCount = 10;

        [Tooltip("初始阵型标识")]
        [BsonElement("Formation")]
        public string Formation = "Default";

        [Tooltip("出生位置偏移（相对队伍根节点）")]
        [BsonElement("SpawnOffset")]
        public Vector3 SpawnOffset;

        [Tooltip("出生朝向（度）")]
        [BsonElement("SpawnYaw")]
        public float SpawnYaw;

        public virtual string BuildRoleKey()
        {
            return $"{UnitId}_{Formation}_{MaxCount}";
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
            // Hook for editor persistence if needed.
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Inspector drawer for SLG squad attributes, matching the hero/unit drawer style.
    /// </summary>
    [CustomPropertyDrawer(typeof(SlgUnitSquadAttributesNodeData), true)]
    public class SlgUnitSquadAttributesNodeDataDrawer : PropertyDrawer
    {
        private static readonly string[] BaseProperties =
        {
            "UnitDataNodeId",
            "UnitName",
            "UnitId",
            "Formation"
        };

        private static readonly string[] CountProperties =
        {
            "InitialCount",
            "MaxCount"
        };

        private static readonly string[] SpawnProperties =
        {
            "SpawnOffset",
            "SpawnYaw"
        };

        private static readonly Dictionary<string, int> TabSelectionStates = new Dictionary<string, int>();
        private static readonly string[] TabTitles = { "Counts", "Spawn" };

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
            return idx == 0 ? (IReadOnlyList<string>)CountProperties : SpawnProperties;
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

            if (GUI.Button(generateRect, "Generate Squad ID"))
            {
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
                long generatedId = GenerateSquadId(property);
                Debug.Log($"Generated squad id: {generatedId}");
                property.serializedObject.ApplyModifiedProperties();
            }

            if (GUI.Button(copyRect, "Copy Squad ID"))
            {
                var idProp = property.FindPropertyRelative("UnitDataNodeId");
                if (idProp != null)
                {
                    EditorGUIUtility.systemCopyBuffer = idProp.longValue.ToString();
                    Debug.Log($"Copied squad id: {EditorGUIUtility.systemCopyBuffer}");
                }
            }
        }

        private static long GenerateSquadId(SerializedProperty property)
        {
            string key = BuildRoleKey(property);
            long id = GlobalHelper.GetRoleID(key);
            var idProp = property.FindPropertyRelative("UnitDataNodeId");
            if (idProp != null)
            {
                idProp.longValue = id < 0 ? 0 : id;
            }
            return id;
        }

        private static string BuildRoleKey(SerializedProperty property)
        {
            long unitId = property.FindPropertyRelative("UnitId")?.longValue ?? 0;
            string formation = property.FindPropertyRelative("Formation")?.stringValue ?? "Default";
            int maxCount = property.FindPropertyRelative("MaxCount")?.intValue ?? 0;
            return $"{unitId}_{formation}_{maxCount}";
        }
    }
#endif
}
